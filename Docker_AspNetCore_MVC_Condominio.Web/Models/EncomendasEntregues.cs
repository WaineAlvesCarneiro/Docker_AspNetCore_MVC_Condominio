using System.ComponentModel.DataAnnotations;

namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class EncomendasEntregues
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public int ImovelId { get; set; }
    public Imovel? Imovel { get; set; }
    public int EncomendasRecebidasId { get; set; }
    public EncomendasRecebidas? EncomendasRecebidas { get; set; }

    [Required(ErrorMessage = "Morador que recebeu é obrigatório")]
    public int MoradorId { get; set; }
    public Morador? Morador { get; set; }

    [Display(Name = "Data da Entrega")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Data de Entrega é obrigatória")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime DataEntrega { get; set; }
}
