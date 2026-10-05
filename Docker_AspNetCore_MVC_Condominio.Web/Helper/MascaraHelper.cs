namespace Docker_AspNetCore_MVC_Condominio.Web.Helper;

public static class MascaraHelper
{
    public static string FormatarCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return string.Empty;

        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cnpj.Length != 14)
            return cnpj;

        return Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
    }

    public static string FormatarCelular(string celular)
    {
        if (string.IsNullOrWhiteSpace(celular))
            return string.Empty;

        celular = new string(celular.Where(char.IsDigit).ToArray());

        if (celular.Length == 11)
            return Convert.ToUInt64(celular).ToString(@"\(00\)\ 00000\-0000");

        if (celular.Length == 10)
            return Convert.ToUInt64(celular).ToString(@"\(00\)\ 0000\-0000");

        return celular;
    }

    public static string FormatarCep(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return string.Empty;

        cep = new string(cep.Where(char.IsDigit).ToArray());

        if (cep.Length == 8)
            return Convert.ToUInt64(cep).ToString(@"00\.000\-000");

        return cep;
    }
}

