using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data.Mappings;

public class ApplicationUserMap : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AspNetUsers");

        builder.Property(x => x.Id)
            .HasColumnType("varchar(450)");

        builder.Property(x => x.AccessFailedCount)
            .HasColumnType("int");

        builder.Property(x => x.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasColumnType("varchar(max)");

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(x => x.EmailConfirmed)
            .HasColumnType("bit");

        builder.Property(x => x.EmpresaId)
            .HasColumnType("int");

        builder.Property(x => x.LockoutEnabled)
            .HasColumnType("bit");

        builder.Property(x => x.LockoutEnd)
            .HasColumnType("datetimeoffset");

        builder.Property(x => x.NormalizedEmail)
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(x => x.NormalizedUserName)
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.Property(x => x.PasswordHash)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.PhoneNumber)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.PhoneNumberConfirmed)
            .HasColumnType("bit");

        builder.Property(x => x.SecurityStamp)
            .HasColumnType("varchar(max)");

        builder.Property(x => x.TwoFactorEnabled)
            .HasColumnType("bit");

        builder.Property(x => x.UserName)
            .HasMaxLength(256)
            .HasColumnType("varchar(256)");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.EmpresaId);

        builder.HasIndex(x => x.NormalizedEmail)
            .HasDatabaseName("EmailIndex");

        builder.HasIndex(x => x.NormalizedUserName)
            .IsUnique()
            .HasDatabaseName("UserNameIndex")
            .HasFilter("[NormalizedUserName] IS NOT NULL");

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new ApplicationUser
            {
                Id = "881a384d-b6f6-440b-b702-cf85ab1fcf25",
                EmpresaId = 1,
                UserName = "Suporte",
                NormalizedUserName = "SUPORTE",
                Email = "suporte@gmail.com",
                NormalizedEmail = "SUPORTE@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEH4HinoUk5qZetdcVXYuqthLpbW0Umn3NuRSuIRZm+1KO1vu3nDW1dqtOVyw4TRnYw==",
                SecurityStamp = "OZEMNFX3PHDSKQGLTD3Z74X2XVC2TFL6",
                ConcurrencyStamp = "a11262f7-3190-4b40-82f6-9d7b89a551ae",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            },
            new ApplicationUser
            {
                Id = "58869cd9-44f6-4ed9-bffa-3c292782b19c",
                EmpresaId = 1,
                UserName = "Porteiro",
                NormalizedUserName = "PORTEIRO",
                Email = "porteiro@gmail.com",
                NormalizedEmail = "PORTEIRO@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEH4HinoUk5qZetdcVXYuqthLpbW0Umn3NuRSuIRZm+1KO1vu3nDW1dqtOVyw4TRnYw==",
                SecurityStamp = "OZEMNFX3PHDSKQGLTD3Z74X2XVC2TFL6",
                ConcurrencyStamp = "a11262f7-3190-4b40-82f6-9d7b89a551ae",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            },
            new ApplicationUser
            {
                Id = "153fdc05-56a3-4f92-a599-f57ce8e3d453",
                EmpresaId = 1,
                UserName = "Sindico",
                NormalizedUserName = "SINDICO",
                Email = "sindico@gmail.com",
                NormalizedEmail = "SINDICO@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEH4HinoUk5qZetdcVXYuqthLpbW0Umn3NuRSuIRZm+1KO1vu3nDW1dqtOVyw4TRnYw==",
                SecurityStamp = "OZEMNFX3PHDSKQGLTD3Z74X2XVC2TFL6",
                ConcurrencyStamp = "a11262f7-3190-4b40-82f6-9d7b89a551ae",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            }
        );
    }
}