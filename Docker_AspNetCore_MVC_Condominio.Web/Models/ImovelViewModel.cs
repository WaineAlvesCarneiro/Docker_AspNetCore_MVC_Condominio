namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class ImovelViewModel
{
    public Imovel? Imovel { get; set; }
    public Morador? Morador { get; set; }
    public ICollection<Morador> Moradores { get; set; } = new List<Morador>();
    public Prestador? Prestador { get; set; }
    public Visitante? Visitante { get; set; }
    public EncomendasRecebidas EncomendasRecebidas { get; set; } = new EncomendasRecebidas
    {
        CodigoRastreio = string.Empty,
        DataRecebimento = DateTime.Now
    };
    public EncomendasEntregues EncomendasEntregues { get; set; } = new EncomendasEntregues
    {
        DataEntrega = DateTime.Now
    };
    public Morador? MoradorRecebeuEntrega { get; set; }
}