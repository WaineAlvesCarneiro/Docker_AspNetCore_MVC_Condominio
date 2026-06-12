using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class IdentityUserLoginMap : IEntityTypeConfiguration<IdentityUserLogin<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
    {
        builder.ToTable("AspNetUserLogins");

        builder.HasKey(x => new { x.LoginProvider, x.ProviderKey });

        builder.Property(x => x.LoginProvider)
            .HasColumnType("varchar(450)");

        builder.Property(x => x.ProviderKey)
            .HasColumnType("varchar(450)");

        builder.Property(x => x.ProviderDisplayName)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnType("varchar(450)");

        builder.HasIndex(x => x.UserId);
    }
}