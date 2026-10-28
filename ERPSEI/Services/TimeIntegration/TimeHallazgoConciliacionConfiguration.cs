
using ERPSEI.Data.Entities.TimeIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPSEI.Data.Configurations.TimeIntegration
{
    public sealed class TimeHallazgoConciliacionConfiguration
        : IEntityTypeConfiguration<TimeHallazgoConciliacion>
    {
        public void Configure(
            EntityTypeBuilder<TimeHallazgoConciliacion> b)
        {
            b.ToTable(
                "HallazgosConciliacion",
                "TimeIntegracion");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            b.Property(x => x.TablaOrigen)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.TipoHallazgo)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.Estado)
                .HasMaxLength(30)
                .IsRequired();

            b.Property(x => x.Observaciones)
                .HasMaxLength(1000);

            b.HasIndex(x => new
            {
                x.TablaOrigen,
                x.RegistroOrigenId,
                x.TipoHallazgo
            }).IsUnique();

            b.HasIndex(x => new
            {
                x.Estado,
                x.UltimaRevision
            });
        }
    }
}
