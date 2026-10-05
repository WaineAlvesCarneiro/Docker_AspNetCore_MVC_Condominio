namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class UsuarioViewModel
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public string EmpresaNome { get; set; } = string.Empty;
    public int? CodigoEmpresa { get; set; }
    public string? NovaSenha { get; set; }
}