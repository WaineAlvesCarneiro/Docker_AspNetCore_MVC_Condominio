using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class VisitanteMap : IEntityTypeConfiguration<Visitante>
{
    public void Configure(EntityTypeBuilder<Visitante> builder)
    {
        builder.ToTable("Visitante");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Empresa)
            .WithMany(x => x.Visitantes)
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Imovel)
            .WithMany()
            .HasForeignKey(x => x.ImovelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("varchar(150)");

        builder.Property(x => x.DataEntrada)
            .IsRequired();

        builder.Property(x => x.DataSaida)
            .IsRequired(false);
    }
}