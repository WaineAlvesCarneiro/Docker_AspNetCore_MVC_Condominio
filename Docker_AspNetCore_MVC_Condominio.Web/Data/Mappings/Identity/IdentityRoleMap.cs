using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class IdentityRoleMap : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.ToTable("AspNetRoles");

        builder.Property(x => x.Id)
            .HasColumnType("varchar(450)");

        builder.Property(x => x.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasColumnType("varchar(max)");

        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(x => x.NormalizedName)
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.NormalizedName)
            .IsUnique()
            .HasDatabaseName("RoleNameIndex")
            .HasFilter("[NormalizedName] IS NOT NULL");

        builder.HasData(
            new IdentityRole
            {
                Id = "931edab8-9e4c-4efe-b36a-809df5e6ef3f",
                Name = "Suporte",
                NormalizedName = "SUPORTE",
                ConcurrencyStamp = "1dd91ff7-b584-405c-844f-29d31484baed"
            },
            new IdentityRole
            {
                Id = "6b48ee9b-c3e0-4769-9c95-b23d372ecccf",
                Name = "Porteiro",
                NormalizedName = "Porteiro",
                ConcurrencyStamp = "c8eb834a-7701-41b2-b9a9-b7f4a2d47030"
            },
            new IdentityRole
            {
                Id = "1b673850-ba2a-41b7-a0a6-bec0e57ace9f",
                Name = "Sindico",
                NormalizedName = "Sindico",
                ConcurrencyStamp = "87c797bd-36e7-4d01-953f-9f6170237874"
            }
        );
    }
}