using System.ComponentModel.DataAnnotations;

namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class Morador
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public int ImovelId { get; set; }
    public Imovel? Imovel { get; set; }

    [Required(ErrorMessage = "{0} é obrigatório")]
    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string Nome { get; set; }

    [Required(ErrorMessage = "{0} é obrigatório")]
    [MaxLength(16, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string Celular { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "{0} é obrigatório")]
    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string Email { get; set; }

    public int Proprietario { get; set; }

    [Display(Name = "Data de Entrada")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Data de Entrada é obrigatória")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime DataEntrada { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? DataSaida { get; set; }
}
