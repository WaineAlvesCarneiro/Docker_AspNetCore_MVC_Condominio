using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class EmpresaMap : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("Empresa");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RazaoSocial)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("varchar(150)");

        builder.Property(x => x.Cnpj)
            .IsRequired()
            .HasMaxLength(14)
            .HasColumnType("varchar(14)");

        builder.Property(p => p.TipoDeCondominio)
            .HasColumnType("int");

        builder.Property(p => p.NomeResponsavel)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("varchar(150)");

        builder.Property(p => p.Celular)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnType("varchar(16)");

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("varchar(150)");

        builder.Property(p => p.Cep)
            .HasMaxLength(12)
            .HasColumnType("varchar(12)");

        builder.Property(p => p.Uf)
            .HasMaxLength(30)
            .HasColumnType("varchar(30)");

        builder.Property(p => p.Cidade)
            .HasMaxLength(150)
            .HasColumnType("varchar(150)");

        builder.Property(p => p.Endereco)
            .HasMaxLength(150)
            .HasColumnType("varchar(150)");

        builder.Property(p => p.Complemento)
            .HasMaxLength(150)
            .HasColumnType("varchar(150)");

        builder.HasData(
            new Empresa
            {
                Id = 1,
                RazaoSocial = "Engenharia de Software S/A",
                Cnpj = "01101201000110",
                TipoDeCondominio = 1,
                NomeResponsavel = "Engenheiro",
                Celular = "62999999999",
                Email = "engenharia@gmail.com",
                Cep = "74000000",
                Uf = "GO",
                Cidade = "Cidade do Engenheiro",
                Endereco = "Rua Engenheiro",
                Bairro = "Setor Engenheiro",
                Complemento = "QD 130 N° 1300 Andar 8 Sala 101"
            }
        );
    }
}
