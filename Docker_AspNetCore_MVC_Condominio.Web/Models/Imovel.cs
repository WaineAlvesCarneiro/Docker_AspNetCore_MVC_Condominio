using System.ComponentModel.DataAnnotations;

namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class Imovel
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public ICollection<Morador>? Moradores { get; set; }

    [Required(ErrorMessage = "{0} é obrigatorio")]
    [MaxLength(10, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string Bloco { get; set; }

    [Required(ErrorMessage = "{0} é obrigatorio")]
    [MaxLength(10, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string Apartamento { get; set; }

    [Required(ErrorMessage = "{0} é obrigatorio")]
    [MaxLength(10, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string BoxGaragem { get; set; }
}
