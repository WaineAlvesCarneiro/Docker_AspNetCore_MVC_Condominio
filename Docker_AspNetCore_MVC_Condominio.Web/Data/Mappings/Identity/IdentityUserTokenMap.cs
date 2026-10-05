using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class IdentityUserTokenMap : IEntityTypeConfiguration<IdentityUserToken<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
    {
        builder.ToTable("AspNetUserTokens");

        builder.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });

        builder.Property(x => x.UserId)
            .HasColumnType("varchar(450)");

        builder.Property(x => x.LoginProvider)
            .HasColumnType("varchar(450)");

        builder.Property(x => x.Name)
            .HasColumnType("varchar(450)");

        builder.Property(x => x.Value)
            .HasColumnType("varchar(max)");
    }
}