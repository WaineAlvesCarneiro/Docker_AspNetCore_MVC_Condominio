using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class ImovelMap : IEntityTypeConfiguration<Imovel>
{
    public void Configure(EntityTypeBuilder<Imovel> builder)
    {
        builder.ToTable("Imovel");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Empresa)
            .WithMany(x => x.Imoveis)
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Bloco)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("varchar(10)");

        builder.Property(x => x.Apartamento)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("varchar(10)");

        builder.Property(x => x.BoxGaragem)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("varchar(10)");
    }
}