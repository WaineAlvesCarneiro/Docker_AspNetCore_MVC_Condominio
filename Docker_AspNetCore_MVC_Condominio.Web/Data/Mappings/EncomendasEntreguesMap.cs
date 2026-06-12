using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class EncomendasEntreguesMap : IEntityTypeConfiguration<EncomendasEntregues>
{
    public void Configure(EntityTypeBuilder<EncomendasEntregues> builder)
    {
        builder.ToTable("EncomendasEntregues");

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

        builder.HasOne(x => x.EncomendasRecebidas)
            .WithMany()
            .HasForeignKey(x => x.EncomendasRecebidasId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.DataEntrega)
            .IsRequired()
            .HasColumnType("datetime");
    }
}