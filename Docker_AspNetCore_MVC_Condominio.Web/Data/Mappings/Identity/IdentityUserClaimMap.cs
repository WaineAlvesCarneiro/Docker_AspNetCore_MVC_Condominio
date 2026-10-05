using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class IdentityUserClaimMap : IEntityTypeConfiguration<IdentityUserClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        builder.ToTable("AspNetUserClaims");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClaimType)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.ClaimValue)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnType("varchar(450)");

        builder.HasIndex(x => x.UserId);
    }
}