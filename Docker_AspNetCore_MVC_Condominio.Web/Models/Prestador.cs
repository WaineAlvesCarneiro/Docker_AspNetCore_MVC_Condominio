using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class Prestador
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
    [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string Documento { get; set; }

    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string? EmpresaPrestadora { get; set; }

    [Display(Name = "Data de Entrada")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Data de Entrada é obrigatória")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime DataEntrada { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? DataSaida { get; set; }
}
