namespace BankMore.Domain.ValueObjects;

public class Cpf
{
    public string Valor { get; private set; }

    public Cpf(string cpf)
    {
        if (!Validar(cpf))
            throw new ArgumentException("CPF inválido", nameof(cpf));

        Valor = Limpar(cpf);
    }

    private static string Limpar(string cpf)
    {
        return new string(cpf.Where(char.IsDigit).ToArray());
    }

    private static bool Validar(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cpfLimpo = Limpar(cpf);

        if (cpfLimpo.Length != 11)
            return false;

        if (cpfLimpo.All(c => c == cpfLimpo[0]))
            return false;

        var soma = 0;
        for (int i = 0; i < 9; i++)
            soma += int.Parse(cpfLimpo[i].ToString()) * (10 - i);

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpfLimpo[9].ToString()) != digito1)
            return false;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(cpfLimpo[i].ToString()) * (11 - i);

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cpfLimpo[10].ToString()) == digito2;
    }

    public override string ToString() => Valor;
}

