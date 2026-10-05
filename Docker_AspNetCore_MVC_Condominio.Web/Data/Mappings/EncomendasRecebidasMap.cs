using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class EncomendasRecebidasMap : IEntityTypeConfiguration<EncomendasRecebidas>
{
    public void Configure(EntityTypeBuilder<EncomendasRecebidas> builder)
    {
        builder.ToTable("EncomendasRecebidas");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Imovel)
            .WithMany()
            .HasForeignKey(x => x.ImovelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Morador)
            .WithMany()
            .HasForeignKey(x => x.MoradorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CodigoRastreio)
            .IsRequired()
            .HasMaxLength(30)
            .HasColumnType("varchar(30)");

        builder.Property(x => x.DataRecebimento)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(x => x.Entregue)
            .HasColumnType("bit")
            .IsRequired();
    }
}