
using ERPSEI.Data;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Entities.Empleados;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ERPSEI.Services.Adquisiciones
{
    public static class AdquisicionesQaSeeder
    {
        private sealed record PerfilQa(
            string Email,
            string Nombre,
            int? AreaId,
            string Rol,
            bool EsCentral
        );

        public static async Task EjecutarAsync(
            IServiceProvider services,
            IHostEnvironment environment)
        {
            // ==========================================
            // 1. VALIDACIÓN DEL ENTORNO
            // ==========================================

            if (!environment.IsDevelopment())
            {
                throw new InvalidOperationException(
                    "QA Adquisiciones: entorno no autorizado.");
            }

            using var scope = services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AppUser>>();

            var databaseName = context.Database
                .GetDbConnection().Database;

            if (!string.Equals(
                    databaseName,
                    "erpsei_local",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Base de datos no autorizada: {databaseName}");
            }

            var password = Environment.GetEnvironmentVariable(
                "ADQ_QA_PASSWORD");

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Configura la variable ADQ_QA_PASSWORD.");
            }

            // ==========================================
            // 2. PERFILES DE PRUEBA
            // ==========================================

            var perfiles = new[]
            {
                new PerfilQa(
                    "qa.sistemas@example.test",
                    "Sistemas",
                    16,
                    "Adquisiciones Sistemas",
                    false),

                new PerfilQa(
                    "qa.th@example.test",
                    "Talento Humano",
                    13,
                    "Adquisiciones Talento Humano",
                    false),

                new PerfilQa(
                    "qa.central@example.test",
                    "Adquisiciones Central",
                    null,
                    "Administrador Adquisiciones",
                    true)
            };

            var strategy = context.Database
                .CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await context.Database.BeginTransactionAsync(
                        System.Data.IsolationLevel.Serializable);

                try
                {
                    // No modificar cuentas existentes.
                    foreach (var perfil in perfiles)
                    {
                        var existente =
                            await userManager.FindByEmailAsync(
                                perfil.Email);

                        var existenteNombre =
                            await userManager.FindByNameAsync(
                                perfil.Email);

                        if (existente != null ||
                            existenteNombre != null)
                        {
                            throw new InvalidOperationException(
                                $"La cuenta {perfil.Email} ya existe.");
                        }
                    }

                    var siguienteEmpleadoId =
                        (await context.Set<Empleado>()
                            .MaxAsync(e => (int?)e.Id) ?? 0) + 1;

                    foreach (var perfil in perfiles)
                    {
                        // ==================================
                        // 3. CREACIÓN DEL USUARIO
                        // ==================================

                        var usuario = new AppUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = perfil.Email,
                            Email = perfil.Email,
                            EmailConfirmed = true,
                            IsBanned = false,
                            IsMaster = false,
                            IsPreregisterAuthorized = true,
                            PasswordResetNeeded = false,
                            LockoutEnabled = true,
                            TwoFactorEnabled = false
                        };

                        var resultado =
                            await userManager.CreateAsync(
                                usuario,
                                password);

                        ValidarResultado(
                            resultado,
                            $"Crear usuario {perfil.Email}");

                        // ==================================
                        // 4. CREACIÓN DEL EMPLEADO
                        // ==================================

                        var empleado = new Empleado
                        {
                            Id = siguienteEmpleadoId++,

                            Nombre = "QA",
                            NombrePreferido =
                                $"QA {perfil.Nombre}",

                            ApellidoPaterno = perfil.Nombre,
                            ApellidoMaterno = "Pruebas",

                            NombreCompleto =
                                $"QA {perfil.Nombre} Pruebas",

                            FechaIngreso = DateTime.Today,
                            FechaNacimiento =
                                new DateTime(1990, 1, 1),

                            Direccion = string.Empty,
                            Telefono = string.Empty,
                            Email = perfil.Email,

                            CURP = string.Empty,
                            RFC = string.Empty,
                            NSS = string.Empty,

                            AreaId = perfil.AreaId,
                            UserId = usuario.Id,

                            Deshabilitado = 0,
                            CalcularAsistencia = false
                        };

                        context.Set<Empleado>().Add(empleado);

                        await context.SaveChangesAsync();

                        // ==================================
                        // 5. VINCULAR USUARIO Y EMPLEADO
                        // ==================================

                        usuario.EmpleadoId = empleado.Id;

                        var actualizacion =
                            await userManager.UpdateAsync(usuario);

                        ValidarResultado(
                            actualizacion,
                            "Vincular empleado");

                        // ==================================
                        // 6. ASIGNACIÓN DEL ROL
                        // ==================================

                        var resultadoRol =
                            await userManager.AddToRoleAsync(
                                usuario,
                                perfil.Rol);

                        ValidarResultado(
                            resultadoRol,
                            $"Asignar rol {perfil.Rol}");

                        // ==================================
                        // 7. PERMISOS ABAC
                        // ==================================

                        await context.Database
                            .ExecuteSqlInterpolatedAsync($@"
                                INSERT INTO dbo.ADQ_PermisosUsuarios
                                (
                                    UsuarioId,
                                    PuedeVisualizar,
                                    PuedeCrearSolicitud,
                                    PuedeGestionarSolicitudes,
                                    PuedeAprobar,
                                    PuedeAsignar,
                                    PuedeCotizar,
                                    PuedeGestionarProveedores,
                                    PuedeGenerarSolicitudPago,
                                    PuedeVerReportes,
                                    PuedeAdministrar,
                                    FechaCreacion,
                                    PuedeAprobarPresupuesto,
                                    PuedeCrear,
                                    PuedeDescargar,
                                    PuedeEditar,
                                    PuedeEliminar,
                                    PuedeTodo
                                )
                                VALUES
                                (
                                    {usuario.Id},
                                    1, 1, 1, 1, 1, 1, 1, 1, 1,
                                    {perfil.EsCentral},
                                    GETDATE(),
                                    0, 1, 1, 1, 1, 0
                                );");

                        Console.WriteLine(
                            $"[QA ADQ] Preparado: {perfil.Email}");

                        Console.WriteLine(
                            $"[QA ADQ] Empleado: {empleado.Id}");

                        Console.WriteLine(
                            $"[QA ADQ] Rol: {perfil.Rol}");
                    }

                    await transaction.CommitAsync();

                    Console.WriteLine(
                        "[QA ADQ] Inicialización completada.");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    context.ChangeTracker.Clear();

                    throw;
                }
            });
        }

        private static void ValidarResultado(
            IdentityResult resultado,
            string operacion)
        {
            if (resultado.Succeeded)
                return;

            var errores = string.Join(
                " | ",
                resultado.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"{operacion}: {errores}");
        }
    }
}
