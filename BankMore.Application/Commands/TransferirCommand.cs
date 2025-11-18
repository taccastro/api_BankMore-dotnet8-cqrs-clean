using MediatR;

namespace BankMore.Application.Commands;

public class TransferirCommand : IRequest
{
    public string IdentificacaoRequisicao { get; set; } = string.Empty;
    public int ContaOrigemId { get; set; }
    public string NumeroContaDestino { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

