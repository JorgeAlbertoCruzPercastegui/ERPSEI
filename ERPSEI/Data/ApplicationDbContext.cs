using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Entities.Clientes;
using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.Cuentas;
using ERPSEI.Data.Entities.Documentos;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Metricas;
using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Entities.Reportes;
using ERPSEI.Data.Entities.RH;
using ERPSEI.Data.Entities.SAT;
using ERPSEI.Data.Entities.SAT.Catalogos;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Entities.SAT.Nomina12;
using ERPSEI.Data.Entities.SAT.Pagos20;
using ERPSEI.Data.Entities.SAT.TimbreFiscalDigital11;
using ERPSEI.Data.Entities.TipoContratos;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Entities.Vacaciones;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;
using ERPSEI.Data.Entities.ServiceDesk;
using ERPSEI.Data.Entities.ExpedientesBancarios;
using ERPSEI.Data.Entities.Adquisiciones;

namespace ERPSEI.Data
{
	public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
	{
        private readonly IHttpContextAccessor? _httpContextAccessor;
        //Tablas de trabajo Empleados
        public DbSet<ArchivoEmpleado> ArchivosEmpleado { get; set; }
		public DbSet<Empleado> Empleados { get; set; }
		public DbSet<ContactoEmergencia> ContactosEmergencia { get; set; }

		//Catálogos Administrables Empleados
		public DbSet<Puesto> Puestos { get; set; }
		public DbSet<Area> Areas { get; set; }
		public DbSet<Oficina> Oficinas { get; set; }
		public DbSet<Subarea> Subareas { get; set; }

		//Catálogos no Administrables Empleados
		public DbSet<EstadoCivil> EstadosCiviles { get; set; }
		public DbSet<Genero> Generos { get; set; }


		//Tablas de trabajo Empresas
		public DbSet<BancoEmpresa> BancosEmpresa { get; set; }
		public DbSet<ArchivoEmpresa> ArchivosEmpresa { get; set; }
		public DbSet<Empresa> Empresas { get; set; }
		public DbSet<ActividadEconomicaEmpresa> ActividadesEconomicasEmpresa { get; set; }
		public DbSet<ProductoServicioPerfil> ProductosServiciosPerfil { get; set; }

        // Expedientes Bancarios
        public DbSet<EbEmpresa> EbEmpresas
        {
            get;
            set;
        } = null!;

        public DbSet<EbAccionista> EbAccionistas
        {
            get;
            set;
        } = null!;

        public DbSet<EbTipoDocumento> EbTiposDocumento
        {
            get;
            set;
        } = null!;

        public DbSet<EbDocumentoVinculoEmpresa>
            EbDocumentosVinculosEmpresa
        {
            get;
            set;
        } = null!;

        public DbSet<EbDocumento> EbDocumentos
        {
            get;
            set;
        } = null!;

        public DbSet<EbBitacoraDocumento> EbBitacoraDocumentos
        {
            get;
            set;
        } = null!;

        public DbSet<EbBitacoraEmpresa>
        EbBitacoraEmpresas
        {
            get;
            set;
        }

        public DbSet<EbPermisoComplianceUsuario>
            EbPermisosComplianceUsuarios
        {
            get;
            set;
        } = null!;

        public DbSet<EbAlcanceComplianceUsuario>
        EbAlcancesComplianceUsuarios
        {
            get;
            set;
        } = null!;

        public DbSet<EbPermisoComplianceEmpresaUsuario>
            EbPermisosComplianceEmpresasUsuario
        {
            get;
            set;
        } = null!;

        //Catálogos Administrables Empresas
        public DbSet<Origen> Origenes { get; set; }
		public DbSet<Nivel> Niveles { get; set; }
		public DbSet<Perfil> Perfiles { get; set; }
		public DbSet<ProductoServicio> ProductosServicios { get; set; }

		//Catálogos no Administrables Empresas
		public DbSet<ActividadEconomica> ActividadesEconomicas { get; set; }

		//Tablas de trabajo SAT
		public DbSet<AutorizacionesPrefactura> AutorizacionesPrefacturas { get; set; }
		public DbSet<Prefactura> Prefacturas { get; set; }
		public DbSet<Concepto> Conceptos { get; set; }
		public DbSet<ComprobanteAddenda> ComprobantesAddendas { get; set; }
		public DbSet<ComprobanteCfdiRelacionados> ComprobantesCfdisRelacionados { get; set; }
		public DbSet<ComprobanteComplemento> ComprobantesComplementos { get; set; }
		public DbSet<ComprobanteConcepto> ComprobantesConceptos { get; set; }
		public DbSet<ComprobanteConceptoACuentaTerceros> ComprobantesConceptosACuentaTerceros { get; set; }
		public DbSet<ComprobanteConceptoComplementoConcepto> ComprobantesConceptosComplementosConceptos { get; set; }
		public DbSet<ComprobanteConceptoCuentaPredial> ComprobantesConceptosCuentasPrediales { get; set; }
		public DbSet<ComprobanteConceptoImpuestos> ComprobantesConceptosImpuestos { get; set; }
		public DbSet<ComprobanteConceptoImpuestosRetencion> ComprobantesConceptosImpuestosRetenciones { get; set; }
		public DbSet<ComprobanteConceptoImpuestosTraslado> ComprobantesConceptosImpuestosTraslados { get; set; }
		public DbSet<ComprobanteConceptoInformacionAduanera> ComprobantesConceptosInformacionesAduaneras { get; set; }
		public DbSet<ComprobanteConceptoParte> ComprobantesConceptosPartes { get; set; }
		public DbSet<ComprobanteConceptoParteInformacionAduanera> ComprobantesConceptosPartesInformacionesAduaneras { get; set; }
		public DbSet<ComprobanteEmisor> ComprobantesEmisores { get; set; }
		public DbSet<ComprobanteImpuestos> ComprobantesImpuestos { get; set; }
		public DbSet<ComprobanteImpuestosRetencion> ComprobantesImpuestosRetenciones { get; set; }
		public DbSet<ComprobanteImpuestosTraslado> ComprobantesImpuestosTraslados { get; set; }
		public DbSet<ComprobanteInformacionGlobal> ComprobantesInformacionesGlobales { get; set; }
		public DbSet<ComprobanteReceptor> ComprobantesReceptores { get; set; }

		public DbSet<Comprobante> Comprobantes { get; set; }

		//Complemento de Nómina
		public DbSet<Nomina> Nominas { get; set; }
		public DbSet<NominaDeducciones> NominasDeducciones { get; set; }
		public DbSet<NominaDeduccionesDeduccion> NominasDeduccionesDeducciones { get; set; }
		public DbSet<NominaEmisor> NominasEmisores { get; set; }
		public DbSet<NominaEmisorEntidadSNCF> NominasEmisoresEntidadesSNCF { get; set; }
		public DbSet<NominaIncapacidad> NominasIncapacidades { get; set; }
		public DbSet<NominaOtroPago> NominasOtrosPagos { get; set; }
		public DbSet<NominaOtroPagoCompensacionSaldosAFavor> NominasOtrosPagosCompensacionesSaldosAFavor { get; set; }
		public DbSet<NominaOtroPagoSubsidioAlEmpleo> NominasOtrosPagosSubsidiosAlEmpleo { get; set; }
		public DbSet<NominaPercepciones> NominasPercepciones { get; set; }
		public DbSet<NominaPercepcionesJubilacionPensionRetiro> NominasPercepcionesJubilacionesPensionesRetiros { get; set; }
		public DbSet<NominaPercepcionesPercepcion> NominasPercepcionesPercepciones { get; set; }
		public DbSet<NominaPercepcionesPercepcionAccionesOTitulos> NominasPercepcionesPercepcionesAccionesOTitulos { get; set; }
		public DbSet<NominaPercepcionesPercepcionHorasExtra> NominasPercepcionesPercepcionesHorasExtras { get; set; }
		public DbSet<NominaPercepcionesSeparacionIndemnizacion> NominasPercepcionesSeparacionesIndemnizaciones { get; set; }
		public DbSet<NominaReceptor> NominasReceptores { get; set; }
		public DbSet<NominaReceptorSubContratacion> NominasReceptoresSubContrataciones { get; set; }

		//Complemento de Pago
		public DbSet<Pagos> Pagos { get; set; }
		public DbSet<PagosPago> PagosPagos { get; set; }
		public DbSet<PagosPagoDoctoRelacionado> PagosPagosDoctosRelacionados { get; set; }
		public DbSet<PagosPagoDoctoRelacionadoImpuestosDR> PagosPagosDoctosRelacionadosImpuestosDR { get; set; }
		public DbSet<PagosPagoDoctoRelacionadoImpuestosDRRetencionDR> PagosPagosDoctosRelacionadosImpuestosDRRetencionesDR { get; set; }
		public DbSet<PagosPagoDoctoRelacionadoImpuestosDRTrasladoDR> PagosPagosDoctosRelacionadosImpuestosDRTrasladosDR { get; set; }
		public DbSet<PagosPagoImpuestosP> PagosPagosImpuestosP { get; set; }
		public DbSet<PagosPagoImpuestosPRetencionP> PagosPagosImpuestosPRetencionesP { get; set; }
		public DbSet<PagosPagoImpuestosPTrasladoP> PagosPagosImpuestosPTrasladosP { get; set; }
		public DbSet<PagosTotales> PagosTotales { get; set; }

		//Complemento de Timbre Fiscal Digital
		public DbSet<TimbreFiscalDigital> TimbresFiscalesDigitales { get; set; }

		//Catálogos no Administrables SAT
		public DbSet<Exportacion> Exportaciones { get; set; }
		public DbSet<FormaPago> FormasPago { get; set; }
		public DbSet<Impuesto> Impuestos { get; set; }
		public DbSet<Mes> Meses { get; set; }
		public DbSet<MetodoPago> MetodosPago { get; set; }
		public DbSet<Moneda> Monedas { get; set; }
		public DbSet<ObjetoImpuesto> ObjetosImpuesto { get; set; }
		public DbSet<Periodicidad> Periodicidades { get; set; }
		public DbSet<RegimenFiscal> RegimenesFiscales { get; set; }
		public DbSet<TasaOCuota> TasasOCuotas { get; set; }
		public DbSet<TipoComprobante> TiposComprobante { get; set; }
		public DbSet<TipoFactor> TiposFactor { get; set; }
		public DbSet<TipoRelacion> TiposRelacion { get; set; }
		public DbSet<UnidadMedida> UnidadesMedida { get; set; }
		public DbSet<UsoCFDI> UsosCFDI { get; set; }

		//Reporte asistencias
		public DbSet<Asistencia> Asistencias { get; set; }
		public DbSet<Horario> Horarios { get; set; }
		public DbSet<HorarioDetalle> HorariosDetalles { get; set; }

		//Conciliaciones
		public DbSet<Conciliacion> Conciliaciones { get; set; }
		public DbSet<ConciliacionDetalle> ConciliacionesDetalles { get; set; }
		public DbSet<Banco> Bancos { get; set; }
		public DbSet<MovimientoBancario> MovimientosBancarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ConciliacionDetalleComprobante> ConciliacionesDetallesComprobantes { get; set; }
        public DbSet<ConciliacionDetalleMovimiento> ConciliacionesDetallesMovimientos { get; set; }

		//Activos Fijos
		public DbSet<ActivoFijo> ActivosFijos {  get; set; }
		public DbSet<CategoriaActivoFijo> CategoriasActivosFijos { get; set; }
        public DbSet<TipoActivoFijo> TiposActivosFijos { get; set; }
        public DbSet<ArchivoActivoFijo> ArchivosActivosFijos { get; set; }

        //Tipo Contratos
        public DbSet<TipoContrato> TipoContratos { get; set; }
        public DbSet<SubTipoContrato> SubTiposContrato { get; set; }

        public DbSet<EmpresaContrato> EmpresaContratos { get; set; }
        public DbSet<ClienteContrato> ClienteContratos { get; set; }
        public DbSet<HistorialContratoGenerado> HistorialContratoGenerados { get; set; }
        public DbSet<TipoRepresentacion> TipoRepresentaciones { get; set; }

		//Políticas
		public DbSet<Documento> Documentos { get; set; }
		public DbSet<TipoDocumento> TiposDocumento { get; set; }
		public DbSet<DocumentoPalabraClave> DocumentosPalabraClave { get; set; }
		public DbSet<DocumentoVersion> DocumentosVersion { get; set; }
		public DbSet<EstatusDocumento> DocumentosEstatus { get; set; }

        public DbSet<DocumentoAutorizacion> DocumentosAutorizaciones { get; set; } = null!;


        //Vacaciones
        public DbSet<DiaFestivo> DiasFestivos { get; set; }
        public DbSet<HistorialVacaciones> HistorialesVacaciones { get; set; }
        public DbSet<PeriodoVacacional> PeriodosVacacionales { get; set; }
        public DbSet<SolicitudVacaciones> SolicitudesVacaciones { get; set; }
        public DbSet<PoliticaVacacion> PoliticasVacaciones { get; set; }
        public DbSet<PoliticaVacacionDetalle> PoliticasVacacionesDetalles { get; set; }
        public DbSet<ConfiguracionVacacion> ConfiguracionesVacaciones { get; set; }
        public DbSet<HistorialVacacionVencida> HistorialVacacionesVencidas { get; set; }

        //Ausencias
        public DbSet<TipoAusencia> TiposAusencias { get; set; }
        public DbSet<TipoIncapacidad> TiposIncapacidades { get; set; }
        public DbSet<Ausencia> Ausencias { get; set; }
        public DbSet<AusenciaDocumento> AusenciasDocumentos { get; set; }

        //Comunicados Internos
        public DbSet<ComunicadoInterno> ComunicadosInternos { get; set; }

        //Eventos
        public DbSet<EventoIntranet> EventosIntranet { get; set; }

        //Cuentas contables
        public DbSet<CuentaContable> CuentasContables { get; set; }
		public DbSet<CuentaContableTipo> CuentaContableTipos { get; set; }
		public DbSet<CuentaContableSubtipo> CuentaContableSubtipos { get; set; }
		public DbSet<CuentaContableProductoServicio> CuentaContableProductosServicios { get; set; }

		//Administrador de polizas
		public DbSet<GrupoPoliza> GruposPolizas { get; set; }
		public DbSet<VPoliza> VPolizas { get; set; }
		public DbSet<PolizaDetalle> PolizasDetalles { get; set; }
		public DbSet<PolizaTipo> PolizasTipos { get; set; }


		//Catálogos no administrables Usuarios
		public DbSet<AccesoModulo> AccesosModulos { get; set; }
		public DbSet<Modulo> Modulos { get; set; }

        //Intranet
        public DbSet<Banner> Banners { get; set; }

        //Header
        public DbSet<HeaderImagen> HeaderImagenes { get; set; }

        //Políticas y Manuales
        public DbSet<ManualPoliticaIntranet> ManualesPoliticasIntranet { get; set; }
        public DbSet<ManualPoliticaArea> ManualPoliticaAreas { get; set; }

        //Notificaciones Intranet
        public DbSet<NotificacionIntranet> NotificacionesIntranet { get; set; }
        public DbSet<NotificacionIntranetUsuario> NotificacionesIntranetUsuarios { get; set; }

        //Mesa de Servicio
        public DbSet<ServiceTicket> ServiceTickets { get; set; }
        public DbSet<ServiceTicketType> ServiceTicketTypes { get; set; }
        public DbSet<ServiceTicketStatus> ServiceTicketStatuses { get; set; }
        public DbSet<ServiceTicketPriority> ServiceTicketPriorities { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<ServiceSubcategory> ServiceSubcategories { get; set; }
        public DbSet<ServiceSupportTeam> ServiceSupportTeams { get; set; }
        public DbSet<ServiceSupportTeamUser> ServiceSupportTeamUsers { get; set; }
        public DbSet<ServiceTicketComment> ServiceTicketComments { get; set; }
        public DbSet<ServiceTicketAttachment> ServiceTicketAttachments { get; set; }
        public DbSet<ServiceTicketHistory> ServiceTicketHistories { get; set; }

        //Métricas
        public DbSet<IntranetActividad> IntranetActividades { get; set; }

        public DbSet<IntranetAuditoria> IntranetAuditorias { get; set; }

        //Correos y Dominios
        public DbSet<CorreoDominio> CorreosDominios { get; set; }

        private readonly AuditoriaContext? _auditoriaContext;

        //Adquisiciones
        public DbSet<AdqSolicitud> AdqSolicitudes { get; set; }

        public DbSet<AdqSolicitudDetalle> AdqSolicitudesDetalle { get; set; }

        public DbSet<AdqEstatus> AdqEstatus { get; set; }

        public DbSet<AdqAdjunto> AdqAdjuntos { get; set; }

        public DbSet<AdqHistorial> AdqHistorial { get; set; }

        public DbSet<AdqAprobacion> AdqAprobaciones { get; set; }

        public DbSet<AdqAsignacion> AdqAsignaciones { get; set; }

        public DbSet<AdqComentario> AdqComentarios { get; set; }

        public DbSet<AdqComentarioAdjunto> AdqComentariosAdjuntos { get; set; }

        public DbSet<AdqPermisoUsuario> AdqPermisosUsuarios { get; set; }

        //Adquisiciones Cotizaciones
        public DbSet<AdqCotizacion> AdqCotizaciones
        {
            get;
            set;
        }

        public DbSet<AdqCotizacionDetalle> AdqCotizacionDetalles
        {
            get;
            set;
        }

        public DbSet<AdqCotizacionAdjunto> AdqCotizacionAdjuntos
        {
            get;
            set;
        }

        public DbSet<AdqAprobacionPresupuestal> AdqAprobacionesPresupuestales { get; set; }

        /*public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }*/
        public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor,
        AuditoriaContext auditoriaContext)
        : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _auditoriaContext = auditoriaContext;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditorias = ObtenerAuditoriasPendientes();

            var resultado = await base.SaveChangesAsync(cancellationToken);

            if (auditorias.Any())
            {
                IntranetAuditorias.AddRange(auditorias);
                await base.SaveChangesAsync(cancellationToken);
            }

            return resultado;
        }

        public override int SaveChanges()
        {
            var auditorias = ObtenerAuditoriasPendientes();

            var resultado = base.SaveChanges();

            if (auditorias.Any())
            {
                IntranetAuditorias.AddRange(auditorias);
                base.SaveChanges();
            }

            return resultado;
        }

        private bool EsCambioDeRolUsuario(EntityEntry entry)
        {
            string nombreEntidad = entry.Entity.GetType().Name.ToLower();

            return nombreEntidad.Contains("identityuserrole") ||
                   nombreEntidad.Contains("appuserrole") ||
                   entry.Metadata.ClrType == typeof(Microsoft.AspNetCore.Identity.IdentityUserRole<string>);
        }

        private List<IntranetAuditoria> ObtenerAuditoriasPendientes()
        {
            if (_auditoriaContext == null || !_auditoriaContext.Activada)
                return new List<IntranetAuditoria>();

            var auditorias = new List<IntranetAuditoria>();

            var entradas = ChangeTracker.Entries()
            .Where(e =>
                e.Entity != null &&
                e.Entity is not IntranetAuditoria &&
                e.Entity is not IntranetActividad &&
                e.Entity is not ServiceTicketHistory &&
                !EsCambioDeRolUsuario(e) &&
                (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted
                ))
            .ToList();

            if (_auditoriaContext.Modulo == "Gestión de Talento")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "Empleado" &&
                        e.State == EntityState.Modified)
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Roles")
            {
                entradas = entradas
                    .Where(e => e.Entity.GetType().Name == "AppRole")
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Puestos")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "Puesto")
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Áreas")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "Area")
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Subáreas")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "Subarea")
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Oficinas")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "Oficina")
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Orígenes")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "Origen")
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Niveles")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "Nivel")
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Perfiles")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "Perfil")
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Activos Fijos")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "ActivoFijo")
                    .ToList();
            }

            if (_auditoriaContext.Modulo == "Empresas")
            {
                entradas = entradas
                    .Where(e =>
                        e.Entity.GetType().Name == "Empresa")
                    .ToList();
            }

            var httpContext = _httpContextAccessor?.HttpContext;

            string? usuarioEjecutorId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? usuarioEjecutor = httpContext?.User?.Identity?.Name;

            string? ip = httpContext?.Connection.RemoteIpAddress?.ToString();

            if (ip == "::1")
                ip = "127.0.0.1";

            string? userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

            foreach (var entry in entradas)
            {
                string entidad = entry.Entity.GetType().Name;

                string accion = !string.IsNullOrWhiteSpace(_auditoriaContext.Accion)
                    ? _auditoriaContext.Accion
                    : entry.State switch
                    {
                        EntityState.Added => "Alta",
                        EntityState.Modified => "Edición",
                        EntityState.Deleted => "Eliminación",
                        _ => "Cambio"
                    };

                string registroId =
                    ObtenerValorPropiedad(entry, "Id") ??
                    ObtenerValorPropiedad(entry, "ID") ??
                    ObtenerValorPropiedad(entry, "UserId") ??
                    "Sin Id";

                string registroNombre =
                    ObtenerValorPropiedad(entry, "NombreCompleto") ??
                    ObtenerValorPropiedad(entry, "Nombre") ??
                    ObtenerValorPropiedad(entry, "UserName") ??
                    ObtenerValorPropiedad(entry, "Email") ??
                    registroId;

                string modulo = !string.IsNullOrWhiteSpace(_auditoriaContext.Modulo)
                    ? _auditoriaContext.Modulo
                    : ObtenerModuloAuditoria(entidad);

                if (entry.State == EntityState.Added)
                {
                    auditorias.Add(CrearAuditoria(
                        usuarioEjecutorId,
                        usuarioEjecutor,
                        modulo,
                        accion,
                        entidad,
                        registroId,
                        registroNombre,
                        "Registro",
                        null,
                        "Registro creado",
                        ip,
                        userAgent
                    ));
                }

                if (entry.State == EntityState.Deleted)
                {
                    auditorias.Add(CrearAuditoria(
                        usuarioEjecutorId,
                        usuarioEjecutor,
                        modulo,
                        accion,
                        entidad,
                        registroId,
                        registroNombre,
                        "Registro",
                        "Registro existente",
                        "Registro eliminado",
                        ip,
                        userAgent
                    ));
                }

                if (entry.State == EntityState.Modified)
                {
                    foreach (var prop in entry.Properties)
                    {
                        if (!prop.IsModified)
                            continue;

                        string campo = prop.Metadata.Name;

                        if (CampoIgnoradoAuditoria(campo))
                            continue;

                        string? valorAnterior = prop.OriginalValue?.ToString();
                        string? valorNuevo = prop.CurrentValue?.ToString();

                        if (valorAnterior == valorNuevo)
                            continue;

                        auditorias.Add(CrearAuditoria(
                            usuarioEjecutorId,
                            usuarioEjecutor,
                            modulo,
                            accion,
                            entidad,
                            registroId,
                            registroNombre,
                            campo,
                            valorAnterior,
                            valorNuevo,
                            ip,
                            userAgent
                        ));
                    }
                }
            }

            return auditorias;
        }

        private IntranetAuditoria CrearAuditoria(
            string? usuarioEjecutorId,
            string? usuarioEjecutor,
            string modulo,
            string accion,
            string entidad,
            string registroId,
            string registroNombre,
            string? campo,
            string? valorAnterior,
            string? valorNuevo,
            string? ip,
            string? userAgent)
        {
            return new IntranetAuditoria
            {
                UsuarioEjecutorId = usuarioEjecutorId,
                UsuarioEjecutor = usuarioEjecutor,

                Modulo = modulo,
                Accion = accion,
                Entidad = entidad,
                RegistroId = registroId,
                RegistroNombre = registroNombre,

                CampoModificado = campo,
                ValorAnterior = valorAnterior,
                ValorNuevo = valorNuevo,

                FechaHora = DateTime.Now,
                Ip = ip,
                UserAgent = userAgent
            };
        }

        private string? ObtenerValorPropiedad(EntityEntry entry, string propiedad)
        {
            var prop = entry.Properties
                .FirstOrDefault(p => p.Metadata.Name.Equals(propiedad, StringComparison.OrdinalIgnoreCase));

            if (prop == null)
                return null;

            return prop.CurrentValue?.ToString() ?? prop.OriginalValue?.ToString();
        }

        private bool CampoIgnoradoAuditoria(string campo)
        {
            string[] camposIgnorados =
            {
        "PasswordHash",
        "SecurityStamp",
        "ConcurrencyStamp",
        "NormalizedUserName",
        "NormalizedEmail",
        "AccessFailedCount",
        "LockoutEnd",
        "TwoFactorEnabled",
        "EmailConfirmed",
        "PhoneNumberConfirmed"
    };

            return camposIgnorados.Any(x =>
                x.Equals(campo, StringComparison.OrdinalIgnoreCase));
        }

        private string ObtenerModuloAuditoria(string entidad)
        {
            entidad = entidad.ToLower();

            if (
                entidad.Contains(
                    "ebpermisocomplianceusuario"
                )
            )
            {
                return "Compliance / Permisos";
            }

            if (
                entidad.Contains("ebempresa") ||
                entidad.Contains("ebaccionista") ||
                entidad.Contains("ebdocumento") ||
                entidad.Contains("ebtipodocumento") ||
                entidad.Contains("ebbitacoradocumento")
            )
            {
                return "Compliance";
            }

            if (entidad.Contains("appuser") || entidad.Contains("identityuser"))
                return "Usuarios";

            if (entidad.Contains("approle") || entidad.Contains("identityrole"))
                return "Roles";

            if (entidad.Contains("identityuserrole") || entidad.Contains("appuserrole"))
                return "Usuarios / Roles";

            if (entidad.Contains("empleado") || entidad.Contains("archivoempleado") || entidad.Contains("contactoemergencia"))
                return "Gestión de Talento";

            if (entidad.Contains("area"))
                return "Áreas";

            if (entidad.Contains("subarea"))
                return "Subáreas";

            if (entidad.Contains("puesto"))
                return "Puestos";

            if (entidad.Contains("oficina"))
                return "Oficinas";

            if (entidad.Contains("solicitudvacaciones") ||
                entidad.Contains("periodovacacional") ||
                entidad.Contains("historialvacacion") ||
                entidad.Contains("configuracionvacacion") ||
                entidad.Contains("politicavacacion"))
                return "Vacaciones";

            if (entidad.Contains("ausencia") || entidad.Contains("incapacidad"))
                return "Ausencias";

            if (entidad.Contains("banner"))
                return "Banners";

            if (entidad.Contains("headerimagen"))
                return "Imágenes Header";

            if (entidad.Contains("manual") ||
                entidad.Contains("politica") ||
                entidad.Contains("documento"))
                return "Biblioteca Corporativa";

            if (entidad.Contains("comunicado"))
                return "Comunicados Internos";

            if (entidad.Contains("evento"))
                return "Eventos";

            if (entidad.Contains("empresa"))
                return "Empresas";

            if (entidad.Contains("activofijo"))
                return "Activos Fijos";

            if (entidad.Contains("conciliacion") || entidad.Contains("movimientobancario"))
                return "Conciliaciones";

            if (entidad.Contains("banco"))
                return "Bancos";

            if (entidad.Contains("cuentacontable"))
                return "Cuentas Contables";

            if (entidad.Contains("prefactura") || entidad.Contains("comprobante"))
                return "Facturación";

            if (entidad.Contains("serviceticket") ||
                entidad.Contains("servicecategory") ||
                entidad.Contains("servicesubcategory") ||
                entidad.Contains("servicesupportteam")
            )
            {
                return "Mesa de Ayuda";
            }

            return entidad;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

			//Empresas
			BuildEmpresas(modelBuilder);

			//Empleados
			BuildEmpleados(modelBuilder);

			//SAT
			BuildSAT(modelBuilder);

			//Accesos
			BuildAccesos(modelBuilder);

			//Asistencias
			BuildAsistencias(modelBuilder);

			//Conciliaciones
            BuildConciliaciones(modelBuilder);

			//Cuentas
			BuildCuentasContables(modelBuilder);

			//Polizas
			BuildPolizas(modelBuilder);

			//Activos Fijos
			BuildActivosFijos(modelBuilder);

            //Vacaciones
            BuildVacaciones(modelBuilder);

            //Ausencias
            BuildAusencias(modelBuilder);

            //Contratos
            BuildTipoContratos(modelBuilder);

			//Políticas
			BuildPoliticas(modelBuilder);

			//Intranet
            BuildBanners(modelBuilder);

			//Header
            BuildHeaderImagenes(modelBuilder);

            //Políticas y manuales
            BuildManualesPoliticasIntranet(modelBuilder);

            //Comunicados Internos
            BuildComunicadosInternos(modelBuilder);

            //Eventos
            BuildEventosIntranet(modelBuilder);

            //Notificaciones
            BuildNotificaciones(modelBuilder);

            // Expedientes Bancarios
            BuildExpedientesBancarios(
                modelBuilder
            );

            // Bitácora documental de Compliance
            BuildBitacoraDocumental(
                modelBuilder
            );

            // Permisos individuales de Compliance
            BuildPermisosCompliance(
                modelBuilder
            );

            BuildBitacoraEmpresas(
                modelBuilder
            );

            //Mesa de Servicio / Incidencias
            BuildServiceDesk(
                modelBuilder
            );

            //Adquisiciones
            BuildAdquisiciones(
                modelBuilder
            );

            //Adquisiciones
            BuildAdquisicionesCotizaciones(
                modelBuilder
            );


        }

        private static void BuildAdquisicionesCotizaciones(ModelBuilder b)
        {
            b.Entity<AdqCotizacion>()
                .HasOne(
                    x => x.Solicitud
                )
                .WithMany()
                .HasForeignKey(
                    x => x.SolicitudId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );


            b.Entity<AdqCotizacionDetalle>()
                .HasOne(
                    x => x.Cotizacion
                )
                .WithMany(
                    x => x.Detalles
                )
                .HasForeignKey(
                    x => x.CotizacionId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );


            b.Entity<AdqCotizacionAdjunto>()
                .HasOne(
                    x => x.Cotizacion
                )
                .WithMany(
                    x => x.Adjuntos
                )
                .HasForeignKey(
                    x => x.CotizacionId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );


            b.Entity<AdqCotizacionAdjunto>()
                .HasOne(
                    x => x.CotizacionDetalle
                )
                .WithMany(
                    x => x.Adjuntos
                )
                .HasForeignKey(
                    x => x.CotizacionDetalleId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );


            // =========================================================
            // APROBACIÓN PRESUPUESTAL
            // =========================================================

            b.Entity<AdqAprobacionPresupuestal>()
                .HasOne(
                    x => x.Solicitud
                )
                .WithMany()
                .HasForeignKey(
                    x => x.SolicitudId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );


            b.Entity<AdqAprobacionPresupuestal>()
                .HasOne(
                    x => x.Cotizacion
                )
                .WithMany()
                .HasForeignKey(
                    x => x.CotizacionId
                )
                .OnDelete(
                    DeleteBehavior.Restrict
                );
        }


        private static void BuildServiceDesk(ModelBuilder b)
        {
            // =========================================================
            // TIPOS DE TICKET
            // =========================================================

            b.Entity<ServiceTicketType>(entity =>
            {
                entity.ToTable("SD_TiposTicket");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Codigo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.Descripcion)
                    .HasMaxLength(500);

                entity.Property(x => x.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(x => x.Orden)
                    .IsRequired();

                entity.HasIndex(x => x.Codigo)
                    .IsUnique()
                    .HasDatabaseName("UX_SD_TiposTicket_Codigo");

                entity.HasIndex(x => x.Nombre)
                    .HasDatabaseName("IX_SD_TiposTicket_Nombre");

                entity.HasData(
                    new ServiceTicketType
                    {
                        Id = 1,
                        Nombre = "Incidente",
                        Codigo = "INC",
                        Descripcion = "Falla, interrupción o afectación de un servicio.",
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceTicketType
                    {
                        Id = 2,
                        Nombre = "Solicitud de Servicio",
                        Codigo = "SR",
                        Descripcion = "Solicitud de acceso, equipo, software o servicio.",
                        Activo = true,
                        Orden = 2
                    },
                    new ServiceTicketType
                    {
                        Id = 3,
                        Nombre = "Problema",
                        Codigo = "PRB",
                        Descripcion = "Análisis de causa raíz de incidentes recurrentes.",
                        Activo = true,
                        Orden = 3
                    },
                    new ServiceTicketType
                    {
                        Id = 4,
                        Nombre = "Cambio",
                        Codigo = "CHG",
                        Descripcion = "Solicitud de cambio controlado en infraestructura o sistemas.",
                        Activo = true,
                        Orden = 4
                    }
                );
            });


            // =========================================================
            // ESTADOS
            // =========================================================

            b.Entity<ServiceTicketStatus>(entity =>
            {
                entity.ToTable("SD_EstadosTicket");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Codigo)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(x => x.Descripcion)
                    .HasMaxLength(500);

                entity.Property(x => x.EsFinal)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(x => x.PausaSla)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(x => x.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(x => x.Orden)
                    .IsRequired();

                entity.HasIndex(x => x.Codigo)
                    .IsUnique()
                    .HasDatabaseName("UX_SD_EstadosTicket_Codigo");

                entity.HasData(
                    new ServiceTicketStatus
                    {
                        Id = 1,
                        Nombre = "Nuevo",
                        Codigo = "NUEVO",
                        Descripcion = "Ticket registrado y pendiente de atención.",
                        EsFinal = false,
                        PausaSla = false,
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceTicketStatus
                    {
                        Id = 2,
                        Nombre = "Asignado",
                        Codigo = "ASIGNADO",
                        Descripcion = "Ticket asignado a un administrador o equipo.",
                        EsFinal = false,
                        PausaSla = false,
                        Activo = true,
                        Orden = 2
                    },
                    new ServiceTicketStatus
                    {
                        Id = 3,
                        Nombre = "En proceso",
                        Codigo = "EN_PROCESO",
                        Descripcion = "Ticket actualmente en atención.",
                        EsFinal = false,
                        PausaSla = false,
                        Activo = true,
                        Orden = 3
                    },
                    new ServiceTicketStatus
                    {
                        Id = 4,
                        Nombre = "Pendiente del usuario",
                        Codigo = "PENDIENTE_USUARIO",
                        Descripcion = "Se requiere información o respuesta del solicitante.",
                        EsFinal = false,
                        PausaSla = true,
                        Activo = true,
                        Orden = 4
                    },
                    new ServiceTicketStatus
                    {
                        Id = 5,
                        Nombre = "Resuelto",
                        Codigo = "RESUELTO",
                        Descripcion = "El administrador ha registrado una solución.",
                        EsFinal = false,
                        PausaSla = false,
                        Activo = true,
                        Orden = 5
                    },
                    new ServiceTicketStatus
                    {
                        Id = 6,
                        Nombre = "Cerrado",
                        Codigo = "CERRADO",
                        Descripcion = "Ticket finalizado.",
                        EsFinal = true,
                        PausaSla = false,
                        Activo = true,
                        Orden = 6
                    },
                    new ServiceTicketStatus
                    {
                        Id = 7,
                        Nombre = "Reabierto",
                        Codigo = "REABIERTO",
                        Descripcion = "Ticket reabierto después de su resolución.",
                        EsFinal = false,
                        PausaSla = false,
                        Activo = true,
                        Orden = 7
                    },
                    new ServiceTicketStatus
                    {
                        Id = 8,
                        Nombre = "Cancelado",
                        Codigo = "CANCELADO",
                        Descripcion = "Ticket cancelado.",
                        EsFinal = true,
                        PausaSla = false,
                        Activo = true,
                        Orden = 8
                    }
                );
            });


            // =========================================================
            // PRIORIDADES
            // =========================================================

            b.Entity<ServiceTicketPriority>(entity =>
            {
                entity.ToTable("SD_PrioridadesTicket");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Codigo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.Nivel)
                    .IsRequired();

                entity.Property(x => x.MinutosRespuesta)
                    .IsRequired();

                entity.Property(x => x.MinutosResolucion)
                    .IsRequired();

                entity.Property(x => x.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasIndex(x => x.Codigo)
                    .IsUnique()
                    .HasDatabaseName("UX_SD_PrioridadesTicket_Codigo");

                entity.HasData(
                    new ServiceTicketPriority
                    {
                        Id = 1,
                        Nombre = "Crítica",
                        Codigo = "CRITICA",
                        Nivel = 1,
                        MinutosRespuesta = 15,
                        MinutosResolucion = 120,
                        Activo = true
                    },
                    new ServiceTicketPriority
                    {
                        Id = 2,
                        Nombre = "Alta",
                        Codigo = "ALTA",
                        Nivel = 2,
                        MinutosRespuesta = 30,
                        MinutosResolucion = 240,
                        Activo = true
                    },
                    new ServiceTicketPriority
                    {
                        Id = 3,
                        Nombre = "Media",
                        Codigo = "MEDIA",
                        Nivel = 3,
                        MinutosRespuesta = 120,
                        MinutosResolucion = 480,
                        Activo = true
                    },
                    new ServiceTicketPriority
                    {
                        Id = 4,
                        Nombre = "Baja",
                        Codigo = "BAJA",
                        Nivel = 4,
                        MinutosRespuesta = 240,
                        MinutosResolucion = 1440,
                        Activo = true
                    }
                );
            });


            // =========================================================
            // CATEGORÍAS
            // =========================================================

            b.Entity<ServiceCategory>(entity =>
            {
                entity.ToTable("SD_Categorias");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Descripcion)
                    .HasMaxLength(500);

                entity.Property(x => x.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(x => x.Orden)
                    .IsRequired();

                entity.HasIndex(x => x.Nombre)
                    .IsUnique()
                    .HasDatabaseName("UX_SD_Categorias_Nombre");

                entity.HasData(
                    new ServiceCategory
                    {
                        Id = 1,
                        Nombre = "Infraestructura",
                        Descripcion = "Redes, servidores, VPN, conectividad e infraestructura tecnológica.",
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceCategory
                    {
                        Id = 2,
                        Nombre = "Software",
                        Descripcion = "Aplicaciones y programas utilizados por los usuarios.",
                        Activo = true,
                        Orden = 2
                    },
                    new ServiceCategory
                    {
                        Id = 3,
                        Nombre = "Accesos",
                        Descripcion = "Solicitudes o problemas relacionados con cuentas y permisos.",
                        Activo = true,
                        Orden = 3
                    },
                    new ServiceCategory
                    {
                        Id = 4,
                        Nombre = "Hardware",
                        Descripcion = "Equipos de cómputo y periféricos.",
                        Activo = true,
                        Orden = 4
                    },
                    new ServiceCategory
                    {
                        Id = 5,
                        Nombre = "Microsoft 365",
                        Descripcion = "Outlook, Teams, OneDrive y servicios Microsoft.",
                        Activo = true,
                        Orden = 5
                    },
                    new ServiceCategory
                    {
                        Id = 6,
                        Nombre = "Intranet",
                        Descripcion = "Incidencias y solicitudes relacionadas con la Intranet.",
                        Activo = true,
                        Orden = 6
                    },
                    new ServiceCategory
                    {
                        Id = 7,
                        Nombre = "Seguridad",
                        Descripcion = "Incidentes relacionados con ciberseguridad.",
                        Activo = true,
                        Orden = 7
                    },
                    new ServiceCategory
                    {
                        Id = 8,
                        Nombre = "Otros",
                        Descripcion = "Solicitudes que no pertenecen a otra categoría.",
                        Activo = true,
                        Orden = 8
                    }
                );
            });


            // =========================================================
            // SUBCATEGORÍAS
            // =========================================================

            b.Entity<ServiceSubcategory>(entity =>
            {
                entity.ToTable("SD_Subcategorias");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Descripcion)
                    .HasMaxLength(500);

                entity.Property(x => x.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(x => x.Orden)
                    .IsRequired();

                entity.HasOne(x => x.Category)
                    .WithMany(x => x.Subcategorias)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.CategoryId,
                    x.Nombre
                })
                .IsUnique()
                .HasDatabaseName("UX_SD_Subcategorias_Categoria_Nombre");

                entity.HasData(
                    new ServiceSubcategory
                    {
                        Id = 1,
                        CategoryId = 1,
                        Nombre = "VPN",
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceSubcategory
                    {
                        Id = 2,
                        CategoryId = 1,
                        Nombre = "Internet",
                        Activo = true,
                        Orden = 2
                    },
                    new ServiceSubcategory
                    {
                        Id = 3,
                        CategoryId = 1,
                        Nombre = "Servidor",
                        Activo = true,
                        Orden = 3
                    },
                    new ServiceSubcategory
                    {
                        Id = 4,
                        CategoryId = 2,
                        Nombre = "Instalación de software",
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceSubcategory
                    {
                        Id = 5,
                        CategoryId = 2,
                        Nombre = "Error de aplicación",
                        Activo = true,
                        Orden = 2
                    },
                    new ServiceSubcategory
                    {
                        Id = 6,
                        CategoryId = 3,
                        Nombre = "Alta de usuario",
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceSubcategory
                    {
                        Id = 7,
                        CategoryId = 3,
                        Nombre = "Permisos",
                        Activo = true,
                        Orden = 2
                    },
                    new ServiceSubcategory
                    {
                        Id = 8,
                        CategoryId = 3,
                        Nombre = "Contraseña",
                        Activo = true,
                        Orden = 3
                    },
                    new ServiceSubcategory
                    {
                        Id = 9,
                        CategoryId = 4,
                        Nombre = "Laptop / PC",
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceSubcategory
                    {
                        Id = 10,
                        CategoryId = 4,
                        Nombre = "Monitor",
                        Activo = true,
                        Orden = 2
                    },
                    new ServiceSubcategory
                    {
                        Id = 11,
                        CategoryId = 4,
                        Nombre = "Impresora",
                        Activo = true,
                        Orden = 3
                    },
                    new ServiceSubcategory
                    {
                        Id = 12,
                        CategoryId = 5,
                        Nombre = "Outlook",
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceSubcategory
                    {
                        Id = 13,
                        CategoryId = 5,
                        Nombre = "Teams",
                        Activo = true,
                        Orden = 2
                    },
                    new ServiceSubcategory
                    {
                        Id = 14,
                        CategoryId = 5,
                        Nombre = "OneDrive",
                        Activo = true,
                        Orden = 3
                    },
                    new ServiceSubcategory
                    {
                        Id = 15,
                        CategoryId = 6,
                        Nombre = "Acceso",
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceSubcategory
                    {
                        Id = 16,
                        CategoryId = 6,
                        Nombre = "Error funcional",
                        Activo = true,
                        Orden = 2
                    },
                    new ServiceSubcategory
                    {
                        Id = 17,
                        CategoryId = 7,
                        Nombre = "Correo sospechoso",
                        Activo = true,
                        Orden = 1
                    },
                    new ServiceSubcategory
                    {
                        Id = 18,
                        CategoryId = 7,
                        Nombre = "Malware",
                        Activo = true,
                        Orden = 2
                    }
                );
            });


            // =========================================================
            // EQUIPOS DE SOPORTE
            // =========================================================

            b.Entity<ServiceSupportTeam>(entity =>
            {
                entity.ToTable("SD_EquiposSoporte");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Descripcion)
                    .HasMaxLength(500);

                entity.Property(x => x.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasIndex(x => x.Nombre)
                    .IsUnique()
                    .HasDatabaseName("UX_SD_EquiposSoporte_Nombre");

                entity.HasData(
                    new ServiceSupportTeam
                    {
                        Id = 1,
                        Nombre = "Mesa de Servicio TI",
                        Descripcion = "Equipo principal responsable de la atención de tickets.",
                        Activo = true
                    }
                );
            });


            // =========================================================
            // USUARIOS DE EQUIPOS
            // =========================================================

            b.Entity<ServiceSupportTeamUser>(entity =>
            {
                entity.ToTable("SD_EquiposSoporteUsuarios");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(x => x.EsResponsable)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(x => x.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(x => x.FechaAsignacion)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(x => x.SupportTeam)
                    .WithMany(x => x.Usuarios)
                    .HasForeignKey(x => x.SupportTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.SupportTeamId,
                    x.UserId
                })
                .IsUnique()
                .HasDatabaseName("UX_SD_EquiposSoporteUsuarios_Equipo_Usuario");
            });


            // =========================================================
            // TICKETS
            // =========================================================

            b.Entity<ServiceTicket>(entity =>
            {
                entity.ToTable("SD_Tickets");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Folio)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(x => x.Titulo)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(x => x.Descripcion)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(x => x.UsuarioSolicitanteId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(x => x.UsuarioAsignadoId)
                    .HasMaxLength(450);

                entity.Property(x => x.Origen)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("Intranet");

                entity.Property(x => x.Resolucion)
                    .HasMaxLength(5000);

                entity.Property(x => x.UsuarioCierreId)
                    .HasMaxLength(450);

                entity.Property(x => x.FechaCreacion)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Eliminado)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(x => x.SlaRespuestaVencido)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(x => x.SlaResolucionVencido)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.HasIndex(x => x.Folio)
                    .IsUnique()
                    .HasDatabaseName("UX_SD_Tickets_Folio");

                entity.HasIndex(x => x.UsuarioSolicitanteId)
                    .HasDatabaseName("IX_SD_Tickets_UsuarioSolicitante");

                entity.HasIndex(x => x.UsuarioAsignadoId)
                    .HasDatabaseName("IX_SD_Tickets_UsuarioAsignado");

                entity.HasIndex(x => x.StatusId)
                    .HasDatabaseName("IX_SD_Tickets_Status");

                entity.HasIndex(x => x.PriorityId)
                    .HasDatabaseName("IX_SD_Tickets_Priority");

                entity.HasIndex(x => x.FechaCreacion)
                    .HasDatabaseName("IX_SD_Tickets_FechaCreacion");

                entity.HasOne(x => x.TicketType)
                    .WithMany(x => x.Tickets)
                    .HasForeignKey(x => x.TicketTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Status)
                    .WithMany(x => x.Tickets)
                    .HasForeignKey(x => x.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Priority)
                    .WithMany(x => x.Tickets)
                    .HasForeignKey(x => x.PriorityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Category)
                    .WithMany(x => x.Tickets)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Subcategory)
                    .WithMany(x => x.Tickets)
                    .HasForeignKey(x => x.SubcategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.SupportTeam)
                    .WithMany(x => x.Tickets)
                    .HasForeignKey(x => x.SupportTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Solicitante
                entity.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioSolicitanteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Administrador/agente asignado
                entity.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioAsignadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Usuario que cierra
                entity.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioCierreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(x => !x.Eliminado);
            });


            // =========================================================
            // COMENTARIOS
            // =========================================================

            b.Entity<ServiceTicketComment>(entity =>
            {
                entity.ToTable("SD_TicketComentarios");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.UsuarioId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(x => x.Comentario)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(x => x.EsNotaInterna)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(x => x.FechaCreacion)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Eliminado)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.HasOne(x => x.Ticket)
                    .WithMany(x => x.Comentarios)
                    .HasForeignKey(x => x.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.TicketId,
                    x.FechaCreacion
                })
                .HasDatabaseName("IX_SD_TicketComentarios_Ticket_Fecha");

                entity.HasQueryFilter(x => !x.Eliminado);
            });


            // =========================================================
            // ADJUNTOS
            // =========================================================

            b.Entity<ServiceTicketAttachment>(entity =>
            {
                entity.ToTable("SD_TicketAdjuntos");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.NombreOriginal)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(x => x.NombreAlmacenado)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(x => x.RutaArchivo)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(x => x.Extension)
                    .HasMaxLength(20);

                entity.Property(x => x.MimeType)
                    .HasMaxLength(150);

                entity.Property(x => x.UsuarioCargaId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(x => x.FechaCarga)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.Eliminado)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.HasOne(x => x.Ticket)
                    .WithMany(x => x.Adjuntos)
                    .HasForeignKey(x => x.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioCargaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.TicketId)
                    .HasDatabaseName("IX_SD_TicketAdjuntos_Ticket");

                entity.HasQueryFilter(x => !x.Eliminado);
            });


            // =========================================================
            // HISTORIAL DEL TICKET
            // =========================================================

            b.Entity<ServiceTicketHistory>(entity =>
            {
                entity.ToTable("SD_TicketHistorial");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.UsuarioId)
                    .HasMaxLength(450);

                entity.Property(x => x.Accion)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Campo)
                    .HasMaxLength(150);

                entity.Property(x => x.ValorAnterior)
                    .HasMaxLength(2000);

                entity.Property(x => x.ValorNuevo)
                    .HasMaxLength(2000);

                entity.Property(x => x.Detalle)
                    .HasMaxLength(3000);

                entity.Property(x => x.FechaHora)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.DireccionIp)
                    .HasMaxLength(64);

                entity.HasOne(x => x.Ticket)
                    .WithMany(x => x.Historial)
                    .HasForeignKey(x => x.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.TicketId,
                    x.FechaHora
                })
                .HasDatabaseName("IX_SD_TicketHistorial_Ticket_Fecha");
            });
        }

        private static void BuildAdquisiciones(ModelBuilder b)
        {
            // =========================================================
            // ESTATUS
            // =========================================================

            b.Entity<AdqEstatus>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_Estatus"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.Nombre
                        )
                        .IsRequired()
                        .HasMaxLength(100);

                    entity.Property(
                            x => x.Codigo
                        )
                        .IsRequired()
                        .HasMaxLength(50);

                    entity.Property(
                            x => x.Descripcion
                        )
                        .HasMaxLength(500);

                    entity.Property(
                            x => x.Orden
                        )
                        .IsRequired();

                    entity.Property(
                            x => x.Activo
                        )
                        .IsRequired()
                        .HasDefaultValue(true);

                    entity.HasIndex(
                            x => x.Codigo
                        )
                        .IsUnique()
                        .HasDatabaseName(
                            "UX_ADQ_Estatus_Codigo"
                        );

                    entity.HasIndex(
                            x => x.Orden
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Estatus_Orden"
                        );

                    entity.HasData(
                        new AdqEstatus
                        {
                            Id = 1,
                            Nombre = "Borrador",
                            Codigo = "BORRADOR",
                            Descripcion =
                                "Solicitud en proceso de captura.",
                            Orden = 1,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 2,
                            Nombre =
                                "Pendiente aprobación Gerente",
                            Codigo =
                                "PENDIENTE_GERENTE",
                            Descripcion =
                                "Solicitud pendiente de aprobación por el gerente del área.",
                            Orden = 2,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 3,
                            Nombre =
                                "Solicitud enviada",
                            Codigo =
                                "SOLICITUD_ENVIADA",
                            Descripcion =
                                "Solicitud enviada al área de Adquisiciones.",
                            Orden = 3,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 4,
                            Nombre =
                                "En revisión por Adquisiciones",
                            Codigo =
                                "EN_REVISION",
                            Descripcion =
                                "Solicitud siendo revisada por Adquisiciones.",
                            Orden = 4,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 5,
                            Nombre =
                                "Aprobada",
                            Codigo =
                                "APROBADA",
                            Descripcion =
                                "Solicitud aprobada por Adquisiciones.",
                            Orden = 5,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 6,
                            Nombre =
                                "Rechazada",
                            Codigo =
                                "RECHAZADA",
                            Descripcion =
                                "Solicitud rechazada.",
                            Orden = 6,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 7,
                            Nombre =
                                "Cancelada",
                            Codigo =
                                "CANCELADA",
                            Descripcion =
                                "Solicitud cancelada.",
                            Orden = 7,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 8,
                            Nombre =
                                "Asignada",
                            Codigo =
                                "ASIGNADA",
                            Descripcion =
                                "Solicitud asignada a un agente de compras.",
                            Orden = 8,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 9,
                            Nombre =
                                "En proceso de cotización",
                            Codigo =
                                "EN_COTIZACION",
                            Descripcion =
                                "Solicitud en proceso de cotización.",
                            Orden = 9,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 10,
                            Nombre =
                                "Cotización finalizada",
                            Codigo =
                                "COTIZACION_FINALIZADA",
                            Descripcion =
                                "Proceso de cotización finalizado.",
                            Orden = 10,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 11,
                            Nombre =
                                "Pendiente aprobación presupuestal",
                            Codigo =
                                "PENDIENTE_PRESUPUESTO",
                            Descripcion =
                                "Solicitud pendiente de iniciar el flujo presupuestal.",
                            Orden = 11,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 12,
                            Nombre =
                                "En aprobación presupuestal",
                            Codigo =
                                "EN_APROBACION_PRESUPUESTAL",
                            Descripcion =
                                "Solicitud dentro del flujo de aprobación presupuestal.",
                            Orden = 12,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 13,
                            Nombre =
                                "Aprobación presupuestal completada",
                            Codigo =
                                "PRESUPUESTO_APROBADO",
                            Descripcion =
                                "Flujo presupuestal completado.",
                            Orden = 13,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 14,
                            Nombre =
                                "En proceso de pago",
                            Codigo =
                                "EN_PAGO",
                            Descripcion =
                                "Solicitud en proceso de pago.",
                            Orden = 14,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 15,
                            Nombre =
                                "En proceso de compra",
                            Codigo =
                                "EN_COMPRA",
                            Descripcion =
                                "Compra en proceso.",
                            Orden = 15,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 16,
                            Nombre =
                                "En proceso de entrega",
                            Codigo =
                                "EN_ENTREGA",
                            Descripcion =
                                "Compra en proceso de entrega.",
                            Orden = 16,
                            Activo = true
                        },
                        new AdqEstatus
                        {
                            Id = 17,
                            Nombre =
                                "Finalizada",
                            Codigo =
                                "FINALIZADA",
                            Descripcion =
                                "Proceso de adquisición finalizado.",
                            Orden = 17,
                            Activo = true
                        }
                    );
                }
            );


            // =========================================================
            // SOLICITUDES
            // =========================================================

            b.Entity<AdqSolicitud>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_Solicitudes"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.Folio
                        )
                        .IsRequired()
                        .HasMaxLength(30);

                    entity.Property(
                            x => x.Titulo
                        )
                        .IsRequired()
                        .HasMaxLength(250);

                    entity.Property(
                            x => x.FechaSolicitud
                        )
                        .IsRequired();

                    entity.Property(
                            x => x.UsuarioSolicitanteId
                        )
                        .IsRequired()
                        .HasMaxLength(450);

                    entity.Property(
                            x => x.UsuarioAsignadoId
                        )
                        .HasMaxLength(450);

                    entity.Property(
                            x => x.Descripcion
                        )
                        .IsRequired()
                        .HasMaxLength(5000);

                    entity.Property(
                            x => x.Justificacion
                        )
                        .IsRequired()
                        .HasMaxLength(5000);

                    entity.Property(
                            x => x.FechaCreacion
                        )
                        .IsRequired()
                        .HasDefaultValueSql(
                            "GETDATE()"
                        );

                    entity.Property(
                            x => x.Eliminado
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.HasOne(
                            x => x.UsuarioSolicitante
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.UsuarioSolicitanteId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.UsuarioAsignado
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.UsuarioAsignadoId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.EmpleadoSolicitante
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.EmpleadoSolicitanteId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.Area
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.AreaId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.Estatus
                        )
                        .WithMany(
                            x => x.Solicitudes
                        )
                        .HasForeignKey(
                            x => x.EstatusId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasIndex(
                            x => x.Folio
                        )
                        .IsUnique()
                        .HasDatabaseName(
                            "UX_ADQ_Solicitudes_Folio"
                        );

                    entity.HasIndex(
                            x => x.UsuarioSolicitanteId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Solicitudes_UsuarioSolicitante"
                        );

                    entity.HasIndex(
                            x => x.UsuarioAsignadoId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Solicitudes_UsuarioAsignado"
                        );

                    entity.HasIndex(
                            x => x.AreaId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Solicitudes_Area"
                        );

                    entity.HasIndex(
                            x => x.EstatusId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Solicitudes_Estatus"
                        );

                    entity.HasIndex(
                            x => x.FechaSolicitud
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Solicitudes_FechaSolicitud"
                        );

                    entity.HasIndex(
                        x => new
                        {
                            x.EstatusId,
                            x.FechaSolicitud
                        })
                        .HasDatabaseName(
                            "IX_ADQ_Solicitudes_Estatus_Fecha"
                        );
                }
            );


            // =========================================================
            // DETALLE DE SOLICITUD
            // =========================================================

            b.Entity<AdqSolicitudDetalle>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_SolicitudesDetalle"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.ProductoServicio
                        )
                        .IsRequired()
                        .HasMaxLength(500);

                    entity.Property(
                            x => x.Cantidad
                        )
                        .IsRequired()
                        .HasPrecision(
                            18,
                            4
                        );

                    entity.Property(
                            x => x.Unidad
                        )
                        .IsRequired()
                        .HasMaxLength(100);

                    entity.Property(
                            x => x.Descripcion
                        )
                        .HasMaxLength(2000);

                    entity.Property(
                            x => x.Orden
                        )
                        .IsRequired();

                    entity.Property(
                            x => x.Eliminado
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.HasOne(
                            x => x.Solicitud
                        )
                        .WithMany(
                            x => x.Detalles
                        )
                        .HasForeignKey(
                            x => x.SolicitudId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasIndex(
                            x => x.SolicitudId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_SolicitudesDetalle_Solicitud"
                        );

                    entity.HasIndex(
                        x => new
                        {
                            x.SolicitudId,
                            x.Orden
                        })
                        .HasDatabaseName(
                            "IX_ADQ_SolicitudesDetalle_Solicitud_Orden"
                        );
                }
            );


            // =========================================================
            // ADJUNTOS
            // =========================================================

            b.Entity<AdqAdjunto>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_Adjuntos"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.NombreOriginal
                        )
                        .IsRequired()
                        .HasMaxLength(260);

                    entity.Property(
                            x => x.NombreGuardado
                        )
                        .IsRequired()
                        .HasMaxLength(260);

                    entity.Property(
                            x => x.RutaArchivo
                        )
                        .IsRequired()
                        .HasMaxLength(1000);

                    entity.Property(
                            x => x.Extension
                        )
                        .HasMaxLength(20);

                    entity.Property(
                            x => x.MimeType
                        )
                        .HasMaxLength(150);

                    entity.Property(
                            x => x.UsuarioCargaId
                        )
                        .IsRequired()
                        .HasMaxLength(450);

                    entity.Property(
                            x => x.TipoDocumento
                        )
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasDefaultValue(
                            "General"
                        );

                    entity.Property(
                            x => x.FechaCarga
                        )
                        .IsRequired()
                        .HasDefaultValueSql(
                            "GETDATE()"
                        );

                    entity.Property(
                            x => x.Eliminado
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.HasOne(
                            x => x.Solicitud
                        )
                        .WithMany(
                            x => x.Adjuntos
                        )
                        .HasForeignKey(
                            x => x.SolicitudId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.UsuarioCarga
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.UsuarioCargaId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasIndex(
                            x => x.SolicitudId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Adjuntos_Solicitud"
                        );

                    entity.HasIndex(
                        x => new
                        {
                            x.SolicitudId,
                            x.TipoDocumento
                        })
                        .HasDatabaseName(
                            "IX_ADQ_Adjuntos_Solicitud_Tipo"
                        );
                }
            );


            // =========================================================
            // HISTORIAL
            // =========================================================

            b.Entity<AdqHistorial>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_Historial"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.UsuarioId
                        )
                        .IsRequired()
                        .HasMaxLength(450);

                    entity.Property(
                            x => x.TipoEvento
                        )
                        .IsRequired()
                        .HasMaxLength(100);

                    entity.Property(
                            x => x.Descripcion
                        )
                        .IsRequired()
                        .HasMaxLength(2000);

                    entity.Property(
                            x => x.FechaEvento
                        )
                        .IsRequired()
                        .HasDefaultValueSql(
                            "GETDATE()"
                        );

                    entity.Property(
                            x => x.DireccionIp
                        )
                        .HasMaxLength(64);

                    entity.HasOne(
                            x => x.Solicitud
                        )
                        .WithMany(
                            x => x.Historial
                        )
                        .HasForeignKey(
                            x => x.SolicitudId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.Usuario
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.UsuarioId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne<AdqEstatus>()
                        .WithMany()
                        .HasForeignKey(
                            x => x.EstatusAnteriorId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne<AdqEstatus>()
                        .WithMany()
                        .HasForeignKey(
                            x => x.EstatusNuevoId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasIndex(
                            x => x.SolicitudId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Historial_Solicitud"
                        );

                    entity.HasIndex(
                        x => new
                        {
                            x.SolicitudId,
                            x.FechaEvento
                        })
                        .HasDatabaseName(
                            "IX_ADQ_Historial_Solicitud_Fecha"
                        );

                    entity.HasIndex(
                            x => x.TipoEvento
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Historial_TipoEvento"
                        );
                }
            );


            // =========================================================
            // APROBACIONES
            // =========================================================

            b.Entity<AdqAprobacion>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_Aprobaciones"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.TipoAprobacion
                        )
                        .IsRequired()
                        .HasMaxLength(100);

                    entity.Property(
                            x => x.UsuarioAprobadorId
                        )
                        .IsRequired()
                        .HasMaxLength(450);

                    entity.Property(
                            x => x.Estatus
                        )
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasDefaultValue(
                            "Pendiente"
                        );

                    entity.Property(
                            x => x.Comentario
                        )
                        .HasMaxLength(2000);

                    entity.Property(
                            x => x.FechaCreacion
                        )
                        .IsRequired()
                        .HasDefaultValueSql(
                            "GETDATE()"
                        );

                    entity.HasOne(
                            x => x.Solicitud
                        )
                        .WithMany(
                            x => x.Aprobaciones
                        )
                        .HasForeignKey(
                            x => x.SolicitudId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.UsuarioAprobador
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.UsuarioAprobadorId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasIndex(
                            x => x.SolicitudId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Aprobaciones_Solicitud"
                        );

                    entity.HasIndex(
                        x => new
                        {
                            x.UsuarioAprobadorId,
                            x.Estatus
                        })
                        .HasDatabaseName(
                            "IX_ADQ_Aprobaciones_Usuario_Estatus"
                        );

                    entity.HasIndex(
                        x => new
                        {
                            x.SolicitudId,
                            x.TipoAprobacion,
                            x.Orden
                        })
                        .HasDatabaseName(
                            "IX_ADQ_Aprobaciones_Solicitud_Tipo_Orden"
                        );
                }
            );


            // =========================================================
            // ASIGNACIONES
            // =========================================================

            b.Entity<AdqAsignacion>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_Asignaciones"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.UsuarioAsignadoId
                        )
                        .IsRequired()
                        .HasMaxLength(450);

                    entity.Property(
                            x => x.UsuarioAsignadorId
                        )
                        .IsRequired()
                        .HasMaxLength(450);

                    entity.Property(
                            x => x.FechaAsignacion
                        )
                        .IsRequired()
                        .HasDefaultValueSql(
                            "GETDATE()"
                        );

                    entity.Property(
                            x => x.Activa
                        )
                        .IsRequired()
                        .HasDefaultValue(true);

                    entity.Property(
                            x => x.Observaciones
                        )
                        .HasMaxLength(2000);

                    entity.HasOne(
                            x => x.Solicitud
                        )
                        .WithMany(
                            x => x.Asignaciones
                        )
                        .HasForeignKey(
                            x => x.SolicitudId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.UsuarioAsignado
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.UsuarioAsignadoId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.UsuarioAsignador
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.UsuarioAsignadorId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasIndex(
                            x => x.SolicitudId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_Asignaciones_Solicitud"
                        );

                    entity.HasIndex(
                        x => new
                        {
                            x.UsuarioAsignadoId,
                            x.Activa
                        })
                        .HasDatabaseName(
                            "IX_ADQ_Asignaciones_Usuario_Activa"
                        );
                }
            );


            // =========================================================
            // COMENTARIOS
            // =========================================================

            b.Entity<AdqComentario>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_Comentarios"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.UsuarioId
                        )
                        .IsRequired()
                        .HasMaxLength(450);

                    entity.Property(
                            x => x.Comentario
                        )
                        .IsRequired()
                        .HasMaxLength(5000);

                    entity.Property(
                            x => x.EsNotaInterna
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.FechaCreacion
                        )
                        .IsRequired()
                        .HasDefaultValueSql(
                            "GETDATE()"
                        );

                    entity.Property(
                            x => x.Eliminado
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.HasOne(
                            x => x.Solicitud
                        )
                        .WithMany(
                            x => x.Comentarios
                        )
                        .HasForeignKey(
                            x => x.SolicitudId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(
                            x => x.Usuario
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.UsuarioId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasIndex(
                        x => new
                        {
                            x.SolicitudId,
                            x.FechaCreacion
                        })
                        .HasDatabaseName(
                            "IX_ADQ_Comentarios_Solicitud_Fecha"
                        );
                }
            );


            // =========================================================
            // ADJUNTOS DE COMENTARIOS
            // =========================================================

            b.Entity<AdqComentarioAdjunto>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_ComentariosAdjuntos"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.NombreOriginal
                        )
                        .IsRequired()
                        .HasMaxLength(260);

                    entity.Property(
                            x => x.NombreGuardado
                        )
                        .IsRequired()
                        .HasMaxLength(260);

                    entity.Property(
                            x => x.RutaArchivo
                        )
                        .IsRequired()
                        .HasMaxLength(1000);

                    entity.Property(
                            x => x.Extension
                        )
                        .HasMaxLength(20);

                    entity.Property(
                            x => x.MimeType
                        )
                        .HasMaxLength(150);

                    entity.Property(
                            x => x.FechaCarga
                        )
                        .IsRequired()
                        .HasDefaultValueSql(
                            "GETDATE()"
                        );

                    entity.Property(
                            x => x.Eliminado
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.HasOne(
                            x => x.Comentario
                        )
                        .WithMany(
                            x => x.Adjuntos
                        )
                        .HasForeignKey(
                            x => x.ComentarioId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasIndex(
                            x => x.ComentarioId
                        )
                        .HasDatabaseName(
                            "IX_ADQ_ComentariosAdjuntos_Comentario"
                        );
                }
            );


            // =========================================================
            // PERMISOS POR USUARIO
            // =========================================================

            b.Entity<AdqPermisoUsuario>(
                entity =>
                {
                    entity.ToTable(
                        "ADQ_PermisosUsuarios"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.UsuarioId
                        )
                        .IsRequired()
                        .HasMaxLength(450);

                    entity.Property(
                            x => x.PuedeVisualizar
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.PuedeCrearSolicitud
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.PuedeGestionarSolicitudes
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.PuedeAprobar
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.PuedeAsignar
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.PuedeCotizar
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.PuedeGestionarProveedores
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.PuedeGenerarSolicitudPago
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.PuedeVerReportes
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.PuedeAdministrar
                        )
                        .IsRequired()
                        .HasDefaultValue(false);

                    entity.Property(
                            x => x.FechaCreacion
                        )
                        .IsRequired()
                        .HasDefaultValueSql(
                            "GETDATE()"
                        );

                    entity.Property(
                            x => x.UsuarioModificacionId
                        )
                        .HasMaxLength(450);

                    entity.HasOne(
                            x => x.Usuario
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.UsuarioId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasIndex(
                            x => x.UsuarioId
                        )
                        .IsUnique()
                        .HasDatabaseName(
                            "UX_ADQ_PermisosUsuarios_UsuarioId"
                        );

                    entity.HasIndex(
                            x => x.FechaModificacion
                        )
                        .HasDatabaseName(
                            "IX_ADQ_PermisosUsuarios_FechaModificacion"
                        );
                }
            );
        }

        private static void BuildBitacoraEmpresas(
        ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EbBitacoraEmpresa>(
                entity =>
                {
                    entity.ToTable(
                        "EB_BitacoraEmpresas"
                    );

                    entity.HasKey(
                        x => x.Id
                    );

                    entity.Property(
                            x => x.EmpresaId
                        )
                        .IsRequired();

                    entity.Property(
                            x => x.Accion
                        )
                        .HasMaxLength(80)
                        .IsRequired();

                    entity.Property(
                            x => x.UsuarioId
                        )
                        .HasMaxLength(450)
                        .IsRequired();

                    entity.Property(
                            x => x.NombreUsuario
                        )
                        .HasMaxLength(250)
                        .IsRequired();

                    entity.Property(
                            x => x.FechaEvento
                        )
                        .HasDefaultValueSql(
                            "GETDATE()"
                        )
                        .IsRequired();

                    entity.Property(
                            x => x.DireccionIp
                        )
                        .HasMaxLength(64);

                    entity.Property(
                            x => x.Navegador
                        )
                        .HasMaxLength(1000);

                    entity.Property(
                            x => x.Exitoso
                        )
                        .HasDefaultValue(true)
                        .IsRequired();

                    entity.Property(
                            x => x.Detalle
                        )
                        .HasMaxLength(2000);

                    entity.HasIndex(
                        x => x.EmpresaId
                    );

                    entity.HasIndex(
                        x => x.UsuarioId
                    );

                    entity.HasIndex(
                        x => x.FechaEvento
                    );

                    entity.HasIndex(
                        x => new
                        {
                            x.Accion,
                            x.FechaEvento
                        }
                    );

                    entity.HasOne(
                            x => x.Empresa
                        )
                        .WithMany()
                        .HasForeignKey(
                            x => x.EmpresaId
                        )
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );
                }
            );
        }

        private static void BuildBitacoraDocumental(ModelBuilder b)
        {
            b.Entity<EbBitacoraDocumento>(
                entity =>
                {
                    entity.ToTable(
                        "EB_BitacoraDocumentos"
                    );

                    entity.HasKey(x => x.Id);

                    entity.Property(x => x.Accion)
                        .HasMaxLength(50)
                        .IsRequired();

                    entity.Property(x => x.UsuarioId)
                        .HasMaxLength(450);

                    entity.Property(x => x.NombreUsuario)
                        .HasMaxLength(250);

                    entity.Property(x => x.NombreDocumento)
                        .HasMaxLength(250);

                    entity.Property(x => x.Banco)
                        .HasMaxLength(50);

                    entity.Property(x => x.DireccionIp)
                        .HasMaxLength(64);

                    entity.Property(x => x.Navegador)
                        .HasMaxLength(1000);

                    entity.Property(x => x.Detalle)
                        .HasMaxLength(1000);

                    entity.Property(x => x.FechaEvento)
                        .IsRequired();

                    entity.Property(x => x.Exitoso)
                        .IsRequired();

                    entity.HasOne(x => x.Empresa)
                        .WithMany(x =>
                            x.BitacoraDocumental)
                        .HasForeignKey(x =>
                            x.EmpresaId)
                        .OnDelete(
                            DeleteBehavior.Restrict
                        );

                    entity.HasOne(x => x.Documento)
                        .WithMany(x =>
                            x.Bitacora)
                        .HasForeignKey(x =>
                            x.DocumentoId)
                        .OnDelete(
                            DeleteBehavior.SetNull
                        );

                    entity.HasOne(x =>
                            x.TipoDocumento)
                        .WithMany(x =>
                            x.Bitacora)
                        .HasForeignKey(x =>
                            x.TipoDocumentoId)
                        .OnDelete(
                            DeleteBehavior.SetNull
                        );

                    entity.HasIndex(x =>
                            x.FechaEvento)
                        .HasDatabaseName(
                            "IX_EB_BitacoraDocumentos_FechaEvento"
                        );

                    entity.HasIndex(x => new
                    {
                        x.Accion,
                        x.FechaEvento
                    })
                        .HasDatabaseName(
                            "IX_EB_BitacoraDocumentos_Accion_Fecha"
                        );

                    entity.HasIndex(x => new
                    {
                        x.UsuarioId,
                        x.FechaEvento
                    })
                        .HasDatabaseName(
                            "IX_EB_BitacoraDocumentos_Usuario_Fecha"
                        );

                    entity.HasIndex(x => new
                    {
                        x.EmpresaId,
                        x.FechaEvento
                    })
                        .HasDatabaseName(
                            "IX_EB_BitacoraDocumentos_Empresa_Fecha"
                        );

                    entity.HasIndex(x => new
                    {
                        x.Banco,
                        x.FechaEvento
                    })
                        .HasDatabaseName(
                            "IX_EB_BitacoraDocumentos_Banco_Fecha"
                        );
                }
            );
        }

        private static void BuildPermisosCompliance(
    ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<EbPermisoComplianceUsuario>(
                    entity =>
                    {
                        entity.ToTable(
                            "EB_PermisosComplianceUsuarios"
                        );

                        entity.HasKey(
                            x => x.Id
                        );

                        entity.Property(
                                x => x.UsuarioId
                            )
                            .IsRequired()
                            .HasMaxLength(450);

                        entity.Property(
                                x => x.PuedeVisualizar
                            )
                            .IsRequired()
                            .HasDefaultValue(false);

                        entity.Property(
                                x => x.PuedeCrearCargar
                            )
                            .IsRequired()
                            .HasDefaultValue(false);

                        entity.Property(
                                x => x.PuedeModificar
                            )
                            .IsRequired()
                            .HasDefaultValue(false);

                        entity.Property(
                                x => x.PuedeEliminar
                            )
                            .IsRequired()
                            .HasDefaultValue(false);

                        entity.Property(
                                x => x.PuedeDescargar
                            )
                            .IsRequired()
                            .HasDefaultValue(false);

                        entity.Property(
                                x => x.FechaCreacion
                            )
                            .IsRequired()
                            .HasDefaultValueSql(
                                "GETDATE()"
                            );

                        entity.Property(
                                x => x.FechaModificacion
                            );

                        entity.Property(
                                x => x.UsuarioModificacionId
                            )
                            .HasMaxLength(450);

                        /*
                         * Cada usuario solamente puede tener
                         * un registro de permisos Compliance.
                         */
                        entity.HasIndex(
                                x => x.UsuarioId
                            )
                            .IsUnique()
                            .HasDatabaseName(
                                "UX_EB_PermisosComplianceUsuarios_UsuarioId"
                            );

                        entity.HasIndex(
                                x => x.FechaModificacion
                            )
                            .HasDatabaseName(
                                "IX_EB_PermisosComplianceUsuarios_FechaModificacion"
                            );
                    }
                );
        }

        private static void BuildExpedientesBancarios(ModelBuilder b)
        {
            // ==========================
            // Empresas del nuevo módulo
            // ==========================
            b.Entity<EbEmpresa>(entity =>
            {
                entity.ToTable("EB_Empresas");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.RazonSocial)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(x => x.NombreCorto)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Rfc)
                    .IsRequired()
                    .HasMaxLength(13);

                entity.Property(x => x.Nivel)
                    .HasMaxLength(100);

                entity.Property(x => x.ActividadComercial)
                    .HasMaxLength(500);

                entity.Property(x => x.TelefonoBancos)
                    .HasMaxLength(30);

                entity.Property(x => x.CorreoBancos)
                    .HasMaxLength(200);

                entity.Property(x => x.NumeroEscritura)
                    .HasMaxLength(200);

                entity.Property(x => x.DomicilioFiscal)
                    .HasMaxLength(500);

                entity.Property(x => x.Observaciones)
                    .HasMaxLength(1000);

                entity.Property(x => x.UsuarioCreacionId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(x => x.UsuarioActualizacionId)
                    .HasMaxLength(450);

                entity.Property(x => x.Deshabilitado)
                    .HasDefaultValue(false);

                entity.Property(x => x.Eliminado)
                    .HasDefaultValue(false);

                entity.Property(x => x.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(x => x.Rfc)
                    .IsUnique()
                    .HasDatabaseName("UX_EB_Empresas_Rfc");

                entity.HasIndex(x => x.RazonSocial)
                    .HasDatabaseName("IX_EB_Empresas_RazonSocial");

                entity.HasIndex(x => x.NombreCorto)
                    .HasDatabaseName("IX_EB_Empresas_NombreCorto");

                entity.HasQueryFilter(x => !x.Eliminado);
            });

            // ==========================
            // Accionistas
            // ==========================
            b.Entity<EbAccionista>(entity =>
            {
                entity.ToTable("EB_Accionistas");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.NombreCompleto)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(x => x.Rfc)
                    .HasMaxLength(13);

                entity.Property(x => x.PorcentajeParticipacion)
                    .HasPrecision(7, 4);

                entity.Property(x => x.Nacionalidad)
                    .HasMaxLength(100);

                entity.Property(x => x.UsuarioCreacionId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(x => x.UsuarioActualizacionId)
                    .HasMaxLength(450);

                entity.Property(x => x.Deshabilitado)
                    .HasDefaultValue(false);

                entity.Property(x => x.Eliminado)
                    .HasDefaultValue(false);

                entity.Property(x => x.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(x => x.Empresa)
                    .WithMany(x => x.Accionistas)
                    .HasForeignKey(x => x.EmpresaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.EmpresaId)
                    .HasDatabaseName("IX_EB_Accionistas_EmpresaId");

                entity.HasQueryFilter(x => !x.Eliminado);
            });

            // ==========================
            // Tipos de documento
            // ==========================
            b.Entity<EbTipoDocumento>(entity =>
            {
                entity.ToTable("EB_TiposDocumento");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Categoria)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Descripcion)
                    .HasMaxLength(500);

                entity.Property(x => x.UsuarioCreacionId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(x => x.EsObligatorio)
                    .HasDefaultValue(true);

                entity.Property(x => x.RequiereFechaVencimiento)
                    .HasDefaultValue(false);

                entity.Property(x => x.PermiteMultiplesArchivos)
                    .HasDefaultValue(false);

                entity.Property(x => x.Deshabilitado)
                    .HasDefaultValue(false);

                entity.Property(x => x.Eliminado)
                    .HasDefaultValue(false);

                entity.Property(x => x.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(x => new { x.Nombre, x.Categoria })
                    .IsUnique()
                    .HasDatabaseName("UX_EB_TiposDocumento_Nombre_Categoria");

                entity.HasQueryFilter(x => !x.Eliminado);
            });

            DateTime fechaCatalogoEb = new DateTime(2026, 7, 29, 0, 0, 0);

            b.Entity<EbTipoDocumento>().HasData(
                new EbTipoDocumento
                {
                    Id = 1,
                    Nombre = "Constancia de Situación Fiscal",
                    Categoria = "Fiscal",
                    Descripcion = "Constancia de Situación Fiscal vigente de la empresa.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = false,
                    Orden = 1,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 2,
                    Nombre = "Certificado FIEL",
                    Categoria = "Fiscal",
                    Descripcion = "Certificado de firma electrónica vigente.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = true,
                    PermiteMultiplesArchivos = false,
                    Orden = 2,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 3,
                    Nombre = "Comprobante de domicilio",
                    Categoria = "Domicilio",
                    Descripcion = "Comprobante de domicilio fiscal o comercial.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = true,
                    PermiteMultiplesArchivos = true,
                    Orden = 3,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 4,
                    Nombre = "Acta constitutiva",
                    Categoria = "Corporativo",
                    Descripcion = "Acta constitutiva de la sociedad.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = false,
                    Orden = 4,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 5,
                    Nombre = "Actas o instrumentos adicionales",
                    Categoria = "Corporativo",
                    Descripcion = "Reformas, protocolizaciones o instrumentos adicionales.",
                    EsObligatorio = false,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = true,
                    Orden = 5,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 6,
                    Nombre = "Poder notarial",
                    Categoria = "Legal",
                    Descripcion = "Poderes notariales vigentes de representantes o apoderados.",
                    EsObligatorio = false,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = true,
                    Orden = 6,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 7,
                    Nombre = "INE de accionistas",
                    Categoria = "Accionistas",
                    Descripcion = "Identificación oficial de los accionistas.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = true,
                    PermiteMultiplesArchivos = true,
                    Orden = 7,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 8,
                    Nombre = "CSF de accionistas",
                    Categoria = "Accionistas",
                    Descripcion = "Constancia de Situación Fiscal de cada accionista.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = true,
                    Orden = 8,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 9,
                    Nombre = "Comprobante de domicilio de accionistas",
                    Categoria = "Accionistas",
                    Descripcion = "Comprobante de domicilio de cada accionista.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = true,
                    PermiteMultiplesArchivos = true,
                    Orden = 9,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 10,
                    Nombre = "Hoja membretada",
                    Categoria = "Corporativo",
                    Descripcion = "Hoja membretada vigente de la empresa.",
                    EsObligatorio = false,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = false,
                    Orden = 10,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 11,
                    Nombre = "Organigrama",
                    Categoria = "Corporativo",
                    Descripcion = "Organigrama actualizado de la empresa.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = false,
                    Orden = 11,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 12,
                    Nombre = "Declaración anual o mensual",
                    Categoria = "Financiero",
                    Descripcion = "Última declaración anual o mensual disponible.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = true,
                    Orden = 12,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 13,
                    Nombre = "Opinión de cumplimiento SAT",
                    Categoria = "Fiscal",
                    Descripcion = "Constancia de opinión de cumplimiento emitida por el SAT.",
                    EsObligatorio = true,
                    RequiereFechaVencimiento = true,
                    PermiteMultiplesArchivos = false,
                    Orden = 13,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 14,
                    Nombre = "Prueba de vida",
                    Categoria = "Evidencias",
                    Descripcion = "Imágenes o evidencias solicitadas por instituciones bancarias.",
                    EsObligatorio = false,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = true,
                    Orden = 14,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                },
                new EbTipoDocumento
                {
                    Id = 15,
                    Nombre = "Otro documento",
                    Categoria = "Otros",
                    Descripcion = "Documentación adicional requerida por la institución.",
                    EsObligatorio = false,
                    RequiereFechaVencimiento = false,
                    PermiteMultiplesArchivos = true,
                    Orden = 15,
                    Deshabilitado = false,
                    Eliminado = false,
                    FechaCreacion = fechaCatalogoEb,
                    UsuarioCreacionId = "SYSTEM"
                }
            );

            // ==========================
            // Documentos
            // ==========================
            b.Entity<EbDocumento>(entity =>
            {
                entity.ToTable("EB_Documentos");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.NombreOriginal)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(x => x.NombreAlmacenado)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(x => x.RutaArchivo)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(x => x.Extension)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.MimeType)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Estado)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Vigente");

                entity.Property(x => x.Observaciones)
                    .HasMaxLength(1000);

                entity.Property(x => x.UsuarioCargaId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(x => x.UsuarioEliminacionId)
                    .HasMaxLength(450);

                entity.Property(x => x.Version)
                    .HasDefaultValue(1);

                entity.Property(x => x.EsVersionActual)
                    .HasDefaultValue(true);

                entity.Property(x => x.Eliminado)
                    .HasDefaultValue(false);

                entity.Property(x => x.FechaCarga)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(x => x.Empresa)
                    .WithMany(x => x.Documentos)
                    .HasForeignKey(x => x.EmpresaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TipoDocumento)
                    .WithMany(x => x.Documentos)
                    .HasForeignKey(x => x.TipoDocumentoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.EmpresaId)
                    .HasDatabaseName("IX_EB_Documentos_EmpresaId");

                entity.HasIndex(x => x.TipoDocumentoId)
                    .HasDatabaseName("IX_EB_Documentos_TipoDocumentoId");

                entity.HasIndex(x => new
                {
                    x.EmpresaId,
                    x.TipoDocumentoId,
                    x.EsVersionActual
                })
                .HasDatabaseName("IX_EB_Documentos_Expediente");

                entity.HasQueryFilter(x => !x.Eliminado);
            });

            /*
             * ==========================================================
             * VÍNCULO DOCUMENTAL EMPRESAS ↔ COMPLIANCE
             * ==========================================================
             *
             * Esta tabla pertenece únicamente a la capa de integración.
             * No modifica las relaciones internas actuales de Empresas
             * ni de Compliance.
             */
            b.Entity<EbDocumentoVinculoEmpresa>(
                entity =>
                {
                    entity.ToTable(
                        "EB_DocumentosVinculosEmpresa"
                    );

                    entity.HasKey(x =>
                        x.Id
                    );

                    entity.Property(x =>
                            x.ArchivoEmpresaId)
                        .HasMaxLength(450);

                    entity.Property(x =>
                            x.HashContenido)
                        .HasMaxLength(64)
                        .IsRequired();

                    entity.Property(x =>
                            x.Origen)
                        .HasMaxLength(30)
                        .IsRequired();

                    entity.Property(x =>
                            x.Activo)
                        .IsRequired()
                        .HasDefaultValue(true);

                    entity.Property(x =>
                            x.FechaCreacion)
                        .IsRequired()
                        .HasDefaultValueSql(
                            "GETDATE()"
                        );

                    entity.Property(x =>
                        x.FechaActualizacion
                    );

                    /*
                     * Índice principal para localizar rápidamente
                     * la relación documental entre ambos módulos.
                     */
                    entity.HasIndex(x =>
                        new
                        {
                            x.EmpresaMaestraId,
                            x.EmpresaComplianceId,
                            x.TipoArchivoEmpresaId,
                            x.TipoDocumentoComplianceId,
                            x.Activo
                        }
                    )
                    .HasDatabaseName(
                        "IX_EB_DocumentosVinculosEmpresa_Relacion"
                    );

                    /*
                     * SHA-256 del contenido sincronizado.
                     */
                    entity.HasIndex(x =>
                        x.HashContenido
                    )
                    .HasDatabaseName(
                        "IX_EB_DocumentosVinculosEmpresa_Hash"
                    );

                    /*
                     * ID lógico del archivo actual en Empresas.
                     *
                     * NO creamos FK física porque Empresas puede
                     * eliminar y recrear ArchivoEmpresa durante
                     * una actualización.
                     */
                    entity.HasIndex(x =>
                        x.ArchivoEmpresaId
                    )
                    .HasDatabaseName(
                        "IX_EB_DocumentosVinculosEmpresa_ArchivoEmpresa"
                    );

                    /*
                     * Documento/version correspondiente en Compliance.
                     */
                    entity.HasIndex(x =>
                        x.DocumentoComplianceId
                    )
                    .HasDatabaseName(
                        "IX_EB_DocumentosVinculosEmpresa_DocumentoCompliance"
                    );
                }
            );
        }

        private static void BuildNotificaciones(ModelBuilder b)
        {
            b.Entity<NotificacionIntranet>()
                .HasOne(n => n.UsuarioCreador)
                .WithMany()
                .HasForeignKey(n => n.UserIdCreador)
                .OnDelete(DeleteBehavior.NoAction);

            b.Entity<NotificacionIntranetUsuario>()
                .HasOne(nu => nu.Notificacion)
                .WithMany(n => n.UsuariosNotificados)
                .HasForeignKey(nu => nu.NotificacionIntranetId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<NotificacionIntranetUsuario>()
                .HasOne(nu => nu.Usuario)
                .WithMany()
                .HasForeignKey(nu => nu.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private static void BuildEventosIntranet(ModelBuilder b)
        {
            b.Entity<EventoIntranet>(e =>
            {
                e.ToTable("EventosIntranet");
                e.HasKey(x => x.Id);

                e.Property(x => x.Titulo)
                    .IsRequired()
                    .HasMaxLength(250);

                e.Property(x => x.Descripcion)
                    .HasMaxLength(2000);

                e.Property(x => x.TipoEvento)
                    .HasMaxLength(100);

                e.Property(x => x.Region)
                    .HasMaxLength(200);

                e.Property(x => x.UrlFormulario)
                    .HasMaxLength(500);

                e.Property(x => x.TextoBoton)
                    .HasMaxLength(50);

                e.Property(x => x.RutaPortada)
                    .HasMaxLength(500);

                e.Property(x => x.NombrePortada)
                    .HasMaxLength(255);

                e.Property(x => x.Publicado)
                    .HasDefaultValue(false);

                e.Property(x => x.NotificacionEnviada)
                    .HasDefaultValue(false);

                e.Property(x => x.FechaNotificacion);

                e.Property(x => x.Activo)
                    .HasDefaultValue(true);

                e.Property(x => x.EsProgramado)
                    .HasDefaultValue(false);

                e.Property(x => x.RequiereGeolocalizacion)
                    .HasDefaultValue(false);

                e.Property(x => x.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                e.Property(x => x.CreadoPorId)
                    .HasMaxLength(450);

                e.Property(x => x.ModificadoPorId)
                    .HasMaxLength(450);

                e.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.CreadoPorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.ModificadoPorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.Activo, x.Publicado, x.FechaEvento });
            });
        }

        private static void BuildComunicadosInternos(ModelBuilder b)
        {
            b.Entity<ComunicadoInterno>(e =>
            {
                e.ToTable("ComunicadosInternos");
                e.HasKey(x => x.Id);

                e.Property(x => x.Titulo)
                    .IsRequired()
                    .HasMaxLength(250);

                e.Property(x => x.Descripcion)
                    .HasMaxLength(1000);

                e.Property(x => x.FechaPublicacion)
                    .IsRequired();

                e.Property(x => x.HoraPublicacion);

                e.Property(x => x.Publicado)
                    .HasDefaultValue(false);

                e.Property(x => x.NotificacionEnviada)
                    .HasDefaultValue(false);

                e.Property(x => x.FechaNotificacion);

                e.Property(x => x.EsPermanente)
                    .HasDefaultValue(false);

                e.Property(x => x.RutaArchivo)
                    .IsRequired()
                    .HasMaxLength(500);

                e.Property(x => x.NombreArchivo)
                    .HasMaxLength(255);

                e.Property(x => x.ExtensionArchivo)
                    .HasMaxLength(20);

                e.Property(x => x.Activo)
                    .HasDefaultValue(true);

                e.Property(x => x.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                e.Property(x => x.CreadoPorId)
                    .HasMaxLength(450);

                e.Property(x => x.ModificadoPorId)
                    .HasMaxLength(450);

                e.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.CreadoPorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey(x => x.ModificadoPorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.Activo, x.Publicado, x.FechaPublicacion });
                e.HasIndex(x => x.FechaPublicacion);
                e.HasIndex(x => x.Titulo);

                e.Property(x => x.RutaPortada)
                    .HasMaxLength(500);

                e.Property(x => x.NombrePortada)
                    .HasMaxLength(255);
            });
        }

        private static void BuildAusencias(ModelBuilder b)
        {
            b.Entity<TipoAusencia>(entity =>
            {
                entity.Property(x => x.Nombre).HasMaxLength(250);
                entity.HasData(
                    new TipoAusencia { Id = 1, Nombre = "Checada fuera de tiempo por instalación cerrada", Activo = true, ManejaHoras = true, ManejaDias = false, Orden = 1 },
                    new TipoAusencia { Id = 2, Nombre = "Permiso llegada tardía", Activo = true, ManejaHoras = true, ManejaDias = false, Orden = 2 },
                    new TipoAusencia { Id = 3, Nombre = "Permiso salida temprana", Activo = true, ManejaHoras = true, ManejaDias = false, Orden = 3 },
                    new TipoAusencia { Id = 4, Nombre = "Permiso de ausencia", Activo = true, ManejaHoras = false, ManejaDias = true, Orden = 4 },
                    new TipoAusencia { Id = 5, Nombre = "Permiso salida diligencia con regreso", Activo = true, ManejaHoras = true, ManejaDias = false, Orden = 5 },
                    new TipoAusencia { Id = 6, Nombre = "Omisión de checada (no aplica para casos en donde se incumplan horarios laborales)", Activo = true, ManejaHoras = true, ManejaDias = false, Orden = 6 },
                    new TipoAusencia { Id = 7, Nombre = "Permiso de paternidad", Activo = true, ManejaHoras = false, ManejaDias = true, Orden = 7 },
                    new TipoAusencia { Id = 8, Nombre = "Cambio de hora de comida (especificar razón y horario tomado)", Activo = true, ManejaHoras = true, ManejaDias = false, Orden = 8 },
                    new TipoAusencia { Id = 9, Nombre = "Permiso de ausencia por Fallecimiento de familiar", Activo = true, ManejaHoras = false, ManejaDias = true, Orden = 9 },
                    new TipoAusencia { Id = 10, Nombre = "Permiso de ausencia médica justificada", Activo = true, ManejaHoras = false, ManejaDias = true, Orden = 10 },
                    new TipoAusencia { Id = 11, Nombre = "Permiso diligencia sin regreso", Activo = true, ManejaHoras = true, ManejaDias = false, Orden = 11 },
                    new TipoAusencia { Id = 12, Nombre = "Permiso de ausencia personal justificada", Activo = true, ManejaHoras = false, ManejaDias = true, Orden = 12 },
                    new TipoAusencia { Id = 13, Nombre = "Permiso de ausencia por accidente justificado", Activo = true, ManejaHoras = false, ManejaDias = true, Orden = 13 },
                    new TipoAusencia { Id = 14, Nombre = "Permiso para trabajar desde casa (HO)", Activo = true, ManejaHoras = false, ManejaDias = true, Orden = 14 },
                    new TipoAusencia { Id = 15, Nombre = "Sin registro por falla de biométrico (sin luz y/o descompuesto)", Activo = true, ManejaHoras = true, ManejaDias = false, Orden = 15 },
                    new TipoAusencia { Id = 16, Nombre = "Permiso sin goce de sueldo", Activo = true, ManejaHoras = false, ManejaDias = true, Orden = 16 }
                );
            });

            b.Entity<TipoIncapacidad>(entity =>
            {
                entity.Property(x => x.Nombre).HasMaxLength(150);
                entity.HasData(
                    new TipoIncapacidad { Id = 1, Nombre = "Riesgo de trabajo", Activo = true, Orden = 1 },
                    new TipoIncapacidad { Id = 2, Nombre = "Enfermedad en general", Activo = true, Orden = 2 },
                    new TipoIncapacidad { Id = 3, Nombre = "Maternidad", Activo = true, Orden = 3 },
                    new TipoIncapacidad { Id = 4, Nombre = "Licencia por cuidados médicos de hijos diagnosticados con cáncer", Activo = true, Orden = 4 }
                );
            });

            b.Entity<Ausencia>(entity =>
            {
                entity.Property(x => x.Categoria).HasMaxLength(50);
                entity.Property(x => x.TipoCaptura).HasMaxLength(20);
                entity.Property(x => x.NumeroFolio).HasMaxLength(100);
                entity.Property(x => x.EstadoJefeDirecto).HasMaxLength(30);
                entity.Property(x => x.EstadoTH).HasMaxLength(30);
                entity.Property(x => x.Comentario).HasMaxLength(1000);
                entity.Property(x => x.Dias).HasPrecision(10, 2);
                entity.Property(x => x.Horas).HasPrecision(10, 2);

                entity.HasOne(x => x.TipoAusencia)
                    .WithMany()
                    .HasForeignKey(x => x.TipoAusenciaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TipoIncapacidad)
                    .WithMany()
                    .HasForeignKey(x => x.TipoIncapacidadId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.JefeDirectoEmpleado)
                    .WithMany()
                    .HasForeignKey(x => x.JefeDirectoEmpleadoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            b.Entity<AusenciaDocumento>(entity =>
            {
                entity.Property(x => x.NombreOriginal).HasMaxLength(260);
                entity.Property(x => x.NombreGuardado).HasMaxLength(260);
                entity.Property(x => x.RutaArchivo).HasMaxLength(500);
                entity.Property(x => x.Extension).HasMaxLength(10);
                entity.Property(x => x.UsuarioCreadorId).HasMaxLength(450);

                entity.HasOne(x => x.Ausencia)
                    .WithMany(x => x.Documentos)
                    .HasForeignKey(x => x.AusenciaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


        }

            private static void BuildPoliticas(ModelBuilder b)
        {
            b.Entity<TipoDocumento>(e =>
            {
                e.ToTable("TiposDocumento");

                e.HasKey(x => x.Id);

                e.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                e.Property(x => x.Activo)
                    .HasDefaultValue(true);

                e.Property(x => x.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Seed TiposDocumento
            b.Entity<TipoDocumento>().HasData(
                new TipoDocumento { Id = 1, Nombre = "Manuales", Activo = true },
                new TipoDocumento { Id = 2, Nombre = "Procedimientos", Activo = true },
                new TipoDocumento { Id = 3, Nombre = "Políticas", Activo = true },
                new TipoDocumento { Id = 4, Nombre = "Reglamentos", Activo = true },
                new TipoDocumento { Id = 5, Nombre = "Formatos", Activo = true },
                new TipoDocumento { Id = 6, Nombre = "Diagramas", Activo = true },
                new TipoDocumento { Id = 7, Nombre = "Referencias Normativas", Activo = true },
                //new TipoDocumento { Id = 8, Nombre = "Requerimientos", Activo = true },
                new TipoDocumento { Id = 9, Nombre = "Manuales de Capacitación", Activo = true },
                new TipoDocumento { Id = 10, Nombre = "Otros", Activo = true }
            );

            b.Entity<EstatusDocumento>(e =>
            {
                e.ToTable("EstatusDocumento");

                e.HasKey(x => x.Id);

                e.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(80);

                e.Property(x => x.Activo)
                    .HasDefaultValue(true);

                e.Property(x => x.EsPublicable)
                    .HasDefaultValue(false);
            });

            // Seed EstatusDocumento (NO incluye "Todos": eso es de UI)
            b.Entity<EstatusDocumento>().HasData(
                new EstatusDocumento { Id = 1, Nombre = "Vigente", Activo = true, EsPublicable = true },
                new EstatusDocumento { Id = 2, Nombre = "Obsoleto", Activo = true, EsPublicable = false },
                new EstatusDocumento { Id = 3, Nombre = "En Revisión", Activo = true, EsPublicable = false }
            );

            b.Entity<Documento>(e =>
            {
                e.ToTable("Documentos");

                e.HasKey(x => x.Id);

                e.Property(x => x.Titulo)
                    .IsRequired()
                    .HasMaxLength(250);

                e.Property(x => x.Descripcion)
                    .HasMaxLength(1000);

                // Nuevos campos
                e.Property(x => x.Responsable)
                    .HasMaxLength(150);

                e.Property(x => x.Observaciones)
                    .HasMaxLength(500);

                e.Property(x => x.Ubicacion)
                    .HasMaxLength(300);

                e.Property(x => x.NombreArchivo)
                    .HasMaxLength(300);

                e.Property(x => x.RutaArchivo)
                    .HasMaxLength(500);

                e.Property(x => x.Activo)
                    .HasDefaultValue(true);

                e.Property(x => x.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                // FK -> Area (tabla existente en Empleados)
                e.HasOne(x => x.Area)
                    .WithMany()
                    .HasForeignKey(x => x.AreaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // FK -> TipoDocumento
                e.HasOne(x => x.TipoDocumento)
                    .WithMany(t => t.Documentos)
                    .HasForeignKey(x => x.TipoDocumentoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // NUEVO: FK -> EstatusDocumento (cabecera)
                e.HasOne(x => x.EstatusDocumento)
                    .WithMany() // (no necesitas navegación inversa, a menos que la quieras)
                    .HasForeignKey(x => x.EstatusDocumentoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                e.HasIndex(x => x.AreaId);
                e.HasIndex(x => x.TipoDocumentoId);
                e.HasIndex(x => x.EstatusDocumentoId);
                e.HasIndex(x => x.Titulo);

                // Opcional (si filtras mucho por activo/estatus/tipo)
                e.HasIndex(x => new { x.AreaId, x.TipoDocumentoId, x.EstatusDocumentoId, x.Activo });
            });

            b.Entity<DocumentoVersion>(e =>
            {
                e.ToTable("DocumentoVersiones");

                e.HasKey(x => x.Id);

                e.Property(x => x.Version)
                    .IsRequired()
                    .HasMaxLength(20);

                e.Property(x => x.Comentarios)
                    .HasMaxLength(1000);

                e.Property(x => x.NombreArchivo)
                    .HasMaxLength(260);

                e.Property(x => x.RutaArchivo)
                    .HasMaxLength(800);

                e.Property(x => x.MimeType)
                    .HasMaxLength(100);

                e.Property(x => x.Activo)
                    .HasDefaultValue(true);

                e.Property(x => x.EsActual)
                    .HasDefaultValue(false);

                e.Property(x => x.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                // FK -> Documento
                e.HasOne(x => x.Documento)
                    .WithMany(d => d.Versiones)
                    .HasForeignKey(x => x.DocumentoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // FK -> EstatusDocumento (por versión)
                e.HasOne(x => x.EstatusDocumento)
                    .WithMany(s => s.Versiones)
                    .HasForeignKey(x => x.EstatusDocumentoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Una versión no se repite por documento
                e.HasIndex(x => new { x.DocumentoId, x.Version })
                    .IsUnique();

                // Solo una versión actual por documento
                e.HasIndex(x => new { x.DocumentoId, x.EsActual })
                    .IsUnique()
                    .HasFilter("[EsActual] = 1");
            });

            b.Entity<DocumentoPalabraClave>(e =>
            {
                e.ToTable("DocumentoPalabrasClave");

                e.HasKey(x => x.Id);

                e.Property(x => x.Palabra)
                    .IsRequired()
                    .HasMaxLength(80);

                e.HasOne(x => x.Documento)
                    .WithMany(d => d.PalabrasClave)
                    .HasForeignKey(x => x.DocumentoId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.DocumentoId, x.Palabra })
                    .IsUnique();

                e.HasIndex(x => x.Palabra);
            });

            b.Entity<DocumentoAutorizacion>(e =>
            {
                e.ToTable("DocumentosAutorizaciones");

                e.HasKey(x => x.Id);

                e.Property(x => x.Rol)
                    .HasConversion<string>()     // enum ⇄ string
                    .HasMaxLength(20)
                    .IsRequired();

                e.Property(x => x.Estado)
                    .HasMaxLength(20)
                    .IsRequired()
                    .HasDefaultValue("PENDIENTE");

                e.Property(x => x.Comentario)
                    .HasMaxLength(500);

                e.Property(x => x.Activo)
                    .HasDefaultValue(true);

                e.Property(x => x.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.Documento)
                    .WithMany(d => d.Autorizaciones)
                    .HasForeignKey(x => x.DocumentoId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.AutorizadoPor)
                    .WithMany()
                    .HasForeignKey(x => x.AutorizadoPorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.DocumentoId, x.Rol })
                    .IsUnique();
            });

        }

        //Intranet
        private static void BuildBanners(ModelBuilder b)
        {
            b.Entity<Banner>(e =>
            {
                e.ToTable("Banners");
                e.HasKey(x => x.Id);

                e.Property(x => x.Titulo).HasMaxLength(150);
                e.Property(x => x.Descripcion).HasMaxLength(500);

                e.Property(x => x.NombreArchivo).IsRequired().HasMaxLength(300);
                e.Property(x => x.RutaArchivo).IsRequired().HasMaxLength(500);

                e.Property(x => x.Activo).HasDefaultValue(true);
                e.Property(x => x.EsPermanente).HasDefaultValue(false);
                e.Property(x => x.Orden).HasDefaultValue(1);
                e.Property(x => x.FechaCreacion).HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.UsuarioCreador)
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioCreadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.Activo, x.EsPermanente, x.Orden });
                e.HasIndex(x => x.VigenciaInicio);
                e.HasIndex(x => x.VigenciaFin);
            });
        }

        //Header
        private static void BuildHeaderImagenes(ModelBuilder b)
        {
            b.Entity<ERPSEI.Data.Entities.Intranet.HeaderImagen>(e =>
            {
                e.ToTable("HeaderImagenes");
                e.HasKey(x => x.Id);

                e.Property(x => x.Temporada).IsRequired().HasMaxLength(80);
                e.Property(x => x.Titulo).HasMaxLength(150);
                e.Property(x => x.Descripcion).HasMaxLength(500);

                e.Property(x => x.NombreArchivo).IsRequired().HasMaxLength(300);
                e.Property(x => x.RutaArchivo).IsRequired().HasMaxLength(500);

                e.Property(x => x.Activo).HasDefaultValue(true);
                e.Property(x => x.EsPermanente).HasDefaultValue(false);
                e.Property(x => x.Orden).HasDefaultValue(1);
                e.Property(x => x.FechaCreacion).HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.UsuarioCreador)
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioCreadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.Activo, x.EsPermanente, x.Orden, x.Temporada });
                e.HasIndex(x => x.VigenciaInicio);
                e.HasIndex(x => x.VigenciaFin);
            });
        }

        //Políticas y manuales
        private void BuildManualesPoliticasIntranet(ModelBuilder builder)
        {
            builder.Entity<ManualPoliticaIntranet>(entity =>
            {
                entity.ToTable("ManualesPoliticasIntranet");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(1000);

                entity.Property(e => e.Tipo)
                    .HasMaxLength(50);

                entity.Property(e => e.ModoVisualizacion)
                    .HasMaxLength(50);

                entity.Property(e => e.CodigoHtml)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.UrlExterna)
                    .HasMaxLength(500);

                entity.Property(e => e.NombreArchivoPdf)
                    .HasMaxLength(250);

                entity.Property(e => e.RutaArchivoPdf)
                    .HasMaxLength(500);

                entity.Property(e => e.NombrePortada)
                    .HasMaxLength(250);

                entity.Property(e => e.RutaPortada)
                    .HasMaxLength(500);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.Publicado)
                    .HasDefaultValue(false);

                entity.Property(e => e.Orden)
                    .HasDefaultValue(1);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");
            });
        }


        private static void BuildTipoContratos(ModelBuilder b)
		{
			b.Entity<TipoContrato>().HasData(
				new TipoContrato { Id = 1, Nombre = "Asimilados", Descripcion = "Contratos de tipo asimilados a salarios", Deshabilitado = true },
                new TipoContrato { Id = 2, Nombre = "Asesoría", Descripcion = "", Deshabilitado = true },
                new TipoContrato { Id = 3, Nombre = "Servicios", Descripcion = "Prestación de servicios profesionales o técnicos", Deshabilitado = true },
				new TipoContrato { Id = 4, Nombre = "Uso de Marca", Descripcion = "Contrato por uso de marca registrada", Deshabilitado = true },
				new TipoContrato { Id = 5, Nombre = "Arrendamiento Act.", Descripcion = "Arrendamiento de activos generales", Deshabilitado = true },
				new TipoContrato { Id = 6, Nombre = "Arrendamiento TI", Descripcion = "Arrendamiento de tecnología e infraestructura", Deshabilitado = true },
				new TipoContrato { Id = 7, Nombre = "Arrendamiento Ofi.", Descripcion = "Arrendamiento de oficinas físicas", Deshabilitado = true }
            );

            b.Entity<SubTipoContrato>().HasData(
				new SubTipoContrato { Id = 1, Nombre = "Servicios de diseño de presentaciones", Descripcion = "", Deshabilitado = false, TipoContratoId = 3 },
				new SubTipoContrato { Id = 2, Nombre = "Servicios de evaluación de clientes y proveedores", Descripcion = "", Deshabilitado = false, TipoContratoId = 3 },
				new SubTipoContrato { Id = 3, Nombre = "Servicios profesionales", Descripcion = "", Deshabilitado = false, TipoContratoId = 3 },
                new SubTipoContrato { Id = 4, Nombre = "Servicios profesionales independientes", Descripcion = "", Deshabilitado = false, TipoContratoId = 3 },
                new SubTipoContrato { Id = 5, Nombre = "Servicios (“El Contrato”)", Descripcion = "", Deshabilitado = false, TipoContratoId = 3 },
                new SubTipoContrato { Id = 6, Nombre = "Servicios profesionales de asesoria en inversiones", Descripcion = "", Deshabilitado = false, TipoContratoId = 3 },
                new SubTipoContrato { Id = 7, Nombre = "Servicios profesionales de mantenimiento de software", Descripcion = "", Deshabilitado = false, TipoContratoId = 3 },
                new SubTipoContrato { Id = 8, Nombre = "Servicios profesionales de integración de expedientes para licitaciones", Descripcion = "", Deshabilitado = false, TipoContratoId = 3 },
                new SubTipoContrato { Id = 9, Nombre = "Servicios profesionales de asesoria legal", Descripcion = "", Deshabilitado = false, TipoContratoId = 3 },
                new SubTipoContrato { Id = 10, Nombre = "Asesoria Financiera", Descripcion = "", Deshabilitado = false, TipoContratoId = 2 },
                new SubTipoContrato { Id = 11, Nombre = "Asesoria en Recursos Humanos", Descripcion = "", Deshabilitado = false, TipoContratoId = 2 },
                new SubTipoContrato { Id = 12, Nombre = "Asesoría financiera y revisión fiscal", Descripcion = "", Deshabilitado = false, TipoContratoId = 2 },
                new SubTipoContrato { Id = 13, Nombre = "Capacitación y asesoria por la venta a clientes", Descripcion = "", Deshabilitado = false, TipoContratoId = 2 }

            );

            b.Entity<TipoRepresentacion>().HasData(
				new TipoRepresentacion { Id = 1, Nombre = "Representante Legal"},
				new TipoRepresentacion { Id = 2, Nombre = "Apoderado Legal"}
);


            // Relación EmpresaContrato -> TipoContrato (muchos a uno)
            b.Entity<EmpresaContrato>()
                .HasOne(ec => ec.TipoContrato)
                .WithMany() // Si deseas una lista de empresas en TipoContrato, aquí puedes usar `.WithMany(tc => tc.Empresas)`
                .HasForeignKey(ec => ec.TipoContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación ClienteContrato -> EmpresaContrato (muchos a uno)
            b.Entity<ClienteContrato>()
                .HasOne(cc => cc.EmpresaContrato)
                .WithMany(ec => ec.Clientes)
                .HasForeignKey(cc => cc.EmpresaContratoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ClienteContrato → TipoContrato
            b.Entity<ClienteContrato>()
                .HasOne(cc => cc.TipoContrato)
                .WithMany()
                .HasForeignKey(cc => cc.TipoContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            // HistorialContratoGenerado -> EmpresaContrato
            b.Entity<HistorialContratoGenerado>()
                .HasOne(h => h.EmpresaContrato)
                .WithMany()
                .HasForeignKey(h => h.EmpresaContratoId)
                .OnDelete(DeleteBehavior.NoAction);

            // HistorialContratoGenerado -> ClienteContrato
            b.Entity<HistorialContratoGenerado>()
                .HasOne(h => h.ClienteContrato)
                .WithMany()
                .HasForeignKey(h => h.ClienteContratoId)
                .OnDelete(DeleteBehavior.NoAction);

            /*b.Entity<EmpresaContrato>()
                .HasOne(e => e.TipoRepresentacion)
                .WithMany()
                .HasForeignKey(e => e.TipoRepresentacionId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<ClienteContrato>()
                .HasOne(c => c.TipoRepresentacion)
                .WithMany()
                .HasForeignKey(c => c.TipoRepresentacionId)
                .OnDelete(DeleteBehavior.Restrict);*/



        }

        private static void BuildActivosFijos(ModelBuilder b)
        {
            // Relación: ActivoFijo -> CategoriaActivoFijo
            b.Entity<ActivoFijo>()
                .HasOne(a => a.Categoria)
                .WithMany(c => c.ActivosFijos)
                .HasForeignKey(a => a.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: ActivoFijo -> TipoActivoFijo
            b.Entity<ActivoFijo>()
                .HasOne(a => a.Tipo)
                .WithMany(t => t.ActivosFijos)
                .HasForeignKey(a => a.TipoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: ActivoFijo -> Empleado
            b.Entity<ActivoFijo>()
                .HasOne(a => a.Empleado)
                .WithMany() // Si agregas navegación en Empleado, puedes usar .WithMany(e => e.ActivosAsignados)
                .HasForeignKey(a => a.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Conversión booleana a int en BD para compatibilidad
            b.Entity<CategoriaActivoFijo>()
                .Property(c => c.Deshabilitado)
                .HasConversion<int>();

            b.Entity<TipoActivoFijo>()
                .Property(t => t.Deshabilitado)
                .HasConversion<int>();

            b.Entity<TipoActivoFijo>()
                .Property(t => t.PermiteMultiplesAsignaciones)
                .HasConversion<int>();

            b.Entity<ActivoFijo>()
                .Property(a => a.Deshabilitado)
                .HasConversion<int>();

            // Datos iniciales para Categorías
            b.Entity<CategoriaActivoFijo>().HasData(
                new CategoriaActivoFijo { Id = 1, Descripcion = "Software", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 2, Descripcion = "Hardware", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 3, Descripcion = "Inmobiliario", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 4, Descripcion = "Archiveros", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 5, Descripcion = "Escritorios y mesas", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 6, Descripcion = "Estaciones", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 7, Descripcion = "Extintores", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 8, Descripcion = "Línea blanca", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 9, Descripcion = "Pingüinos y ventiladores", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 10, Descripcion = "Sillas", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 11, Descripcion = "Decoración", Deshabilitado = false },
                new CategoriaActivoFijo { Id = 12, Descripcion = "Otros ", Deshabilitado = false }

            );

            // Datos iniciales para Tipos
            b.Entity<TipoActivoFijo>().HasData(
                new TipoActivoFijo { Id = 1, Descripcion = "Laptop", PermiteMultiplesAsignaciones = false, Deshabilitado = false },
                new TipoActivoFijo { Id = 2, Descripcion = "Monitor", PermiteMultiplesAsignaciones = false, Deshabilitado = false },
                new TipoActivoFijo { Id = 3, Descripcion = "Licencia", PermiteMultiplesAsignaciones = true, Deshabilitado = false },
                new TipoActivoFijo { Id = 4, Descripcion = "Programa", PermiteMultiplesAsignaciones = true, Deshabilitado = false },
                new TipoActivoFijo { Id = 5, Descripcion = "Mesa", PermiteMultiplesAsignaciones = false, Deshabilitado = false },
                new TipoActivoFijo { Id = 6, Descripcion = "Silla", PermiteMultiplesAsignaciones = false, Deshabilitado = false },
                new TipoActivoFijo { Id = 7, Descripcion = "Escritorio", PermiteMultiplesAsignaciones = false, Deshabilitado = false },
                new TipoActivoFijo { Id = 8, Descripcion = "Unidad de Almacenamiento", PermiteMultiplesAsignaciones = false, Deshabilitado = false }
            );

            // Relación: ArchivoActivoFijo -> ActivoFijo
            b.Entity<ArchivoActivoFijo>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.ActivoFijo)
                      .WithMany(a => a.Archivos)
                      .HasForeignKey(e => e.ActivoFijoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.NombreArchivo)
                      .IsRequired()
                      .HasMaxLength(300);

                entity.Property(e => e.Extension)
                      .HasMaxLength(10);

                entity.Property(e => e.MimeType)
                      .HasMaxLength(50);

                entity.Property(e => e.RutaArchivo)
                      .HasMaxLength(500);
            });
        }

        private static void BuildVacaciones(ModelBuilder b)
        {
            // === PeriodoVacacional - Empleado (1:N) ===
            b.Entity<PeriodoVacacional>()
                .HasOne(p => p.Empleado)
                .WithMany(e => e.PeriodosVacacionales)
                .HasForeignKey(p => p.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict); // Evita cascada

            // === SolicitudVacaciones - Empleado solicitante (1:N) ===
            b.Entity<SolicitudVacaciones>()
                .HasOne(s => s.Empleado)
                .WithMany(e => e.SolicitudesVacaciones)
                .HasForeignKey(s => s.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict); // Evita cascada

            // === SolicitudVacaciones - Autorizador (Empleado) (1:N) ===
            b.Entity<SolicitudVacaciones>()
                .HasOne(s => s.Autorizador)
                .WithMany(e => e.SolicitudesAutorizadas)
                .HasForeignKey(s => s.AutorizadorId)
                .OnDelete(DeleteBehavior.Restrict); // Evita ciclos de cascada

            // === HistorialVacaciones - Empleado (1:N) ===
            b.Entity<HistorialVacaciones>()
                .HasOne(h => h.Empleado)
                .WithMany(e => e.HistorialVacaciones)
                .HasForeignKey(h => h.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict); // Evita cascada

            // === HistorialVacaciones - SolicitudVacaciones (1:N) ===
            b.Entity<HistorialVacaciones>()
                .HasOne(h => h.Solicitud)
                .WithMany(s => s.Historiales)
                .HasForeignKey(h => h.SolicitudVacacionesId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<PoliticaVacacion>()
                .HasMany(p => p.Detalles)
                .WithOne(d => d.PoliticaVacacion)
                .HasForeignKey(d => d.PoliticaVacacionId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<PoliticaVacacion>().HasData(
                new PoliticaVacacion
                {
                    Id = 1,
                    Nombre = "Legal 2023",
                    TipoVacacion = "Legales",
                    Descripcion = "Política legal vigente 2023",
                    Activo = true,
                    FechaCreacion = new DateTime(2026, 3, 17)
                },
                new PoliticaVacacion
                {
                    Id = 2,
                    Nombre = "Anual 2023",
                    TipoVacacion = "Anuales",
                    Descripcion = "Política anual interna 2023",
                    Activo = true,
                    FechaCreacion = new DateTime(2026, 3, 17)
                }
            );

            b.Entity<PoliticaVacacionDetalle>().HasData(
                new PoliticaVacacionDetalle { Id = 1, PoliticaVacacionId = 1, AniosAntiguedad = 1.0m, DiasVacaciones = 12.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 1 },
                new PoliticaVacacionDetalle { Id = 2, PoliticaVacacionId = 1, AniosAntiguedad = 2.0m, DiasVacaciones = 14.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 2 },
                new PoliticaVacacionDetalle { Id = 3, PoliticaVacacionId = 1, AniosAntiguedad = 3.0m, DiasVacaciones = 16.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 3 },
                new PoliticaVacacionDetalle { Id = 4, PoliticaVacacionId = 1, AniosAntiguedad = 4.0m, DiasVacaciones = 18.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 4 },
                new PoliticaVacacionDetalle { Id = 5, PoliticaVacacionId = 1, AniosAntiguedad = 5.0m, DiasVacaciones = 20.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 5 },
                new PoliticaVacacionDetalle { Id = 6, PoliticaVacacionId = 1, AniosAntiguedad = 6.0m, DiasVacaciones = 22.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 6 },
                new PoliticaVacacionDetalle { Id = 7, PoliticaVacacionId = 1, AniosAntiguedad = 11.0m, DiasVacaciones = 24.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 7 },
                new PoliticaVacacionDetalle { Id = 8, PoliticaVacacionId = 1, AniosAntiguedad = 16.0m, DiasVacaciones = 26.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 8 },
                new PoliticaVacacionDetalle { Id = 9, PoliticaVacacionId = 1, AniosAntiguedad = 21.0m, DiasVacaciones = 28.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 9 },
                new PoliticaVacacionDetalle { Id = 10, PoliticaVacacionId = 1, AniosAntiguedad = 26.0m, DiasVacaciones = 30.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 10 },
                new PoliticaVacacionDetalle { Id = 11, PoliticaVacacionId = 1, AniosAntiguedad = 31.0m, DiasVacaciones = 32.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 11 },
                new PoliticaVacacionDetalle { Id = 12, PoliticaVacacionId = 1, AniosAntiguedad = 36.0m, DiasVacaciones = 34.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 12 },

                new PoliticaVacacionDetalle { Id = 13, PoliticaVacacionId = 2, AniosAntiguedad = 1.0m, DiasVacaciones = 12.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 1 },
                new PoliticaVacacionDetalle { Id = 14, PoliticaVacacionId = 2, AniosAntiguedad = 2.0m, DiasVacaciones = 12.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 2 },
                new PoliticaVacacionDetalle { Id = 15, PoliticaVacacionId = 2, AniosAntiguedad = 3.0m, DiasVacaciones = 12.0m, PrimaVacacional = 0.25m, DiasAguinaldo = 15.0m, Orden = 3 }
            );

            b.Entity<ConfiguracionVacacion>().HasData(
                new ConfiguracionVacacion
                {
                    Id = 1,
                    TipoVisualizacion = "LegalesProporcionales",
                    FechaActualizacion = new DateTime(2026, 3, 17)
                }
            );

            b.Entity<HistorialVacacionVencida>()
                .HasOne(x => x.Empleado)
                .WithMany()
                .HasForeignKey(x => x.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<HistorialVacacionVencida>()
                .Property(x => x.DiasVencidos)
                .HasPrecision(10, 2);

            b.Entity<HistorialVacacionVencida>()
                .Property(x => x.Causa)
                .HasMaxLength(500);

            b.Entity<HistorialVacacionVencida>()
                .Property(x => x.Periodo)
                .HasMaxLength(150);
        }

        private static void BuildPolizas(ModelBuilder b) 
		{
			b.Entity<GrupoPoliza>().HasMany(p => p.Polizas).WithOne(p => p.Grupo).OnDelete(DeleteBehavior.NoAction);
			b.Entity<GrupoPoliza>().HasOne(p => p.UsuarioCreador).WithMany(p => p.GruposPolizasCreados).OnDelete(DeleteBehavior.NoAction);
			b.Entity<GrupoPoliza>().HasOne(p => p.UsuarioModificador).WithMany(p => p.GruposPolizasModificados).OnDelete(DeleteBehavior.NoAction);

			b.Entity<VPoliza>().HasOne(p => p.Tipo).WithMany(p => p.Polizas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<VPoliza>().HasMany(p => p.PolizasDetalles).WithOne(p => p.Poliza).OnDelete(DeleteBehavior.NoAction);

			b.Entity<PolizaDetalle>().HasOne(p => p.Cuenta).WithMany(p => p.PolizasDetalles).OnDelete(DeleteBehavior.NoAction);

			b.Entity<PolizaDetalle>().Property(t => t.Debe).HasPrecision(24, 6);
			b.Entity<PolizaDetalle>().Property(t => t.Haber).HasPrecision(24, 6);
		}

		private static void BuildAsistencias(ModelBuilder b) 
		{
			b.Entity<Asistencia>().HasOne(e => e.Empleado).WithMany(a => a.Asistencias).OnDelete(DeleteBehavior.NoAction);

			b.Entity<Empleado>().HasOne(e => e.Horario).WithMany(h => h.Empleados).OnDelete(DeleteBehavior.NoAction);

			b.Entity<Horario>().HasMany(h => h.HorarioDetalles).WithOne(hd => hd.Horario).OnDelete(DeleteBehavior.NoAction);
		}

		private static void BuildConciliaciones(ModelBuilder b) 
		{
            b.Entity<Conciliacion>().HasOne(e => e.Banco).WithMany(a => a.Conciliaciones).OnDelete(DeleteBehavior.NoAction);
            b.Entity<Conciliacion>().HasOne(e => e.UsuarioCreador).WithMany(a => a.ConciliacionesCreadas).OnDelete(DeleteBehavior.NoAction);
            b.Entity<Conciliacion>().HasOne(e => e.UsuarioModificador).WithMany(a => a.ConciliacionesModificadas).OnDelete(DeleteBehavior.NoAction);
            b.Entity<Conciliacion>().HasOne(e => e.Empresa).WithMany(a => a.Conciliaciones).OnDelete(DeleteBehavior.NoAction);
            b.Entity<Conciliacion>().HasMany(e => e.DetallesConciliacion).WithOne(a => a.Conciliacion).OnDelete(DeleteBehavior.NoAction);
            b.Entity<Conciliacion>().HasOne(e => e.Cliente).WithMany(a => a.Conciliaciones).OnDelete(DeleteBehavior.NoAction);
            b.Entity<ConciliacionDetalle>().HasMany(e => e.ConciliacionesDetallesMovimientos).WithOne(a => a.ConciliacionDetalle).OnDelete(DeleteBehavior.NoAction);
            b.Entity<ConciliacionDetalle>().HasMany(e => e.ConciliacionesDetallesComprobantes).WithOne(a => a.ConciliacionDetalle).OnDelete(DeleteBehavior.NoAction);
            b.Entity<ConciliacionDetalleComprobante>().HasOne(e => e.Comprobante).WithOne(a => a.ConciliacionDetalleComprobante).OnDelete(DeleteBehavior.NoAction);
            b.Entity<ConciliacionDetalleMovimiento>().HasOne(e => e.MovimientoBancario).WithOne(a => a.ConciliacionDetalleMovimiento).OnDelete(DeleteBehavior.NoAction);

            b.Entity<Conciliacion>().Property(t => t.Total).HasPrecision(24, 6);
            b.Entity<MovimientoBancario>().Property(t => t.Importe).HasPrecision(24, 6);

            b.Entity<Banco>()
                .HasData(
                    new Banco() {Id = 1, Nombre = "Alquimia" },
                    new Banco() { Id = 2, Nombre = "Afirme" },
					new Banco() { Id = 3, Nombre = "Autofin" },//
					new Banco() { Id = 4, Nombre = "Azteca" },
                    new Banco() { Id = 5, Nombre = "American Express" },
                    new Banco() { Id = 6, Nombre = "Bancomer" },
                    new Banco() { Id = 7, Nombre = "Bancoppel" },
					new Banco() { Id = 8, Nombre = "Banamex" },//
					new Banco() { Id = 9, Nombre = "Bankaool" },//
					new Banco() { Id = 10, Nombre = "Banorte" },
                    new Banco() { Id = 11, Nombre = "Banregio" },
                    new Banco() { Id = 12, Nombre = "Bajio" },
					new Banco() { Id = 13, Nombre = "Banbajio" },
					new Banco() { Id = 14, Nombre = "Base" },
					new Banco() { Id = 15, Nombre = "BBVA" },
					new Banco() { Id = 16, Nombre = "Bx" },
                    new Banco() { Id = 17, Nombre = "Cibanco" },
                    new Banco() { Id = 18, Nombre = "Citibanamex" },
					new Banco() { Id = 19, Nombre = "Eplata" },//
					new Banco() { Id = 20, Nombre = "Fortuna" },
                    new Banco() { Id = 21, Nombre = "HSBC" },
                    new Banco() { Id = 22, Nombre = "Inbursa" },
                    new Banco() { Id = 23, Nombre = "Intercam" },
                    new Banco() { Id = 24, Nombre = "Invex" },
                    new Banco() { Id = 25, Nombre = "Jeeves" },
					new Banco() { Id = 26, Nombre = "KLU" },//
					new Banco() { Id = 27, Nombre = "Konfio" },
                    new Banco() { Id = 28, Nombre = "Mercado Pago" },
                    new Banco() { Id = 29, Nombre = "Mifel" },
                    new Banco() { Id = 30, Nombre = "Monex" },
                    new Banco() { Id = 31, Nombre = "Multiva" },
					new Banco() { Id = 32, Nombre = "PayMax" },//
					new Banco() { Id = 33, Nombre = "Santander" },
                    new Banco() { Id = 34, Nombre = "Scotiabank" },
					new Banco() { Id = 35, Nombre = "SantanderDig" }
					
				);
        }

        private static void BuildEmpresas(ModelBuilder b) 
		{
			b.Entity<Empresa>().HasOne(e => e.Perfil).WithMany(p => p.Empresas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empresa>().HasOne(e => e.Origen).WithMany(o => o.Empresas).OnDelete(DeleteBehavior.NoAction);
            b.Entity<Empresa>().HasOne(e => e.Nivel).WithMany(o => o.Empresas).OnDelete(DeleteBehavior.NoAction);;
			b.Entity<Empresa>().HasMany(e => e.BancosEmpresa).WithOne(b => b.Empresa).OnDelete(DeleteBehavior.NoAction);
            b.Entity<Empresa>().HasMany(e => e.ArchivosEmpresa).WithOne(a => a.Empresa).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empresa>().HasMany(e => e.ActividadesEconomicasEmpresa).WithOne(a => a.Empresa).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empresa>().HasOne(e => e.RegimenFiscal).WithMany(f => f.Empresas).OnDelete(DeleteBehavior.NoAction);

			b.Entity<BancoEmpresa>().Property(b => b.Limite).HasPrecision(18, 2);

			b.Entity<ActividadEconomica>().HasMany(a => a.ActividadesEconomicasEmpresa).WithOne(a => a.ActividadEconomica).OnDelete(DeleteBehavior.NoAction);

			b.Entity<ArchivoEmpresa>().HasOne(a => a.TipoArchivo).WithMany(ta => ta.ArchivosEmpresa).OnDelete(DeleteBehavior.NoAction);

			b.Entity<Perfil>().HasMany(p => p.ProductosServiciosPerfil).WithOne(p => p.Perfil).OnDelete(DeleteBehavior.NoAction);

			b.Entity<ProductoServicio>().HasMany(p => p.ProductosServiciosPerfil).WithOne(p => p.ProductoServicio).OnDelete(DeleteBehavior.NoAction);

            b.Entity<TipoArchivoEmpresa>()
            .HasData(
                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.CSF,
                    "CSF"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.INE,
                    "INE"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.RFC,
                    "RFC"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.ComprobanteDomicilio,
                    "ComprobanteDomicilio"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.Otro,
                    "Otro"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.CER,
                    "CER"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.KEY,
                    "KEY"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.Logo,
                    "Logo"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.HojaMembretada,
                    "HojaMembretada"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.INE2,
                    "INE2"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.INE3,
                    "INE3"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.Organigrama,
                    "Organigrama"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.ActaConstitutiva,
                    "ActaConstitutiva"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.ActasAdicionales,
                    "ActasAdicionales"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.PoderNotarial,
                    "PoderNotarial"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.INEAccionistas,
                    "INEAccionistas"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.CSFAccionistas,
                    "CSFAccionistas"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.ComprobanteDomicilioAccionistas,
                    "ComprobanteDomicilioAccionistas"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.DeclaracionAnualMensual,
                    "DeclaracionAnualMensual"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.OpinionCumplimientoSAT,
                    "OpinionCumplimientoSAT"
                ),

                new TipoArchivoEmpresa(
                    (int)Entities.Empresas.FileTypes.PruebaVida,
                    "PruebaVida"
                )
            );
        }

		private static void BuildEmpleados(ModelBuilder b)
		{
			b.Entity<ArchivoEmpleado>().HasOne(ae => ae.TipoArchivo).WithMany(ta => ta.ArchivosEmpleado).OnDelete(DeleteBehavior.NoAction);

			b.Entity<Empleado>().HasOne(e => e.EstadoCivil).WithMany(ec => ec.Empleados).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empleado>().HasOne(e => e.Genero).WithMany(g => g.Empleados).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empleado>().HasOne(e => e.Puesto).WithMany(p => p.Empleados).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empleado>().HasOne(e => e.Area).WithMany(a => a.Empleados).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empleado>().HasOne(e => e.Subarea).WithMany(sa => sa.Empleados).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empleado>().HasOne(e => e.Oficina).WithMany(o => o.Empleados).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empleado>().HasMany(e => e.ContactosEmergencia).WithOne(ce => ce.Empleado).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Empleado>().HasMany(e => e.ArchivosEmpleado).WithOne(ae => ae.Empleado).OnDelete(DeleteBehavior.NoAction);

            b.Entity<Empleado>()
            .HasOne(e => e.Jefe)
            .WithMany(j => j.Subordinados)
            .HasForeignKey(e => e.JefeId)
            .OnDelete(DeleteBehavior.Restrict);


            b.Entity<TipoArchivo>()
            .HasData(
                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.ImagenPerfil,
                    "Imagen de perfil"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.ActaNacimiento,
                    "Acta de nacimiento"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.CURP,
                    "CURP"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.CLABE,
                    "CLABE"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.ComprobanteDomicilio,
                    "Comprobante de domicilio"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.CSF,
                    "CSF"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.INE,
                    "INE"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.RFC,
                    "RFC"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.ComprobanteEstudios,
                    "Comprobante de estudios"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.NSS,
                    "NSS"
                ),

                new TipoArchivo(
                    (int)Entities.Empleados.FileTypes.Otro,
                    "Otro"
                )
            );

            b.Entity<EstadoCivil>()
				.HasData(
					new EstadoCivil() { Id = 1, Nombre = "Soltero" },
					new EstadoCivil() { Id = 2, Nombre = "Casado" }
				);

			b.Entity<Genero>()
				.HasData(
					new Genero() { Id = 1, Nombre = "Masculino" },
					new Genero() { Id = 2, Nombre = "Femenino" },
					new Genero() { Id = 3, Nombre = "Otro" }
				);

			List<string> puestos = new List<string>()
			{
				"Analista",
				"Asistente",
				"Auditor",
				"Auxiliar",
				"Chofer",
				"Desarrollador",
				"Director",
				"Socio Director",
				"Encargado",
				"Gerente",
				"Mantenimiento y Limpieza",
				"Recepcionista",
				"Recepcionista Coordinadora",
				"Seguridad Privada",
				"Socio",
				"Subencargado",
				"Subgerente",
				"Supervisor",
				"Técnico",
				"Tesorero"
			};
			Puesto[] dataPuestos = new Puesto[puestos.Count];
			int i = 0;
			foreach (string puesto in puestos)
			{
				dataPuestos[i] = new Puesto() { Id = i + 1, Nombre = puesto };
				i++;
			}
			b.Entity<Puesto>().HasData(dataPuestos);

			List<KeyValuePair<string, List<string>>> areas = new List<KeyValuePair<string, List<string>>>()
			{
				new KeyValuePair<string, List<string>>("Administración", new List<string>(){"Sistemas"}),
				new KeyValuePair<string, List<string>>("Auditoría", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Bancos", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Contabilidad", new List<string>(){"Interna", "Externa", "Impuestos"}),
				new KeyValuePair<string, List<string>>("Dirección General", new List<string>(){"Control Vehicular"}),
				new KeyValuePair<string, List<string>>("Expedientes", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Family Office", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Fiscal", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Impuestos", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Legal", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Nóminas", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Operaciones", new List<string>(){"IMSS", "Internas", "Facturación", "Nóminas"}),
				new KeyValuePair<string, List<string>>("Recursos Humanos", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Tesorería", new List<string>(){ }),
				new KeyValuePair<string, List<string>>("Socio", new List<string>() { })
			};

			Area[] dataAreas = new Area[areas.Count];
			List<Subarea> dataSubareas = new List<Subarea>();
			int j = 0;
			int k = 0;
			foreach (KeyValuePair<string, List<string>> area in areas)
			{
				dataAreas[j] = new Area() { Id = j + 1, Nombre = area.Key };
				//Se agregan las subareas
				foreach (string subarea in area.Value)
				{
					dataSubareas.Add(new Subarea() { Id = k + 1, Nombre = subarea, AreaId = dataAreas[j].Id });
					k++;
				}
				j++;
			}
			b.Entity<Area>().HasData(dataAreas);
			b.Entity<Area>().HasMany(a => a.Subareas).WithOne(sa => sa.Area).OnDelete(DeleteBehavior.NoAction);

			b.Entity<Subarea>().HasData(dataSubareas.ToArray());

			List<string> oficinas = new List<string>()
			{
				"Austria 1",
				"Austria 6",
				"Big Ben",
				"Cancún",
				"Capri",
				"Centro Urbano",
				"Cóndor",
				"Izaguirre",
				"Lago de Guadalupe",
				"León",
				"Lomas Verdes",
				"Los Reyes La Paz",
				"Pafnuncio",
				"Pirules",
				"Polanco",
				"Santa Mónica",
				"Torre Esmeralda"
			};
			Oficina[] dataOficinas = new Oficina[oficinas.Count];
			int l = 0;
			foreach (string oficina in oficinas)
			{
				dataOficinas[l] = new Oficina() { Id = l + 1, Nombre = oficina };
				l++;
			}
			b.Entity<Oficina>().HasData(dataOficinas);
		}

		private static void BuildSAT(ModelBuilder b)
		{
			b.Entity<TasaOCuota>().HasOne(t => t.Factor).WithMany(f => f.TasasOCuotas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<TasaOCuota>().HasOne(t => t.Impuesto).WithMany(i => i.TasasOCuotas).OnDelete(DeleteBehavior.NoAction);

			b.Entity<TipoComprobante>().Property(t => t.ValorMaximo).HasPrecision(24, 6);

			b.Entity<Prefactura>().HasMany(p => p.Conceptos).WithOne(c => c.Prefactura).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.Emisor).WithMany(e => e.PrefacturasEmitidas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.Receptor).WithMany(e => e.PrefacturasRecibidas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.TipoComprobante).WithMany(e => e.Prefacturas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.Moneda).WithMany(e => e.Prefacturas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.FormaPago).WithMany(e => e.Prefacturas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.MetodoPago).WithMany(e => e.Prefacturas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.UsoCFDI).WithMany(e => e.Prefacturas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.Exportacion).WithMany(e => e.Prefacturas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.UsuarioCreador).WithMany(e => e.PrefacturasCreadas).OnDelete(DeleteBehavior.NoAction);
            b.Entity<Prefactura>().HasOne(p => p.UsuarioTimbrador).WithMany(e => e.PrefacturasTimbradas).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Prefactura>().HasOne(p => p.Estatus).WithMany(e => e.Prefacturas).OnDelete(DeleteBehavior.NoAction);
            b.Entity<Prefactura>().Property(c => c.TipoCambio).HasPrecision(18, 6);

			b.Entity<Concepto>().HasOne(c => c.UnidadMedida).WithMany(e => e.Conceptos).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Concepto>().HasOne(c => c.ObjetoImpuesto).WithMany(e => e.Conceptos).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Concepto>().Property(c => c.PrecioUnitario).HasPrecision(18, 6);
			b.Entity<Concepto>().Property(c => c.Descuento).HasPrecision(18, 6);
            b.Entity<Concepto>().Property(c => c.TasaTraslado).HasPrecision(18, 6);
            b.Entity<Concepto>().Property(c => c.TasaRetencion).HasPrecision(18, 6);
            b.Entity<Concepto>().Property(c => c.Traslado).HasPrecision(18, 6);
			b.Entity<Concepto>().Property(c => c.Retencion).HasPrecision(18, 6);

			b.Entity<AutorizacionesPrefactura>().HasOne(ap => ap.Prefactura).WithMany(p => p.Autorizaciones).OnDelete(DeleteBehavior.NoAction);
			b.Entity<AutorizacionesPrefactura>().HasOne(ap => ap.Usuario).WithMany(p => p.AutorizacionesPrefacturas).OnDelete(DeleteBehavior.NoAction);

            b.Entity<EstatusPrefactura>()
                .HasData(
                    new EstatusPrefactura() { Id = 1, Descripcion = "Solicitada" },
                    new EstatusPrefactura() { Id = 2, Descripcion = "Autorizada" },
                    new EstatusPrefactura() { Id = 3, Descripcion = "Timbrada" }
                );

			b.Entity<Comprobante>().Property(c => c.Descuento).HasPrecision(18, 6);
			b.Entity<Comprobante>().Property(c => c.SubTotal).HasPrecision(18, 6);
			b.Entity<Comprobante>().Property(c => c.TipoCambio).HasPrecision(18, 6);
			b.Entity<Comprobante>().Property(c => c.Total).HasPrecision(18, 6);

			b.Entity<ComprobanteConcepto>().Property(c => c.Cantidad).HasPrecision(18, 6);
			b.Entity<ComprobanteConcepto>().Property(c => c.Descuento).HasPrecision(18, 6);
			b.Entity<ComprobanteConcepto>().Property(c => c.Importe).HasPrecision(18, 6);
			b.Entity<ComprobanteConcepto>().Property(c => c.ValorUnitario).HasPrecision(18, 6);

			b.Entity<ComprobanteConceptoImpuestosRetencion>().Property(c => c.Base).HasPrecision(18, 6);
			b.Entity<ComprobanteConceptoImpuestosRetencion>().Property(c => c.Importe).HasPrecision(18, 6);
			b.Entity<ComprobanteConceptoImpuestosRetencion>().Property(c => c.TasaOCuota).HasPrecision(18, 6);

			b.Entity<ComprobanteConceptoImpuestosTraslado>().Property(c => c.Base).HasPrecision(18, 6);
			b.Entity<ComprobanteConceptoImpuestosTraslado>().Property(c => c.Importe).HasPrecision(18, 6);
			b.Entity<ComprobanteConceptoImpuestosTraslado>().Property(c => c.TasaOCuota).HasPrecision(18, 6);

			b.Entity<ComprobanteConceptoParte>().Property(c => c.Cantidad).HasPrecision(18, 6);
			b.Entity<ComprobanteConceptoParte>().Property(c => c.Importe).HasPrecision(18, 6);
			b.Entity<ComprobanteConceptoParte>().Property(c => c.ValorUnitario).HasPrecision(18, 6);

			b.Entity<ComprobanteImpuestos>().Property(c => c.TotalImpuestosRetenidos).HasPrecision(18, 6);
			b.Entity<ComprobanteImpuestos>().Property(c => c.TotalImpuestosTrasladados).HasPrecision(18, 6);

			b.Entity<ComprobanteImpuestosRetencion>().Property(c => c.Importe).HasPrecision(18, 6);

			b.Entity<ComprobanteImpuestosTraslado>().Property(c => c.Base).HasPrecision(18, 6);
			b.Entity<ComprobanteImpuestosTraslado>().Property(c => c.TasaOCuota).HasPrecision(18, 6);
			b.Entity<ComprobanteImpuestosTraslado>().Property(c => c.Importe).HasPrecision(18, 6);

			b.Entity<Nomina>().Property(n => n.NumDiasPagados).HasPrecision(18, 6);
			b.Entity<Nomina>().Property(n => n.TotalDeducciones).HasPrecision(18, 6);
			b.Entity<Nomina>().Property(n => n.TotalPercepciones).HasPrecision(18, 6);
			b.Entity<Nomina>().Property(n => n.Version).HasPrecision(18, 6);
			b.Entity<Nomina>().Property(n => n.TotalOtrosPagos).HasPrecision(18, 6);
			b.Entity<NominaDeducciones>().Property(n => n.TotalImpuestosRetenidos).HasPrecision(18, 6);
			b.Entity<NominaDeducciones>().Property(n => n.TotalOtrasDeducciones).HasPrecision(18, 6);
			b.Entity<NominaDeduccionesDeduccion>().Property(n => n.Importe).HasPrecision(18, 6);
			b.Entity<NominaEmisorEntidadSNCF>().Property(p => p.MontoRecursoPropio).HasPrecision(18, 6);
			b.Entity<NominaIncapacidad>().Property(p => p.ImporteMonetario).HasPrecision(18, 6);
			b.Entity<NominaOtroPago>().Property(p => p.Importe).HasPrecision(18, 6);
			b.Entity<NominaOtroPagoCompensacionSaldosAFavor>().Property(p => p.SaldoAFavor).HasPrecision(18, 6);
			b.Entity<NominaOtroPagoCompensacionSaldosAFavor>().Property(p => p.RemanenteSalFav).HasPrecision(18, 6);
			b.Entity<NominaOtroPagoSubsidioAlEmpleo>().Property(p => p.SubsidioCausado).HasPrecision(18, 6);
			b.Entity<NominaPercepciones>().Property(p => p.TotalSueldos).HasPrecision(18, 6);
			b.Entity<NominaPercepciones>().Property(p => p.TotalSeparacionIndemnizacion).HasPrecision(18, 6);
			b.Entity<NominaPercepciones>().Property(p => p.TotalJubilacionPensionRetiro).HasPrecision(18, 6);
			b.Entity<NominaPercepciones>().Property(p => p.TotalExento).HasPrecision(18, 6);
			b.Entity<NominaPercepciones>().Property(p => p.TotalGravado).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesJubilacionPensionRetiro>().Property(p => p.TotalUnaExhibicion).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesJubilacionPensionRetiro>().Property(p => p.TotalParcialidad).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesJubilacionPensionRetiro>().Property(p => p.MontoDiario).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesJubilacionPensionRetiro>().Property(p => p.IngresoAcumulable).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesJubilacionPensionRetiro>().Property(p => p.IngresoNoAcumulable).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesPercepcion>().Property(n => n.ImporteExento).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesPercepcion>().Property(n => n.ImporteGravado).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesPercepcionAccionesOTitulos>().Property(p => p.ValorMercado).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesPercepcionAccionesOTitulos>().Property(p => p.PrecioAlOtorgarse).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesPercepcionHorasExtra>().Property(p => p.ImportePagado).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesSeparacionIndemnizacion>().Property(p => p.TotalPagado).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesSeparacionIndemnizacion>().Property(p => p.UltimoSueldoMensOrd).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesSeparacionIndemnizacion>().Property(p => p.IngresoAcumulable).HasPrecision(18, 6);
			b.Entity<NominaPercepcionesSeparacionIndemnizacion>().Property(p => p.IngresoNoAcumulable).HasPrecision(18, 6);
			b.Entity<NominaReceptor>().Property(p => p.SalarioBaseCotApor).HasPrecision(18, 6);
			b.Entity<NominaReceptor>().Property(p => p.SalarioDiarioIntegrado).HasPrecision(18, 6);
			b.Entity<NominaReceptorSubContratacion>().Property(p => p.PorcentajeTiempo).HasPrecision(18, 6);

			b.Entity<PagosPago>().Property(p => p.TipoCambioP).HasPrecision(18, 6);
			b.Entity<PagosPago>().Property(p => p.Monto).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionado>().Property(p => p.EquivalenciaDR).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionado>().Property(p => p.ImpSaldoAnt).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionado>().Property(p => p.ImpPagado).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionado>().Property(p => p.ImpSaldoInsoluto).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionadoImpuestosDRRetencionDR>().Property(p => p.BaseDR).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionadoImpuestosDRRetencionDR>().Property(p => p.TasaOCuotaDR).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionadoImpuestosDRRetencionDR>().Property(p => p.ImporteDR).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionadoImpuestosDRTrasladoDR>().Property(p => p.BaseDR).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionadoImpuestosDRTrasladoDR>().Property(p => p.TasaOCuotaDR).HasPrecision(18, 6);
			b.Entity<PagosPagoDoctoRelacionadoImpuestosDRTrasladoDR>().Property(p => p.ImporteDR).HasPrecision(18, 6);
			b.Entity<PagosPagoImpuestosPRetencionP>().Property(p => p.ImporteP).HasPrecision(18, 6);
			b.Entity<PagosPagoImpuestosPTrasladoP>().Property(p => p.BaseP).HasPrecision(18, 6);
			b.Entity<PagosPagoImpuestosPTrasladoP>().Property(p => p.TasaOCuotaP).HasPrecision(18, 6);
			b.Entity<PagosPagoImpuestosPTrasladoP>().Property(p => p.ImporteP).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalRetencionesIVA).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalRetencionesISR).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalRetencionesIEPS).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalTrasladosBaseIVA16).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalTrasladosImpuestoIVA16).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalTrasladosBaseIVA8).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalTrasladosImpuestoIVA8).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalTrasladosBaseIVA0).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalTrasladosImpuestoIVA0).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.TotalTrasladosBaseIVAExento).HasPrecision(18, 6);
			b.Entity<PagosTotales>().Property(p => p.MontoTotalPagos).HasPrecision(18, 6);

			b.Entity<TimbreFiscalDigital>().Property(n => n.Version).HasPrecision(18, 6);

		}

		private static void BuildAccesos(ModelBuilder b)
		{
			b.Entity<AppRole>().HasMany(r => r.Accesos).WithOne(am => am.Rol).OnDelete(DeleteBehavior.NoAction);
			b.Entity<Modulo>().HasMany(m => m.Accesos).WithOne(am => am.Modulo).OnDelete(DeleteBehavior.NoAction);

			b.Entity<Modulo>()
				.HasData(
					new Modulo() { Id = 1, Nombre = "Gestión de Talento", NombreNormalizado = "gestiondetalento", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 2, Nombre = "Usuarios", NombreNormalizado = "usuarios", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 3, Nombre = "Puestos", NombreNormalizado = "puestos", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 4, Nombre = "Áreas", NombreNormalizado = "areas", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 5, Nombre = "Subareas", NombreNormalizado = "subareas", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 6, Nombre = "Oficinas", NombreNormalizado = "oficinas", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 7, Nombre = "Empresas", NombreNormalizado = "empresas", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 8, Nombre = "Orígenes", NombreNormalizado = "origenes", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 9, Nombre = "Niveles", NombreNormalizado = "niveles", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 10, Nombre = "Perfiles", NombreNormalizado = "perfiles", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 11, Nombre = "Vacaciones", NombreNormalizado = "vacaciones", Deshabilitado = 0, Categoria = "erp" },
					new Modulo() { Id = 12, Nombre = "Incapacidades", NombreNormalizado = "incapacidades", Deshabilitado = 0, Categoria = "erp" },
					new Modulo() { Id = 13, Nombre = "Permisos", NombreNormalizado = "permisos", Deshabilitado = 0, Categoria = "erp" },
					new Modulo() { Id = 14, Nombre = "Prefacturas", NombreNormalizado = "prefacturas", Deshabilitado = 0, Categoria = "erp" },
					new Modulo() { Id = 15, Nombre = "Organigrama", NombreNormalizado = "organigrama", Deshabilitado = 0, Categoria = "reporte" },
					new Modulo() { Id = 16, Nombre = "Asistencia", NombreNormalizado = "asistencia", Deshabilitado = 0, Categoria = "reporte" },
					new Modulo() { Id = 17, Nombre = "Roles", NombreNormalizado = "roles", Deshabilitado = 0, Categoria = "catalogo" },
					new Modulo() { Id = 18, Nombre = "Activos Fijos", NombreNormalizado = "activosfijos", Deshabilitado = 0, Categoria = "erp" },
					new Modulo() { Id = 19, Nombre = "Conciliaciones", NombreNormalizado = "conciliaciones", Deshabilitado = 0, Categoria = "erp" },
					new Modulo() { Id = 20, Nombre = "Administrador de Comprobantes", NombreNormalizado = "administradordecomprobantes", Deshabilitado = 0, Categoria = "erp" },
                    new Modulo() { Id = 25, Nombre = "Banners", NombreNormalizado = "banners", Deshabilitado = 0, Categoria = "erp" },
                    new Modulo() { Id = 26, Nombre = "Mesa de Ayuda", NombreNormalizado = "mesadeayuda", Deshabilitado = 0, Categoria = "erp" }
                );
		}

		private static void BuildCuentasContables(ModelBuilder b)
		{
			b.Entity<CuentaContable>().HasOne(c => c.Empresa).WithMany(e => e.CuentasContables).OnDelete(DeleteBehavior.NoAction);
			b.Entity<CuentaContable>().HasOne(c => c.Tipo).WithMany(t => t.CuentasContables).OnDelete(DeleteBehavior.NoAction);
			b.Entity<CuentaContable>().HasOne(c => c.Subtipo).WithMany(t => t.CuentasContables).OnDelete(DeleteBehavior.NoAction);

			b.Entity<CuentaContableTipo>()
				.HasData(
					new CuentaContableTipo() { Id = 1, Clave = "E", Descripcion = "Egreso" },
					new CuentaContableTipo() { Id = 2, Clave = "I", Descripcion = "Ingreso" }
				);

			b.Entity<CuentaContableSubtipo>()
				.HasData(
					new CuentaContableSubtipo() { Id = 1, Clave = "CL", Descripcion = "Cliente" },
					new CuentaContableSubtipo() { Id = 2, Clave = "GA", Descripcion = "Gasto" },
					new CuentaContableSubtipo() { Id = 3, Clave = "VA", Descripcion = "Ventas al 16" },
					new CuentaContableSubtipo() { Id = 4, Clave = "PR", Descripcion = "Proveedor" },
					new CuentaContableSubtipo() { Id = 5, Clave = "VB", Descripcion = "Ventas al 0" },
					new CuentaContableSubtipo() { Id = 6, Clave = "VC", Descripcion = "Ventas Exentas" },
					new CuentaContableSubtipo() { Id = 7, Clave = "IN", Descripcion = "I.V.A. No Cobrado" },
					new CuentaContableSubtipo() { Id = 8, Clave = "IC", Descripcion = "I.V.A. Cobrado" }
				);
			b.Entity<CuentaContable>().HasMany(c => c.CuentasProductoServicio).WithOne(p => p.CuentaContable).OnDelete(DeleteBehavior.NoAction);

			b.Entity<ProductoServicio>().HasMany(p => p.ProductosServiciosCuenta).WithOne(c => c.ProductoServicio).OnDelete(DeleteBehavior.NoAction);
		}
	}
}