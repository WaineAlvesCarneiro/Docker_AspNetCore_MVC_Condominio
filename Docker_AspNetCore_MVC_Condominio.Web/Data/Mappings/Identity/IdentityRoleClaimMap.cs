using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class IdentityRoleClaimMap : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        builder.ToTable("AspNetRoleClaims");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClaimType)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.ClaimValue)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.RoleId)
            .IsRequired()
            .HasColumnType("varchar(450)");

        builder.HasIndex(x => x.RoleId);
    }
}