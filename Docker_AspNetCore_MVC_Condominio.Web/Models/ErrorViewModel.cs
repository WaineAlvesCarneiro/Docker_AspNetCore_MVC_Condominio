namespace Docker_AspNetCore_MVC_Condominio.Web.Models;

public class ErrorViewModel
{
    public required string RequestId { get; set; }
    public string? Message { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
