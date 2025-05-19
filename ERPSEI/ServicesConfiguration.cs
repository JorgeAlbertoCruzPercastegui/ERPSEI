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

namespace ERPSEI
{
    public static class ServicesConfiguration
    {
        public const string RolMaster = "Master";
        public const string RolAdministrador = "Administrador";
        public const string RolUsuario = "Usuario";
        public const string RolCandidato = "Candidato";

        private static readonly List<AppRole> Roles = [];

        public static string MasterPassword { get; set; } = string.Empty;
        public static AppUser MasterUser { get; } = new AppUser() { EmailConfirmed = true, IsPreregisterAuthorized = true, PasswordResetNeeded = false, IsMaster = true };

        public static void ConfigureEmail(WebApplicationBuilder _builder)
        {
            //Obtiene la configuración del enviador de correos.
            IConfigurationSection emailSection = _builder.Configuration.GetSection("Email");
            string address = (string)(emailSection.GetValue(typeof(string), "address") ?? throw new InvalidOperationException("Email 'address' not found."));
            string password = (string)(emailSection.GetValue(typeof(string), "password") ?? throw new InvalidOperationException("Email 'password' not found."));
            string smtp = (string)(emailSection.GetValue(typeof(string), "smtp") ?? throw new InvalidOperationException("Email 'smtp' not found."));
            int port = (int)(emailSection.GetValue(typeof(int), "port") ?? throw new InvalidOperationException("Email 'port' not found."));

            MasterUser.Email = address;
            MasterUser.UserName = address;

            _builder.Services.AddTransient<IEmailSender, EmailSender>(x =>
                new EmailSender(address, password, smtp, port)
            );
        }

        public static void ConfigureDatabase(WebApplicationBuilder _builder)
        {
            var connectionString = _builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            if (_builder.Environment.IsDevelopment())
            {
                _builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            }
		}

        public static void ConfigureDependencyInjection(WebApplicationBuilder _builder)
        {
            ConfigureDIUtils(_builder);

			ConfigureDIFacturacion(_builder);

			ConfigureDIEmpresas(_builder);

			ConfigureDIEmpleados(_builder);

			ConfigureDIAsistencias(_builder);

            ConfigureDIConciliaciones(_builder);

            ConfigureDICuentasContables(_builder);

            ConfigureDIPolizas(_builder);

            ConfigureDIActivosFijos(_builder);
        }
        private static void ConfigureDIAsistencias(WebApplicationBuilder _builder) 
        {
			//Asistencias
			_builder.Services.AddScoped<IAsistenciaManager, AsistenciaManager>();
			_builder.Services.AddScoped<IHorariosManager, HorariosManager>();
		}
        private static void ConfigureDIConciliaciones(WebApplicationBuilder _builder)
        {
            // Conciliaciones
            _builder.Services.AddScoped<IBancoManager, BancoManager>();
            //_builder.Services.AddScoped<IRCatalogoManager<Banco>, BancoManager>(); 
            _builder.Services.AddScoped<IConciliacionManager, ConciliacionManager>();
            _builder.Services.AddScoped<IConciliacionDetalleManager, ConciliacionDetalleManager>();
            _builder.Services.AddScoped<IConciliacionDetalleComprobanteManager, ConciliacionDetalleComprobanteManager>();
            _builder.Services.AddScoped<IConciliacionDetalleMovimientoManager, ConciliacionDetalleMovimientoManager>();
            _builder.Services.AddScoped<IClienteManager, ClienteManager>();
            _builder.Services.AddScoped<IMovimientoBancarioManager, MovimientoBancarioManager>();
        }
		private static void ConfigureDIPolizas(WebApplicationBuilder _builder)
		{
			//Polizas
			_builder.Services.AddScoped<IGruposPolizasManager, GruposPolizasManager>();
			_builder.Services.AddScoped<IPolizasManager, PolizasManager>();
			_builder.Services.AddScoped<IPolizasDetalles, PolizasDetallesManager>();
			_builder.Services.AddScoped<IPolizasTipos, PolizasTiposManager>();
		}
        private static void ConfigureDIActivosFijos(WebApplicationBuilder _builder)
        {
            //Activos Fijos
            _builder.Services.AddScoped<IActivoFijoManager, ActivoFijoManager>();
            _builder.Services.AddScoped<ITipoActivosFijosManager, TipoActivosFijosManager>();
            _builder.Services.AddScoped<ICategoriaActivosFijosManager, CategoriaActivosFijosManager>();
        }
        private static void ConfigureDICuentasContables(WebApplicationBuilder _builder)
		{
			//Cuentas Contables
			_builder.Services.AddScoped<ICuentaContableManager, CuentaContableManager>();
			_builder.Services.AddScoped<ICuentaContableTipoManager, CuentaContableTipoManager>();
			_builder.Services.AddScoped<ICuentaContableSubtipoManager, CuentaContableSubtipoManager>();
		}

		private static void ConfigureDIFacturacion(WebApplicationBuilder _builder)
        {
            //Comprobantes
            _builder.Services.AddScoped<IComprobanteManager, ComprobanteManager>();
            _builder.Services.AddScoped<IComprobanteEmisorManager, ComprobanteEmisorManager>();
			_builder.Services.AddScoped<IComprobanteReceptorManager, ComprobanteReceptorManager>();

			//Catálogos SAT
			_builder.Services.AddScoped<IAutorizacionesPrefactura, AutorizacionesPrefacturaManager>();
			_builder.Services.AddScoped<IExportacionManager, ExportacionManager>();
			_builder.Services.AddScoped<IFormaPagoManager, FormaPagoManager>();
			_builder.Services.AddScoped<IImpuestoManager, ImpuestoManager>();
			_builder.Services.AddScoped<IMesManager, MesManager>();
			_builder.Services.AddScoped<IMetodoPagoManager, MetodoPagoManager>();
			_builder.Services.AddScoped<IMonedaManager, MonedaManager>();
			_builder.Services.AddScoped<IObjetoImpuestoManager, ObjetoImpuestoManager>();
			_builder.Services.AddScoped<IPeriodicidadManager, PeriodicidadManager>();
			_builder.Services.AddScoped<IRegimenFiscalManager, RegimenFiscalManager>();
			_builder.Services.AddScoped<ITasaOCuotaManager, TasaOCuotaManager>();
			_builder.Services.AddScoped<ITipoComprobanteManager, TipoComprobanteManager>();
			_builder.Services.AddScoped<ITipoFactorManager, TipoFactorManager>();
			_builder.Services.AddScoped<ITipoRelacionManager, TipoRelacionManager>();
			_builder.Services.AddScoped<IUnidadMedidaManager, UnidadMedidaManager>();
			_builder.Services.AddScoped<IUsoCFDIManager, UsoCFDIManager>();
			_builder.Services.AddScoped<IProductoServicioManager, ProductoServicioManager>();
			_builder.Services.AddScoped<IRWCatalogoManager<ActividadEconomica>, ActividadEconomicaManager>();

            //Prefacturas
			_builder.Services.AddScoped<IConceptoManager, ConceptoManager>();
			_builder.Services.AddScoped<IPrefacturaManager, PrefacturaManager>();

            //Web Service EDICOM
            _builder.Services.AddSingleton<ServicioEDICOM.CFDi, ServicioEDICOM.CFDiClient>();
		}
        private static void ConfigureDIEmpresas(WebApplicationBuilder _builder)
        {
            //Gestión de Empresas
			_builder.Services.AddScoped<IBancoEmpresaManager, BancoEmpresaManager>();
			_builder.Services.AddScoped<IArchivoEmpresaManager, ArchivoEmpresaManager>();
			_builder.Services.AddScoped<IEmpresaManager, EmpresaManager>();
			_builder.Services.AddScoped<IProductoServicioPerfilManager, ProductoServicioPerfilManager>();
			_builder.Services.AddScoped<IPerfilManager, PerfilManager>();
			_builder.Services.AddScoped<IRWCatalogoManager<Origen>, OrigenManager>();
			_builder.Services.AddScoped<IRWCatalogoManager<Nivel>, NivelManager>();
			_builder.Services.AddScoped<IActividadEconomicaEmpresaManager, ActividadEconomicaEmpresaManager>();
		}
        private static void ConfigureDIEmpleados(WebApplicationBuilder _builder) 
        {
            //Accesos módulos
			_builder.Services.AddScoped<IAccesoModuloManager, AccesoModuloManager>();
			_builder.Services.AddScoped<AppRoleManager, AppRoleManager>();
			_builder.Services.AddScoped<IModuloManager, ModuloManager>();

            //Gestión de Talento
			_builder.Services.AddScoped<IArchivoEmpleadoManager, ArchivoEmpleadoManager>();
			_builder.Services.AddScoped<IContactoEmergenciaManager, ContactoEmergenciaManager>();
			_builder.Services.AddScoped<IEmpleadoManager, EmpleadoManager>();
			_builder.Services.AddScoped<IRWCatalogoManager<Puesto>, PuestoManager>();
			_builder.Services.AddScoped<IRWCatalogoManager<Area>, AreaManager>();
			_builder.Services.AddScoped<IRWCatalogoManager<Oficina>, OficinaManager>();
			_builder.Services.AddScoped<IRWCatalogoManager<Subarea>, SubareaManager>();
			_builder.Services.AddScoped<IRCatalogoManager<Genero>, GeneroManager>();
			_builder.Services.AddScoped<IRCatalogoManager<EstadoCivil>, EstadoCivilManager>();

            
		}    
        private static void ConfigureDIUtils(WebApplicationBuilder _builder)
        {
            _builder.Services.AddSingleton<IEncriptacionAES, EncriptacionAES>();
        }

		public static void ConfigureIdentity(WebApplicationBuilder _builder)
        {
            _builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<AppRole>()
            .AddRoleManager<AppRoleManager>()
            .AddUserManager<AppUserManager>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddTokenProvider<UserAuthorizationTokenProvider<AppUser>>("UserAuthorization");
        }

        public static void ConfigureAuthorization(WebApplicationBuilder _builder) {
			_builder.Services.AddAuthorizationBuilder()
				.AddPolicy("AccessPolicy", policy => policy.Requirements.Add(new AccessRequirement()))
                .AddPolicy("EmpresasPolicy", policy => policy.Requirements.Add(new AccessRequirementEmpresas()));

            _builder.Services.AddScoped<IAuthorizationHandler, AccessHandler>();
			_builder.Services.AddScoped<IAuthorizationHandler, AccessHandlerEmpresas>();
		}

		public static void ConfigurePagesAndLocalization(WebApplicationBuilder _builder)
        {
            _builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            _builder.Services.AddRazorPages()
            .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
			.AddDataAnnotationsLocalization(options =>
			{
				options.DataAnnotationLocalizerProvider = (type, factory) =>
				{
					var assemblyName = new AssemblyName(typeof(ValidationsLocalization).GetTypeInfo().Assembly.FullName ?? "");
                    return factory.Create(nameof(ValidationsLocalization), assemblyName.Name ?? "");
                };
			});

            _builder.Services.AddSession();
            _builder.Services.AddMemoryCache();
            _builder.Services.AddMvc(options =>
            {
                var assemblyName = new AssemblyName(typeof(ModelBindingMessages).GetTypeInfo().Assembly.FullName ?? "");
                var F = _builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();

                if(F == null) { return; }

                var L = F.Create(nameof(ModelBindingMessages), assemblyName.Name ?? "");

                options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor((x) => L["MissingBindRequiredValueAccessor", x]);
                options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => L["MissingKeyOrValueAccessor"]);
                options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => L["MissingRequestBodyRequiredValueAccessor"]);
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => L["ValueMustNotBeNullAccessor", x]);

                options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor((x) => L["UnknownValueIsInvalidAccessor", x]);
                options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => L["NonPropertyUnknownValueIsInvalidAccessor"]);
                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => L["ValueIsInvalidAccessor", x]);

                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor((x) => L["ValueMustBeANumberAccessor", x]);
                options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => L["NonPropertyValueMustBeANumberAccessor"]);

                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => L["AttemptedValueIsInvalidAccessor", x, y]);
                options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor((x) => L["NonPropertyAttemptedValueIsInvalidAccessor", x]);
            });

			_builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("es-MX")
                };

                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es-MX");
                options.SupportedUICultures = supportedCultures;
                options.SupportedCultures = supportedCultures;
            });
        }

        /*public static void ConfigureFormOptions(WebApplicationBuilder _builder)
        {
            _builder.Services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 10000;
            });
        }*/
        public static void ConfigureFormOptions(WebApplicationBuilder _builder)
        {
            _builder.Services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 10000; 
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
        }

    }
}
