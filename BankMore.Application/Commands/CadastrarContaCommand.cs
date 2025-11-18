using BankMore.Application.DTOs;
using MediatR;

namespace BankMore.Application.Commands;

public class CadastrarContaCommand : IRequest<CadastrarContaResponse>
{
    public string Cpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

