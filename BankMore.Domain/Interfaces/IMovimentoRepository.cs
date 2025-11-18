namespace BankMore.Domain.Interfaces;

public interface IMovimentoRepository
{
    Task AdicionarAsync(Entities.Movimento movimento);
    Task<bool> ExisteMovimentoComIdentificacaoAsync(string identificacaoRequisicao, int contaCorrenteId);
    Task<decimal> CalcularSaldoAsync(int contaCorrenteId);
}

