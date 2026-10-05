using Docker_AspNetCore_MVC_Condominio.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace Docker_AspNetCore_MVC_Condominio.Web.Data;

public class ApplicationUser : IdentityUser
{
    public Empresa? Empresa { get; set; }
    public int EmpresaId { get; set; }
}