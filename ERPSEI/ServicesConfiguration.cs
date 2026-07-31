using ERPSEI.Authorization;
using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.Clientes;
using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Entities.SAT.Catalogos;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Data.Managers.ActivosFijos;
using ERPSEI.Data.Managers.Vacaciones;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.Reportes;
using ERPSEI.Data.Managers.Conciliaciones;
using ERPSEI.Data.Managers.SAT;
using ERPSEI.Data.Managers.SAT.Catalogos;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Email;
using ERPSEI.Resources;
using ERPSEI.TokenProviders;
using ERPSEI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;
using ERPSEI.Data.Managers.SAT.cfdiv40;
using ERPSEI.Data.Managers.Clientes;
using ERPSEI.Data.Managers.Cuentas;
using ERPSEI.Data.Managers.AdministradorPolizas;
using ERPSEI.Data.Managers.Polizas;
using Microsoft.AspNetCore.Http.Features;
using ERPSEI.Data.Managers.TipoContratos;
using ERPSEI.Data.Managers.Documentos;
using ERPSEI.Data.Entities.Documentos;
using ERPSEI.Data.Managers.Intranet;
using Azure.Identity;
using Microsoft.Graph;

namespace ERPSEI
{
    public static class ServicesConfiguration
    {
        public const string RolMaster = "Master";
        public const string RolAdministrador = "Administrador";
        public const string RolUsuario = "Usuario";
        public const string RolCandidato = "Candidato";

        private static readonly List<AppRole> Roles = [];

        public static string MasterPassword
        {
            get;
            set;
        } = string.Empty;

        public static AppUser MasterUser
        {
            get;
        } = new AppUser
        {
            EmailConfirmed = true,
            IsPreregisterAuthorized = true,
            PasswordResetNeeded = false,
            IsMaster = true
        };

        public static void ConfigureEmail(
            WebApplicationBuilder builder)
        {
            var graphSection =
                builder.Configuration
                    .GetSection("Graph");

            var tenantId =
                graphSection.GetValue<string>(
                    "TenantId"
                )
                ?? throw new InvalidOperationException(
                    "Graph:TenantId not found."
                );

            var clientId =
                graphSection.GetValue<string>(
                    "ClientId"
                )
                ?? throw new InvalidOperationException(
                    "Graph:ClientId not found."
                );

            var clientSecret =
                graphSection.GetValue<string>(
                    "ClientSecret"
                )
                ?? throw new InvalidOperationException(
                    "Graph:ClientSecret not found."
                );

            var fromEmail =
                graphSection.GetValue<string>(
                    "FromEmail"
                )
                ?? throw new InvalidOperationException(
                    "Graph:FromEmail not found."
                );

            var credential =
                new ClientSecretCredential(
                    tenantId,
                    clientId,
                    clientSecret
                );

            builder.Services.AddSingleton(
                credential
            );

            builder.Services
                .AddSingleton<GraphServiceClient>(
                    serviceProvider =>
                    {
                        var cred =
                            serviceProvider
                                .GetRequiredService<
                                    ClientSecretCredential>();

                        return new GraphServiceClient(
                            cred
                        );
                    }
                );

            builder.Services
                .AddTransient<
                    IEmailSender,
                    EmailSender>();

            MasterUser.Email =
                fromEmail;

            MasterUser.UserName =
                fromEmail;
        }

        public static void ConfigureDatabase(
            WebApplicationBuilder builder)
        {
            var connectionString =
                builder.Configuration
                    .GetConnectionString(
                        "DefaultConnection"
                    )
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found."
                );

            builder.Services
                .AddDbContext<ApplicationDbContext>(
                    options =>
                        options.UseSqlServer(
                            connectionString
                        )
                );

            if (builder.Environment.IsDevelopment())
            {
                builder.Services
                    .AddDatabaseDeveloperPageExceptionFilter();
            }
        }

        public static void ConfigureDependencyInjection(
            WebApplicationBuilder builder)
        {
            ConfigureDIUtils(
                builder
            );

            ConfigureDIFacturacion(
                builder
            );

            ConfigureDIEmpresas(
                builder
            );

            ConfigureDIEmpleados(
                builder
            );

            ConfigureDIAsistencias(
                builder
            );

            ConfigureDIConciliaciones(
                builder
            );

            ConfigureDICuentasContables(
                builder
            );

            ConfigureDIPolizas(
                builder
            );

            ConfigureDIActivosFijos(
                builder
            );

            ConfigureDIVacaciones(
                builder
            );

            ConfigureDITipoContratos(
                builder
            );

            ConfigureDIDocumentos(
                builder
            );

            ConfigureDIComunicadosInteros(
                builder
            );

            ConfigureDIEventos(
                builder
            );

            ConfigureDINotificacionesEventosComunicados(
                builder
            );
        }

        private static void
            ConfigureDINotificacionesEventosComunicados(
                WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IIntranetNotificationService,
                    IntranetNotificationService>();
        }

        private static void ConfigureDIEventos(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IEventoIntranetManager,
                    EventoIntranetManager>();
        }

        private static void
            ConfigureDIComunicadosInteros(
                WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IComunicadoInternoManager,
                    ComunicadoInternoManager>();
        }

        private static void ConfigureDITipoContratos(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    ITipoContratosManager,
                    TipoContratosManager>();

            builder.Services
                .AddScoped<
                    IEmpresaContratosManager,
                    EmpresaContratosManager>();

            builder.Services
                .AddScoped<
                    IClienteContratosManager,
                    ClienteContratosManager>();
        }

        private static void ConfigureDIAsistencias(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IAsistenciaManager,
                    AsistenciaManager>();

            builder.Services
                .AddScoped<
                    IHorariosManager,
                    HorariosManager>();
        }

        private static void ConfigureDIConciliaciones(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IBancoManager,
                    BancoManager>();

            builder.Services
                .AddScoped<
                    IConciliacionManager,
                    ConciliacionManager>();

            builder.Services
                .AddScoped<
                    IConciliacionDetalleManager,
                    ConciliacionDetalleManager>();

            builder.Services
                .AddScoped<
                    IConciliacionDetalleComprobanteManager,
                    ConciliacionDetalleComprobanteManager>();

            builder.Services
                .AddScoped<
                    IConciliacionDetalleMovimientoManager,
                    ConciliacionDetalleMovimientoManager>();

            builder.Services
                .AddScoped<
                    IClienteManager,
                    ClienteManager>();

            builder.Services
                .AddScoped<
                    IMovimientoBancarioManager,
                    MovimientoBancarioManager>();
        }

        private static void ConfigureDIPolizas(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IGruposPolizasManager,
                    GruposPolizasManager>();

            builder.Services
                .AddScoped<
                    IPolizasManager,
                    PolizasManager>();

            builder.Services
                .AddScoped<
                    IPolizasDetalles,
                    PolizasDetallesManager>();

            builder.Services
                .AddScoped<
                    IPolizasTipos,
                    PolizasTiposManager>();
        }

        private static void ConfigureDIActivosFijos(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IActivoFijoManager,
                    ActivoFijoManager>();

            builder.Services
                .AddScoped<
                    ITipoActivosFijosManager,
                    TipoActivosFijosManager>();

            builder.Services
                .AddScoped<
                    ICategoriaActivosFijosManager,
                    CategoriaActivosFijosManager>();

            builder.Services
                .AddScoped<
                    IOficinaManager,
                    OficinaManager>();
        }

        private static void ConfigureDIVacaciones(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    ISolicitudVacacionesManager,
                    SolicitudVacacionesManager>();

            builder.Services
                .AddScoped<
                    IPoliticaVacacionManager,
                    PoliticaVacacionManager>();
        }

        private static void ConfigureDIDocumentos(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IDocumentoManager,
                    DocumentoManager>();

            builder.Services
                .AddScoped<
                    IEstatusDocumentoManager,
                    EstatusDocumentoManager>();

            builder.Services
                .AddScoped<
                    ITipoDocumentoManager,
                    TipoDocumentoManager>();
        }

        private static void ConfigureDICuentasContables(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    ICuentaContableManager,
                    CuentaContableManager>();

            builder.Services
                .AddScoped<
                    ICuentaContableTipoManager,
                    CuentaContableTipoManager>();

            builder.Services
                .AddScoped<
                    ICuentaContableSubtipoManager,
                    CuentaContableSubtipoManager>();
        }

        private static void ConfigureDIFacturacion(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IComprobanteManager,
                    ComprobanteManager>();

            builder.Services
                .AddScoped<
                    IComprobanteEmisorManager,
                    ComprobanteEmisorManager>();

            builder.Services
                .AddScoped<
                    IComprobanteReceptorManager,
                    ComprobanteReceptorManager>();

            builder.Services
                .AddScoped<
                    IAutorizacionesPrefactura,
                    AutorizacionesPrefacturaManager>();

            builder.Services
                .AddScoped<
                    IExportacionManager,
                    ExportacionManager>();

            builder.Services
                .AddScoped<
                    IFormaPagoManager,
                    FormaPagoManager>();

            builder.Services
                .AddScoped<
                    IImpuestoManager,
                    ImpuestoManager>();

            builder.Services
                .AddScoped<
                    IMesManager,
                    MesManager>();

            builder.Services
                .AddScoped<
                    IMetodoPagoManager,
                    MetodoPagoManager>();

            builder.Services
                .AddScoped<
                    IMonedaManager,
                    MonedaManager>();

            builder.Services
                .AddScoped<
                    IObjetoImpuestoManager,
                    ObjetoImpuestoManager>();

            builder.Services
                .AddScoped<
                    IPeriodicidadManager,
                    PeriodicidadManager>();

            builder.Services
                .AddScoped<
                    IRegimenFiscalManager,
                    RegimenFiscalManager>();

            builder.Services
                .AddScoped<
                    ITasaOCuotaManager,
                    TasaOCuotaManager>();

            builder.Services
                .AddScoped<
                    ITipoComprobanteManager,
                    TipoComprobanteManager>();

            builder.Services
                .AddScoped<
                    ITipoFactorManager,
                    TipoFactorManager>();

            builder.Services
                .AddScoped<
                    ITipoRelacionManager,
                    TipoRelacionManager>();

            builder.Services
                .AddScoped<
                    IUnidadMedidaManager,
                    UnidadMedidaManager>();

            builder.Services
                .AddScoped<
                    IUsoCFDIManager,
                    UsoCFDIManager>();

            builder.Services
                .AddScoped<
                    IProductoServicioManager,
                    ProductoServicioManager>();

            builder.Services
                .AddScoped<
                    IRWCatalogoManager<
                        ActividadEconomica>,
                    ActividadEconomicaManager>();

            builder.Services
                .AddScoped<
                    IConceptoManager,
                    ConceptoManager>();

            builder.Services
                .AddScoped<
                    IPrefacturaManager,
                    PrefacturaManager>();

            builder.Services
                .AddSingleton<
                    ServicioEDICOM.CFDi,
                    ServicioEDICOM.CFDiClient>();
        }

        private static void ConfigureDIEmpresas(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IBancoEmpresaManager,
                    BancoEmpresaManager>();

            builder.Services
                .AddScoped<
                    IArchivoEmpresaManager,
                    ArchivoEmpresaManager>();

            builder.Services
                .AddScoped<
                    IEmpresaManager,
                    EmpresaManager>();

            builder.Services
                .AddScoped<
                    IProductoServicioPerfilManager,
                    ProductoServicioPerfilManager>();

            builder.Services
                .AddScoped<
                    IPerfilManager,
                    PerfilManager>();

            builder.Services
                .AddScoped<
                    IRWCatalogoManager<Origen>,
                    OrigenManager>();

            builder.Services
                .AddScoped<
                    IRWCatalogoManager<Nivel>,
                    NivelManager>();

            builder.Services
                .AddScoped<
                    IActividadEconomicaEmpresaManager,
                    ActividadEconomicaEmpresaManager>();
        }

        private static void ConfigureDIEmpleados(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddScoped<
                    IAccesoModuloManager,
                    AccesoModuloManager>();

            builder.Services
                .AddScoped<
                    AppRoleManager,
                    AppRoleManager>();

            builder.Services
                .AddScoped<
                    IModuloManager,
                    ModuloManager>();

            builder.Services
                .AddScoped<
                    IArchivoEmpleadoManager,
                    ArchivoEmpleadoManager>();

            builder.Services
                .AddScoped<
                    IContactoEmergenciaManager,
                    ContactoEmergenciaManager>();

            builder.Services
                .AddScoped<
                    IEmpleadoManager,
                    EmpleadoManager>();

            builder.Services
                .AddScoped<
                    IRWCatalogoManager<Puesto>,
                    PuestoManager>();

            builder.Services
                .AddScoped<
                    IRWCatalogoManager<Area>,
                    AreaManager>();

            builder.Services
                .AddScoped<
                    IRWCatalogoManager<Oficina>,
                    OficinaManager>();

            builder.Services
                .AddScoped<
                    IRWCatalogoManager<Subarea>,
                    SubareaManager>();

            builder.Services
                .AddScoped<
                    IRCatalogoManager<Genero>,
                    GeneroManager>();

            builder.Services
                .AddScoped<
                    IRCatalogoManager<EstadoCivil>,
                    EstadoCivilManager>();
        }

        private static void ConfigureDIUtils(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddSingleton<
                    IEncriptacionAES,
                    EncriptacionAES>();
        }

        public static void ConfigureIdentity(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddDefaultIdentity<AppUser>(
                    options =>
                        options.SignIn
                            .RequireConfirmedAccount =
                            true
                )
                .AddRoles<AppRole>()
                .AddRoleManager<AppRoleManager>()
                .AddUserManager<AppUserManager>()
                .AddEntityFrameworkStores<
                    ApplicationDbContext>()
                .AddTokenProvider<
                    UserAuthorizationTokenProvider<
                        AppUser>>(
                    "UserAuthorization"
                );

            builder.Services
                .ConfigureApplicationCookie(
                    options =>
                    {
                        options.ExpireTimeSpan =
                            TimeSpan.FromMinutes(
                                10
                            );

                        options.SlidingExpiration =
                            true;

                        options.LoginPath =
                            "/Identity/Account/Login";

                        options.LogoutPath =
                            "/Identity/Account/Logout";

                        options.AccessDeniedPath =
                            "/Identity/Account/AccessDenied";
                    }
                );
        }

        public static void ConfigureAuthorization(
            WebApplicationBuilder builder)
        {
            builder.Services
                .AddAuthorizationBuilder()
                .AddPolicy(
                    "AccessPolicy",
                    policy =>
                        policy.Requirements.Add(
                            new AccessRequirement()
                        )
                )
                .AddPolicy(
                    "EmpresasPolicy",
                    policy =>
                        policy.Requirements.Add(
                            new AccessRequirementEmpresas()
                        )
                );

            builder.Services
                .AddScoped<
                    IAuthorizationHandler,
                    AccessHandler>();

            builder.Services
                .AddScoped<
                    IAuthorizationHandler,
                    AccessHandlerEmpresas>();
        }

        public static void
            ConfigurePagesAndLocalization(
                WebApplicationBuilder builder)
        {
            builder.Services.AddLocalization(
                options =>
                    options.ResourcesPath =
                        "Resources"
            );

            builder.Services
                .AddRazorPages()
                .AddViewLocalization(
                    Microsoft.AspNetCore.Mvc.Razor
                        .LanguageViewLocationExpanderFormat
                        .Suffix
                )
                .AddDataAnnotationsLocalization(
                    options =>
                    {
                        options
                            .DataAnnotationLocalizerProvider =
                            (type, factory) =>
                            {
                                var assemblyName =
                                    new AssemblyName(
                                        typeof(
                                            ValidationsLocalization
                                        )
                                        .GetTypeInfo()
                                        .Assembly
                                        .FullName ??
                                        string.Empty
                                    );

                                return factory.Create(
                                    nameof(
                                        ValidationsLocalization
                                    ),
                                    assemblyName.Name ??
                                    string.Empty
                                );
                            };
                    }
                );

            builder.Services.AddSession(
                options =>
                {
                    options.IdleTimeout =
                        TimeSpan.FromMinutes(
                            10
                        );

                    options.Cookie.HttpOnly =
                        true;

                    options.Cookie.IsEssential =
                        true;
                }
            );

            builder.Services.AddMemoryCache();

            builder.Services.AddMvc(
                options =>
                {
                    var assemblyName =
                        new AssemblyName(
                            typeof(
                                ModelBindingMessages
                            )
                            .GetTypeInfo()
                            .Assembly
                            .FullName ??
                            string.Empty
                        );

                    var factory =
                        builder.Services
                            .BuildServiceProvider()
                            .GetService<
                                IStringLocalizerFactory>();

                    if (factory == null)
                    {
                        return;
                    }

                    var localizer =
                        factory.Create(
                            nameof(
                                ModelBindingMessages
                            ),
                            assemblyName.Name ??
                            string.Empty
                        );

                    options.ModelBindingMessageProvider
                        .SetMissingBindRequiredValueAccessor(
                            campo =>
                                localizer[
                                    "MissingBindRequiredValueAccessor",
                                    campo
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetMissingKeyOrValueAccessor(
                            () =>
                                localizer[
                                    "MissingKeyOrValueAccessor"
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetMissingRequestBodyRequiredValueAccessor(
                            () =>
                                localizer[
                                    "MissingRequestBodyRequiredValueAccessor"
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetValueMustNotBeNullAccessor(
                            campo =>
                                localizer[
                                    "ValueMustNotBeNullAccessor",
                                    campo
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetUnknownValueIsInvalidAccessor(
                            campo =>
                                localizer[
                                    "UnknownValueIsInvalidAccessor",
                                    campo
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetNonPropertyUnknownValueIsInvalidAccessor(
                            () =>
                                localizer[
                                    "NonPropertyUnknownValueIsInvalidAccessor"
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetValueIsInvalidAccessor(
                            valor =>
                                localizer[
                                    "ValueIsInvalidAccessor",
                                    valor
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetValueMustBeANumberAccessor(
                            campo =>
                                localizer[
                                    "ValueMustBeANumberAccessor",
                                    campo
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetNonPropertyValueMustBeANumberAccessor(
                            () =>
                                localizer[
                                    "NonPropertyValueMustBeANumberAccessor"
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetAttemptedValueIsInvalidAccessor(
                            (valor, campo) =>
                                localizer[
                                    "AttemptedValueIsInvalidAccessor",
                                    valor,
                                    campo
                                ]
                        );

                    options.ModelBindingMessageProvider
                        .SetNonPropertyAttemptedValueIsInvalidAccessor(
                            valor =>
                                localizer[
                                    "NonPropertyAttemptedValueIsInvalidAccessor",
                                    valor
                                ]
                        );
                }
            );

            builder.Services.Configure<
                RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures =
                        new[]
                        {
                            new CultureInfo(
                                "en-US"
                            ),

                            new CultureInfo(
                                "es-MX"
                            )
                        };

                    options.DefaultRequestCulture =
                        new Microsoft.AspNetCore
                            .Localization
                            .RequestCulture(
                                "es-MX"
                            );

                    options.SupportedUICultures =
                        supportedCultures;

                    options.SupportedCultures =
                        supportedCultures;
                }
            );
        }

        // =====================================================
        // CONFIGURACIÓN DE FORMULARIOS Y ARCHIVOS
        // =====================================================
        public static void ConfigureFormOptions(
            WebApplicationBuilder builder)
        {
            /*
             * El archivo individual del módulo Compliance
             * estará limitado a 100 MB desde el handler.
             *
             * FormOptions permite 110 MB para considerar
             * encabezados, campos y estructura multipart.
             */
            const long limiteSolicitudMultipart =
                110L * 1024L * 1024L;

            builder.Services.Configure<FormOptions>(
                options =>
                {
                    options.ValueCountLimit =
                        10000;

                    options.ValueLengthLimit =
                        int.MaxValue;

                    options.MultipartBodyLengthLimit =
                        limiteSolicitudMultipart;

                    /*
                     * No debe establecerse en int.MaxValue.
                     * Este valor controla los encabezados de
                     * cada sección multipart, no el archivo.
                     */
                    options.MultipartHeadersLengthLimit =
                        64 * 1024;
                }
            );
        }
    }
}