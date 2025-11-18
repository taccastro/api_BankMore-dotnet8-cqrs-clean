using BankMore.Application.DTOs;
using MediatR;

namespace BankMore.Application.Commands;

public class LoginCommand : IRequest<LoginResponse>
{
    public string NumeroContaOuCpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

