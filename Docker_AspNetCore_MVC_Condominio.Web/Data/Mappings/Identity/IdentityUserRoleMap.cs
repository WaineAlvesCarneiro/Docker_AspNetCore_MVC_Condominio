using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class IdentityUserRoleMap : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.ToTable("AspNetUserRoles");

        builder.Property(x => x.UserId)
            .HasColumnType("varchar(450)");

        builder.Property(x => x.RoleId)
            .HasColumnType("varchar(450)");

        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.HasIndex(x => x.RoleId);

        builder.HasData(
            new IdentityUserRole<string>
            {
                UserId = "881a384d-b6f6-440b-b702-cf85ab1fcf25",
                RoleId = "931edab8-9e4c-4efe-b36a-809df5e6ef3f"
            },
            new IdentityUserRole<string>
            {
                UserId = "58869cd9-44f6-4ed9-bffa-3c292782b19c",
                RoleId = "6b48ee9b-c3e0-4769-9c95-b23d372ecccf"
            },
            new IdentityUserRole<string>
            {
                UserId = "153fdc05-56a3-4f92-a599-f57ce8e3d453",
                RoleId = "1b673850-ba2a-41b7-a0a6-bec0e57ace9f"
            }
        );
    }
}