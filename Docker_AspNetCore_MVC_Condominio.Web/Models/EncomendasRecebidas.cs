using System.ComponentModel.DataAnnotations;

namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class EncomendasRecebidas
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public int ImovelId { get; set; }
    public Imovel? Imovel { get; set; }

    [Required(ErrorMessage = "Morador na encomenda é obrigatório")]
    public int MoradorId { get; set; }
    public Morador? Morador { get; set; }

    [Required(ErrorMessage = "{0} é obrigatório")]
    [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public required string CodigoRastreio { get; set; }

    [Display(Name = "Data Recebida")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Data de Recebida é obrigatória")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public required DateTime DataRecebimento { get; set; }

    public bool Entregue { get; set; }
}
