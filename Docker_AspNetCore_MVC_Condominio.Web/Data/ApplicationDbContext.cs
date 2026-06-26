using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Imovel> Imoveis => Set<Imovel>();
    public DbSet<Morador> Moradores => Set<Morador>();
    public DbSet<Prestador> Prestadores => Set<Prestador>();
    public DbSet<Visitante> Visitantes => Set<Visitante>();
    public DbSet<EncomendasRecebidas> EncomendasRecebidas => Set<EncomendasRecebidas>();
    public DbSet<EncomendasEntregues> EncomendasEntregues => Set<EncomendasEntregues>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}