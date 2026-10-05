using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class MoradorMap : IEntityTypeConfiguration<Morador>
{
    public void Configure(EntityTypeBuilder<Morador> builder)
    {
        builder.ToTable("Morador");

        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Empresa)
            .WithMany(x => x.Moradores)
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Imovel)
            .WithMany(x => x.Moradores)
            .HasForeignKey(x => x.ImovelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("varchar(150)");

        builder.Property(x => x.Celular)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnType("varchar(16)");

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("varchar(150)");

        builder.Property(x => x.Proprietario)
            .IsRequired();

        builder.Property(x => x.DataEntrada)
            .IsRequired();

        builder.Property(x => x.DataSaida)
            .IsRequired(false);
    }
}