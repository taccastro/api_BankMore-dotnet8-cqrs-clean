namespace BankMore.Application.DTOs;

public class LoginRequest
{
    public string NumeroContaOuCpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

