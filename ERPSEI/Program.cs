using ERPSEI;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Email;
using ERPSEI.Middleware;
using ERPSEI.Services;
using ERPSEI.Services.CorreosDominios;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder =
    WebApplication.CreateBuilder(args);

// =====================================================
// LÍMITE GENERAL DE SOLICITUDES
// =====================================================

/*
 * Kestrel permitirá solicitudes de hasta 110 MB.
 *
 * El módulo Compliance validará archivos individuales
 * de hasta 100 MB. Los 10 MB adicionales sirven como
 * margen para los encabezados y campos del formulario
 * multipart/form-data.
 */
const long limiteSolicitudDocumental =
    110L * 1024L * 1024L;

builder.WebHost.ConfigureKestrel(
    options =>
    {
        options.Limits.MaxRequestBodySize =
            limiteSolicitudDocumental;
    }
);

// =====================================================
// DATAPROTECTION
// =====================================================

builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo(
            @"C:\DataProtectionKeys\IntranetSEI"
        )
    )
    .SetApplicationName(
        "IntranetSEI"
    );

// =====================================================
// CONFIGURACIÓN DE SERVICIOS
// =====================================================

// Email
ServicesConfiguration.ConfigureEmail(
    builder
);

// Base de datos
ServicesConfiguration.ConfigureDatabase(
    builder
);

// Identity
ServicesConfiguration.ConfigureIdentity(
    builder
);

// Razor Pages y localización
ServicesConfiguration
    .ConfigurePagesAndLocalization(
        builder
    );

// Autorización
ServicesConfiguration
    .ConfigureAuthorization(
        builder
    );

// Inyección de dependencias
ServicesConfiguration
    .ConfigureDependencyInjection(
        builder
    );

/*
 * Aquí se configura MultipartBodyLengthLimit
 * en 110 MB desde ServicesConfiguration.
 *
 * No debe agregarse otra configuración
 * de FormOptions directamente en Program.cs.
 */
ServicesConfiguration.ConfigureFormOptions(
    builder
);

// =====================================================
// AUDITORÍA GLOBAL
// =====================================================

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ERPSEI.Data.Entities.Metricas.AuditoriaContext>();

// =====================================================
// SERVICIOS EN SEGUNDO PLANO
// =====================================================

builder.Services.AddHostedService<
    EventosProgramadosBackgroundService>();

builder.Services.AddHostedService<
    CorreoDominioCaducidadService>();

// =====================================================
// CONSTRUIR APLICACIÓN
// =====================================================

WebApplication app =
    builder.Build();

// =====================================================
// ENCABEZADOS DEL PROXY INVERSO
// =====================================================

app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders =
            ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto
    }
);

// =====================================================
// INICIALIZACIÓN DE ROLES Y PERMISOS
// =====================================================

using (
    IServiceScope scope =
        app.Services.CreateScope()
)
{
    RoleManager<AppRole> roleManager =
        scope.ServiceProvider
            .GetRequiredService<
                RoleManager<AppRole>>();

    // =================================================
    // CREAR ROLES PRINCIPALES
    // =================================================

    if (!await roleManager.RoleExistsAsync(
            ServicesConfiguration.RolMaster))
    {
        await roleManager.CreateAsync(
            new AppRole(
                ServicesConfiguration.RolMaster
            )
        );
    }

    if (!await roleManager.RoleExistsAsync(
            ServicesConfiguration.RolAdministrador))
    {
        await roleManager.CreateAsync(
            new AppRole(
                ServicesConfiguration.RolAdministrador
            )
        );
    }

    if (!await roleManager.RoleExistsAsync(
            ServicesConfiguration.RolUsuario))
    {
        await roleManager.CreateAsync(
            new AppRole(
                ServicesConfiguration.RolUsuario
            )
        );
    }

    if (!await roleManager.RoleExistsAsync(
            ServicesConfiguration.RolCandidato))
    {
        await roleManager.CreateAsync(
            new AppRole(
                ServicesConfiguration.RolCandidato
            )
        );
    }

    // =================================================
    // INICIALIZAR ACCESOS A MÓDULOS
    // =================================================

    IAccesoModuloManager accesoModuloManager =
        scope.ServiceProvider
            .GetRequiredService<
                IAccesoModuloManager>();

    IModuloManager moduloManager =
        scope.ServiceProvider
            .GetRequiredService<
                IModuloManager>();

    List<AppRole> roles =
        await roleManager.Roles
            .ToListAsync();

    List<Modulo> modulos =
        (
            await moduloManager.GetAllAsync()
        ).ToList();

    foreach (AppRole rol in roles)
    {
        List<AccesoModulo> accesos =
            await accesoModuloManager
                .GetByRolIdAsync(
                    rol.Id
                );

        foreach (Modulo modulo in modulos)
        {
            bool accesoExistente =
                accesos.Exists(
                    acceso =>
                        acceso.Modulo?
                            .NombreNormalizado ==
                        modulo.NombreNormalizado
                );

            if (accesoExistente)
            {
                continue;
            }

            switch (rol.Name)
            {
                case ServicesConfiguration.RolMaster:

                    /*
                     * Master tiene acceso completo
                     * a todos los módulos.
                     */
                    await accesoModuloManager
                        .CreateAsync(
                            new AccesoModulo
                            {
                                RolId = rol.Id,
                                ModuloId = modulo.Id,

                                PuedeTodo = 1,
                                PuedeConsultar = 1,
                                PuedeEditar = 1,
                                PuedeEliminar = 1,
                                PuedeAutorizar = 1
                            }
                        );

                    break;

                case ServicesConfiguration.RolAdministrador:

                    /*
                     * Administrador puede consultar
                     * y editar todos los módulos.
                     */
                    await accesoModuloManager
                        .CreateAsync(
                            new AccesoModulo
                            {
                                RolId = rol.Id,
                                ModuloId = modulo.Id,

                                PuedeTodo = 0,
                                PuedeConsultar = 1,
                                PuedeEditar = 1,
                                PuedeEliminar = 0,
                                PuedeAutorizar = 0
                            }
                        );

                    break;

                case ServicesConfiguration.RolUsuario:

                    /*
                     * Usuario solo puede consultar
                     * determinados módulos.
                     */
                    switch (
                        modulo.NombreNormalizado
                    )
                    {
                        case "vacaciones":
                        case "incapacidades":
                        case "permisos":
                        case "organigrama":
                        case "activosfijos":

                            await accesoModuloManager
                                .CreateAsync(
                                    new AccesoModulo
                                    {
                                        RolId = rol.Id,
                                        ModuloId =
                                            modulo.Id,

                                        PuedeTodo = 0,
                                        PuedeConsultar = 1,
                                        PuedeEditar = 0,
                                        PuedeEliminar = 0,
                                        PuedeAutorizar = 0
                                    }
                                );

                            break;

                        default:

                            /*
                             * El resto de los módulos
                             * permanece sin acceso.
                             */
                            break;
                    }

                    break;

                default:

                    /*
                     * Candidato y otros roles
                     * no reciben permisos iniciales.
                     */
                    break;
            }
        }
    }

    // =================================================
    // CREAR USUARIO MASTER
    // =================================================

    AppUserManager userManager =
        scope.ServiceProvider
            .GetRequiredService<
                AppUserManager>();

    AppUser? usuarioMaster =
        await userManager.Users
            .FirstOrDefaultAsync(
                usuario =>
                    usuario.IsMaster
            );

    if (usuarioMaster == null)
    {
        ServicesConfiguration.MasterPassword =
            userManager.GenerateRandomPassword(
                10
            );

        IdentityResult resultado =
            await userManager.CreateAsync(
                ServicesConfiguration.MasterUser,
                ServicesConfiguration
                    .MasterPassword
            );

        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(
                ServicesConfiguration.MasterUser,
                ServicesConfiguration.RolMaster
            );

            IEmailSender emailSender =
                scope.ServiceProvider
                    .GetRequiredService<
                        IEmailSender>();

            await emailSender.SendEmailAsync(
                ServicesConfiguration
                    .MasterUser
                    .Email ??
                string.Empty,

                "Login Password",

                $"Use this password to login: " +
                $"{ServicesConfiguration.MasterPassword}"
            );
        }
    }
}

// =====================================================
// PIPELINE HTTP
// =====================================================

app.UseSession();

app.UseRequestLocalization();

// =====================================================
// MANEJO DE ERRORES
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler(
        "/Error"
    );

    app.UseHsts();
}

// Se mantiene deshabilitado porque el HTTPS
// puede gestionarse mediante Nginx.
// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseMiddleware<
    IntranetActividadMiddleware>();

app.UseAuthorization();

app.MapRazorPages();

app.Run();