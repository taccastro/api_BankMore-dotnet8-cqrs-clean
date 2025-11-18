using MediatR;

namespace BankMore.Application.Commands;

public class InativarContaCommand : IRequest
{
    public int ContaCorrenteId { get; set; }
    public string Senha { get; set; } = string.Empty;
}

