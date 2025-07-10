using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.Clientes;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.Reportes;
using ERPSEI.Data.Entities.SAT;
using ERPSEI.Data.Entities.SAT.Catalogos;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ERPSEI.Data.Entities.SAT.Nomina12;
using ERPSEI.Data.Entities.SAT.TimbreFiscalDigital11;
using ERPSEI.Data.Entities.Cuentas;
using ERPSEI.Data.Entities.SAT.Pagos20;
using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Entities.ActivosFijos;
using Microsoft.Identity.Client;
using ERPSEI.Data.Entities.Vacaciones;
using System.Reflection.Emit;
using ERPSEI.Data.Entities.TipoContratos;

namespace ERPSEI.Data
{
	public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
	{
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
        public DbSet<EmpresaContrato> EmpresaContratos { get; set; }
        public DbSet<ClienteContrato> ClienteContratos { get; set; }

        //Vacaciones
        public DbSet<DiaFestivo> DiasFestivos { get; set; }
        public DbSet<HistorialVacaciones> HistorialesVacaciones { get; set; }
        public DbSet<PeriodoVacacional> PeriodosVacacionales { get; set; }
        public DbSet<SolicitudVacaciones> SolicitudesVacaciones { get; set; }

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

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

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

            //Vacaciones
            BuildTipoContratos(modelBuilder);
        }

		private static void BuildTipoContratos(ModelBuilder b)
		{
			b.Entity<TipoContrato>().HasData(
				new TipoContrato { Id = 1, Nombre = "Asimilados", Descripcion = "Contratos de tipo asimilados a salarios", Deshabilitado = true },
				new TipoContrato { Id = 2, Nombre = "Servicios", Descripcion = "Prestación de servicios profesionales o técnicos", Deshabilitado = true },
				new TipoContrato { Id = 3, Nombre = "Uso de Marca", Descripcion = "Contrato por uso de marca registrada", Deshabilitado = true },
				new TipoContrato { Id = 4, Nombre = "Arrendamiento Act.", Descripcion = "Arrendamiento de activos generales", Deshabilitado = true },
				new TipoContrato { Id = 5, Nombre = "Arrendamiento TI", Descripcion = "Arrendamiento de tecnología e infraestructura", Deshabilitado = true },
				new TipoContrato { Id = 6, Nombre = "Arrendamiento Ofi.", Descripcion = "Arrendamiento de oficinas físicas", Deshabilitado = true }
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
                new CategoriaActivoFijo { Id = 3, Descripcion = "Inmobiliario", Deshabilitado = false }
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
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.CSF, "CSF"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.INE, "INE"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.RFC, "RFC"),
                    new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.ComprobanteDomicilio, "ComprobanteDomicilio"),
                    new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.Otro, "Otro"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.CER, "CER"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.KEY, "KEY"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.Logo, "Logo"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.HojaMembretada, "HojaMembretada"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.INE2, "INE2"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.INE3, "INE3"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.ActaConstitutiva, "ActaConstitutiva"),
					new TipoArchivoEmpresa((int)Entities.Empresas.FileTypes.Organigrama, "Organigrama")
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
			

			b.Entity<TipoArchivo>()
				.HasData(
					new TipoArchivo((int)Entities.Empleados.FileTypes.ImagenPerfil, "Imagen de perfil"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.ActaNacimiento, "Acta de nacimiento"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.CURP, "CURP"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.CLABE, "CLABE"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.ComprobanteDomicilio, "Comprobante de domicilio"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.CSF, "CSF"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.INE, "INE"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.RFC, "RFC"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.ComprobanteEstudios, "Comprobante de estudios"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.NSS, "NSS"),
					new TipoArchivo((int)Entities.Empleados.FileTypes.Otro, "Otro")
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
					new Modulo() { Id = 20, Nombre = "Administrador de Comprobantes", NombreNormalizado = "administradordecomprobantes", Deshabilitado = 0, Categoria = "erp" }
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