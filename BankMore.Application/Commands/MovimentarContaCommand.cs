using MediatR;

namespace BankMore.Application.Commands;

public class MovimentarContaCommand : IRequest
{
    public string IdentificacaoRequisicao { get; }
    public int ContaCorrenteId { get; }
    public int? ContaDestinoId { get; }
    public decimal Valor { get; }
    public char Tipo { get; } // C = Crédito, D = Débito

    public MovimentarContaCommand(
        string identificacaoRequisicao,
        int contaCorrenteId,
        int? contaDestinoId,
        decimal valor,
        char tipo)
    {
        IdentificacaoRequisicao = identificacaoRequisicao;
        ContaCorrenteId = contaCorrenteId;
        ContaDestinoId = contaDestinoId;
        Valor = valor;
        Tipo = tipo;
    }
}
