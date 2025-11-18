namespace BankMore.Application.Exceptions;

public class BusinessException : Exception
{
    public string TipoFalha { get; }

    public BusinessException(string mensagem, string tipoFalha) : base(mensagem)
    {
        TipoFalha = tipoFalha;
    }
}

