namespace BankMore.Application.DTOs;

public class ErrorResponse
{
    public string Mensagem { get; set; } = string.Empty;
    public string TipoFalha { get; set; } = string.Empty;
}

