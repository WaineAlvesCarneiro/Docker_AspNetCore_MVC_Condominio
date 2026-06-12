using System.ComponentModel.DataAnnotations;

namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class Veiculo
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public int ImovelId { get; set; }
    public Imovel? Imovel { get; set; }

    public int? MoradorId { get; set; }
    public Morador? Morador { get; set; }

    [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
    public string? Marca { get; set; }

    [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
    public string? Modelo { get; set; }

    [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
    public string? Cor { get; set; }

    [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
    public string? Placa { get; set; }
}
