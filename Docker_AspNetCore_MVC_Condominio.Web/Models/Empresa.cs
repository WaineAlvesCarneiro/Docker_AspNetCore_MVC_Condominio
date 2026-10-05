using Docker_AspNetCore_MVC_Condominio.Web.Helper;
using System.ComponentModel.DataAnnotations;

namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class Empresa
{
    public int Id { get; set; }
    public ICollection<Imovel> Imoveis { get; set; } = [];
    public ICollection<Morador> Moradores { get; set; } = [];
    public ICollection<Visitante> Visitantes { get; set; } = [];
    public ICollection<Prestador> Prestadores { get; set; } = [];

    [Required(ErrorMessage = "{0} é obrigatorio")]
    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string RazaoSocial { get; set; }

    [Cnpj]
    [Required(ErrorMessage = "{0} é obrigatorio")]
    public required string Cnpj { get; set; }

    public int TipoDeCondominio { get; set; }

    [Required(ErrorMessage = "{0} é obrigatorio")]
    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string NomeResponsavel { get; set; }

    [Required(ErrorMessage = "{0} é obrigatorio")]
    public required string Celular { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "{0} é obrigatorio")]
    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "{0} é obrigatorio")]
    public required string Cep { get; set; }

    public string? Uf { get; set; }

    public string? Cidade { get; set; }

    public string? Endereco { get; set; }

    public string? Bairro { get; set; }

    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? Complemento { get; set; }
}
