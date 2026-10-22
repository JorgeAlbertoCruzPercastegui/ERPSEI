
using ERPSEI.Data.Entities.TimeIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPSEI.Data.Configurations.TimeIntegration
{
    public class TimeUsuarioConfiguration
        : IEntityTypeConfiguration<TimeUsuario>
    {
        public void Configure(EntityTypeBuilder<TimeUsuario> b)
        {
            b.ToTable("Usuarios", "TimeIntegracion");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Nombre).HasMaxLength(191);
            b.Property(x => x.Email).HasMaxLength(191);
            b.HasIndex(x => x.DepartamentoId);
        }
    }

    public class TimeDepartamentoConfiguration
        : IEntityTypeConfiguration<TimeDepartamento>
    {
        public void Configure(EntityTypeBuilder<TimeDepartamento> b)
        {
            b.ToTable("Departamentos", "TimeIntegracion");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Nombre).HasMaxLength(100);
            b.Property(x => x.Clave).HasMaxLength(5);
        }
    }

    public class TimeClaveConfiguration
        : IEntityTypeConfiguration<TimeClave>
    {
        public void Configure(EntityTypeBuilder<TimeClave> b)
        {
            b.ToTable("Claves", "TimeIntegracion");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Proyecto).HasMaxLength(150);
            b.Property(x => x.Clave).HasMaxLength(15);
            b.HasIndex(x => x.DepartamentoId);
        }
    }

    public class TimeActividadConfiguration
        : IEntityTypeConfiguration<TimeActividad>
    {
        public void Configure(EntityTypeBuilder<TimeActividad> b)
        {
            b.ToTable("Actividades", "TimeIntegracion");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Actividad).HasMaxLength(100);
            b.HasIndex(x => new { x.UsuarioId, x.ClaveId });
        }
    }

    public class TimeHoraConfiguration
        : IEntityTypeConfiguration<TimeHora>
    {
        public void Configure(EntityTypeBuilder<TimeHora> b)
        {
            b.ToTable("Horas", "TimeIntegracion");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Fecha).HasColumnType("date");
            b.HasIndex(x => x.Fecha);
            b.HasIndex(x => x.ActividadId);
        }
    }

    public class TimeEstadoSincronizacionConfiguration
        : IEntityTypeConfiguration<TimeEstadoSincronizacion>
    {
        public void Configure(
            EntityTypeBuilder<TimeEstadoSincronizacion> b)
        {
            b.ToTable("EstadoSincronizacion", "TimeIntegracion");
            b.HasKey(x => x.Tabla);
            b.Property(x => x.Tabla).HasMaxLength(100);
            b.Property(x => x.Estado).HasMaxLength(30);
            b.Property(x => x.Error).HasMaxLength(2000);
            b.Property(x => x.UltimoId).HasDefaultValue(0L);
            b.Property(x => x.RegistrosProcesados)
                .HasDefaultValue(0L);
        }
    }
}
