
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.RelojChecador;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;

namespace ERPSEI.Data.Configurations.RelojChecador;

public static class RelojChecadorModelConfiguration
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RC_OficinaGeocerca>(e =>
        {
            e.ToTable("RC_OficinasGeocercas");

            e.HasKey(x => x.Id);

            e.Property(x => x.Latitud).HasPrecision(9, 6);
            e.Property(x => x.Longitud).HasPrecision(9, 6);

            e.HasIndex(x => x.OficinaId).IsUnique();

            e.HasOne<Oficina>()
                .WithMany()
                .HasForeignKey(x => x.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasCheckConstraint(
                "CK_RC_Geocerca_Latitud",
                "[Latitud] BETWEEN -90 AND 90");

            e.HasCheckConstraint(
                "CK_RC_Geocerca_Longitud",
                "[Longitud] BETWEEN -180 AND 180");

            e.HasCheckConstraint(
                "CK_RC_Geocerca_Radio",
                "[RadioMetros] BETWEEN 10 AND 1000");

            e.HasCheckConstraint(
                "CK_RC_Geocerca_Precision",
                "[PrecisionMaximaMetros] BETWEEN 5 AND 500");
        });

        modelBuilder.Entity<RC_AutorizacionModalidad>(e =>
        {
            e.ToTable("RC_AutorizacionesModalidades");

            e.HasKey(x => x.Id);

            e.Property(x => x.UserId)
                .HasMaxLength(450).IsRequired();

            e.Property(x => x.Modalidad)
                .HasMaxLength(30).IsRequired();

            e.Property(x => x.Estado)
                .HasMaxLength(30).IsRequired();

            e.Property(x => x.Motivo)
                .HasMaxLength(500);

            e.Property(x => x.AutorizadoPorUserId)
                .HasMaxLength(450);

            e.HasIndex(x => new
            {
                x.UserId,
                x.FechaInicio,
                x.FechaFin
            });

            e.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne<Oficina>()
                .WithMany()
                .HasForeignKey(x => x.OficinaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasCheckConstraint(
                "CK_RC_Autorizacion_Fechas",
                "[FechaFin] >= [FechaInicio]");

            e.HasCheckConstraint(
                "CK_RC_Autorizacion_Modalidad",
                "[Modalidad] IN ('Presencial','Remoto','Hibrido','Comision')");

            e.HasCheckConstraint(
                "CK_RC_Autorizacion_Estado",
                "[Estado] IN ('Pendiente','Aprobada','Rechazada','Revocada')");

            // Relación única entre Ausencias y autorizaciones HO.
            // Permite NULL para las autorizaciones manuales existentes.
            e.HasIndex(x => x.AusenciaId)
                .IsUnique()
                .HasFilter("[AusenciaId] IS NOT NULL")
                .HasDatabaseName("UX_RC_AutorizacionesModalidades_AusenciaId");
        });

        modelBuilder.Entity<RC_Jornada>(e =>
        {
            e.ToTable("RC_Jornadas");

            e.HasKey(x => x.Id);

            e.Property(x => x.UserId)
                .HasMaxLength(450).IsRequired();

            e.Property(x => x.Estado)
                .HasMaxLength(30).IsRequired();

            e.Property(x => x.Modalidad)
                .HasMaxLength(30).IsRequired();

            e.Property(x => x.MotivoRevision)
                .HasMaxLength(500);

            // Un usuario no puede tener dos jornadas abiertas.
            e.HasIndex(x => x.UserId)
                .IsUnique()
                .HasFilter("[Estado] = 'Abierta'");

            e.HasIndex(x => new
            {
                x.EmpleadoId,
                x.InicioUtc
            });

            e.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne<Empleado>()
                .WithMany()
                .HasForeignKey(x => x.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasCheckConstraint(
                "CK_RC_Jornada_Estado",
                "[Estado] IN ('Abierta','Cerrada','EnRevision','Anulada')");

            e.HasCheckConstraint(
                "CK_RC_Jornada_Objetivo",
                "[MinutosObjetivo] BETWEEN 1 AND 1440");

            e.HasCheckConstraint(
                "CK_RC_Jornada_Fechas",
                "[FinUtc] IS NULL OR [FinUtc] >= [InicioUtc]");
        });

        modelBuilder.Entity<RC_Checada>(e =>
        {
            e.ToTable("RC_Checadas");

            e.HasKey(x => x.Id);

            e.Property(x => x.UserId)
                .HasMaxLength(450).IsRequired();

            e.Property(x => x.TipoMovimiento)
                .HasMaxLength(30).IsRequired();

            e.Property(x => x.Origen)
                .HasMaxLength(30).IsRequired();

            e.Property(x => x.Latitud).HasPrecision(9, 6);
            e.Property(x => x.Longitud).HasPrecision(9, 6);
            e.Property(x => x.PrecisionMetros).HasPrecision(9, 2);

            e.Property(x => x.DistanciaOficinaMetros)
                .HasPrecision(12, 2);

            e.Property(x => x.ResultadoUbicacion)
                .HasMaxLength(30);

            e.Property(x => x.EstadoValidacion)
                .HasMaxLength(30);

            e.Property(x => x.ObservacionCodigo)
                .HasMaxLength(100);

            e.HasIndex(x => x.IdempotencyKey)
                .IsUnique();

            e.HasIndex(x => new
            {
                x.UserId,
                x.FechaHoraUtc
            });

            e.HasIndex(x => new
            {
                x.JornadaId,
                x.FechaHoraUtc
            });

            e.HasOne(x => x.Jornada)
                .WithMany()
                .HasForeignKey(x => x.JornadaId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasCheckConstraint(
                "CK_RC_Checada_Tipo",
                "[TipoMovimiento] IN ('Entrada','SalidaComer','RegresoComer','Salida')");

            e.HasCheckConstraint(
                "CK_RC_Checada_Origen",
                "[Origen] IN ('INTRANET_WEB','INTRANET_PWA','AJUSTE_TH')");
        });

        modelBuilder.Entity<RC_Evidencia>(e =>
        {
            e.ToTable("RC_Evidencias");

            e.HasKey(x => x.Id);

            e.Property(x => x.RutaPrivada)
                .HasMaxLength(500).IsRequired();

            e.Property(x => x.HashSha256)
                .HasMaxLength(64).IsRequired();

            e.Property(x => x.TipoContenido)
                .HasMaxLength(100);

            e.HasIndex(x => x.ChecadaId).IsUnique();

            e.HasOne(x => x.Checada)
                .WithOne()
                .HasForeignKey<RC_Evidencia>(x => x.ChecadaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RC_Auditoria>(e =>
        {
            e.ToTable("RC_Auditoria");

            e.HasKey(x => x.Id);

            e.Property(x => x.ActorUserId)
                .HasMaxLength(450);

            e.Property(x => x.AfectadoUserId)
                .HasMaxLength(450);

            e.Property(x => x.Accion)
                .HasMaxLength(100).IsRequired();

            e.Property(x => x.Resultado)
                .HasMaxLength(50).IsRequired();

            e.Property(x => x.CodigoDetalle)
                .HasMaxLength(200);

            e.HasIndex(x => x.FechaUtc);

            e.HasIndex(x => new
            {
                x.AfectadoUserId,
                x.FechaUtc
            });
        });
    }
}
