
using ERPSEI.Data.Entities.TalentoHumano;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Configurations.TalentoHumano
{
    public static class TalentoHumanoModelConfiguration
    {
        public static void Configurar(ModelBuilder b)
        {
            ConfigurarSolicitudes(b);
            ConfigurarFolios(b);
            ConfigurarHistorial(b);
            ConfigurarRespuestas(b);
            ConfigurarDocumentos(b);
        }

        private static void ConfigurarSolicitudes(ModelBuilder b)
        {
            b.Entity<THSolicitud>(e =>
            {
                e.ToTable("TH_Solicitudes");
                e.HasKey(x => x.Id);

                e.Property(x => x.Folio)
                    .HasMaxLength(40)
                    .IsRequired();

                e.HasIndex(x => x.Folio)
                    .IsUnique();

                e.Property(x => x.Tipo)
                    .HasConversion<int>()
                    .IsRequired();

                e.Property(x => x.Estatus)
                    .HasConversion<int>()
                    .IsRequired();

                e.Property(x => x.SolicitanteUserId)
                    .HasMaxLength(450)
                    .IsRequired();

                e.Property(x => x.AtendidoPorUserId)
                    .HasMaxLength(450);

                e.Property(x => x.NumeroEmpleado)
                    .HasMaxLength(40)
                    .IsRequired();

                e.Property(x => x.NombreSolicitante)
                    .HasMaxLength(250)
                    .IsRequired();

                e.Property(x => x.AreaSolicitante)
                    .HasMaxLength(200);

                e.Property(x => x.OficinaSolicitante)
                    .HasMaxLength(200);

                e.Property(x => x.RowVersion)
                    .IsRowVersion();

                e.HasOne(x => x.Empleado)
                    .WithMany()
                    .HasForeignKey(x => x.EmpleadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Solicitante)
                    .WithMany()
                    .HasForeignKey(x => x.SolicitanteUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.AtendidoPor)
                    .WithMany()
                    .HasForeignKey(x => x.AtendidoPorUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new
                {
                    x.EmpleadoId,
                    x.FechaEnvioUtc
                });

                e.HasIndex(x => new
                {
                    x.Tipo,
                    x.Estatus,
                    x.FechaEnvioUtc
                });

                e.HasIndex(x => x.AtendidoPorUserId);
            });
        }

        private static void ConfigurarFolios(ModelBuilder b)
        {
            b.Entity<THFolioConsecutivo>(e =>
            {
                e.ToTable("TH_FolioConsecutivos", t =>
                {
                    t.HasCheckConstraint(
                        "CK_TH_Folios_Anio",
                        "[Anio] >= 2000");

                    t.HasCheckConstraint(
                        "CK_TH_Folios_Consecutivo",
                        "[UltimoConsecutivo] >= 0");
                });

                e.HasKey(x => x.Id);

                e.Property(x => x.Tipo)
                    .HasConversion<int>();

                e.HasIndex(x => new
                {
                    x.Tipo,
                    x.Anio
                }).IsUnique();

                e.Property(x => x.RowVersion)
                    .IsRowVersion();
            });
        }

        private static void ConfigurarHistorial(ModelBuilder b)
        {
            b.Entity<THHistorialSolicitud>(e =>
            {
                e.ToTable("TH_HistorialSolicitudes");
                e.HasKey(x => x.Id);

                e.Property(x => x.UsuarioId)
                    .HasMaxLength(450);

                e.Property(x => x.RolEjecutor)
                    .HasMaxLength(100)
                    .IsRequired();

                e.Property(x => x.Accion)
                    .HasMaxLength(100)
                    .IsRequired();

                e.Property(x => x.Comentario)
                    .HasMaxLength(2000);

                e.Property(x => x.IpOrigen)
                    .HasMaxLength(64);

                e.Property(x => x.EstatusAnterior)
                    .HasConversion<int?>();

                e.Property(x => x.EstatusNuevo)
                    .HasConversion<int?>();

                e.HasOne(x => x.Solicitud)
                    .WithMany(x => x.Historial)
                    .HasForeignKey(x => x.SolicitudId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Usuario)
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new
                {
                    x.SolicitudId,
                    x.FechaHoraUtc
                });
            });
        }

        private static void ConfigurarRespuestas(ModelBuilder b)
        {
            b.Entity<THRespuesta>(e =>
            {
                e.ToTable("TH_Respuestas");
                e.HasKey(x => x.Id);

                e.Property(x => x.TipoRespuesta)
                    .HasMaxLength(40)
                    .IsRequired();

                e.Property(x => x.Contenido)
                    .HasMaxLength(4000);

                e.Property(x => x.UsuarioRespuestaId)
                    .HasMaxLength(450);

                e.HasOne(x => x.Solicitud)
                    .WithMany(x => x.Respuestas)
                    .HasForeignKey(x => x.SolicitudId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.UsuarioRespuesta)
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioRespuestaId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new
                {
                    x.SolicitudId,
                    x.FechaRespuestaUtc
                });
            });
        }

        private static void ConfigurarDocumentos(ModelBuilder b)
        {
            b.Entity<THDocumento>(e =>
            {
                e.ToTable("TH_Documentos");
                e.HasKey(x => x.Id);

                e.Property(x => x.NombreOriginal)
                    .HasMaxLength(260)
                    .IsRequired();

                e.Property(x => x.NombreAlmacenado)
                    .HasMaxLength(100);

                e.Property(x => x.TipoContenido)
                    .HasMaxLength(150);

                e.Property(x => x.Origen)
                    .HasMaxLength(30)
                    .IsRequired();

                e.Property(x => x.DriveId)
                    .HasMaxLength(300);

                e.Property(x => x.DriveItemId)
                    .HasMaxLength(500);

                e.Property(x => x.RutaAlmacenamiento)
                    .HasMaxLength(1000);

                e.Property(x => x.HashSha256)
                    .HasMaxLength(64);

                e.Property(x => x.Periodo)
                    .HasMaxLength(50);

                e.Property(x => x.Formato)
                    .HasMaxLength(15);

                e.Property(x => x.UsuarioCargaId)
                    .HasMaxLength(450);

                e.HasOne(x => x.Solicitud)
                    .WithMany(x => x.Documentos)
                    .HasForeignKey(x => x.SolicitudId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Respuesta)
                    .WithMany()
                    .HasForeignKey(x => x.RespuestaId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.UsuarioCarga)
                    .WithMany()
                    .HasForeignKey(x => x.UsuarioCargaId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new
                {
                    x.SolicitudId,
                    x.FechaRegistroUtc
                });

                e.HasIndex(x => new
                {
                    x.DriveId,
                    x.DriveItemId
                });
            });
        }
    }
}
