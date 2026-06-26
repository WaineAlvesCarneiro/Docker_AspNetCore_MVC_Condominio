using System.ComponentModel.DataAnnotations;

namespace Docker_AspNetCore_MVC_Condominio.Web.Helper;

public class CnpjAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        var cnpj = value as string;
        if (string.IsNullOrEmpty(cnpj) || !ValidarCnpj(cnpj))
        {
            return new ValidationResult("CNPJ inválido.");
        }
        return ValidationResult.Success;
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj)) return false;
        cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        if (cnpj.Length != 14) return false;

        if (new string(cnpj[0], 14) == cnpj) return false;

        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = cnpj.Substring(0, 12);
        int soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        if (resto < 2) resto = 0;
        else resto = 11 - resto;

        string digito = resto.ToString();
        tempCnpj += digito;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        if (resto < 2) resto = 0;
        else resto = 11 - resto;

        digito += resto.ToString();

        return cnpj.EndsWith(digito);
    }
}
