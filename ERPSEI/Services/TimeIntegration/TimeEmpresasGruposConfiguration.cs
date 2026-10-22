
using ERPSEI.Data.Entities.TimeIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPSEI.Data.Configurations.TimeIntegration
{
    public sealed class TimeEmpresaConfiguration
        : IEntityTypeConfiguration<TimeEmpresa>
    {
        public void Configure(EntityTypeBuilder<TimeEmpresa> b)
        {
            b.ToTable("Empresas", "TimeIntegracion");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedNever();

            b.Property(x => x.Nombre)
                .HasMaxLength(150);

            b.Property(x => x.Clave)
                .HasMaxLength(7);

            b.HasIndex(x => x.GrupoId);

            b.HasIndex(x => x.Nombre);
        }
    }

    public sealed class TimeGrupoConfiguration
        : IEntityTypeConfiguration<TimeGrupo>
    {
        public void Configure(EntityTypeBuilder<TimeGrupo> b)
        {
            b.ToTable("Grupos", "TimeIntegracion");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedNever();

            b.Property(x => x.Nombre)
                .HasMaxLength(50);

            b.HasIndex(x => x.Nombre);
        }
    }
}
