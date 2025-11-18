namespace BankMore.Domain.Interfaces;

public interface IContaCorrenteRepository
{
    Task<Entities.ContaCorrente?> ObterPorCpfAsync(string cpf);
    Task<Entities.ContaCorrente?> ObterPorNumeroContaAsync(string numeroConta);
    Task<Entities.ContaCorrente?> ObterPorIdAsync(int id);
    Task<string> ObterProximoNumeroContaAsync();
    Task AdicionarAsync(Entities.ContaCorrente conta);
    Task AtualizarAsync(Entities.ContaCorrente conta);
    Task<bool> ExisteMovimentoComIdentificacaoAsync(string identificacaoRequisicao, int contaCorrenteId);
}

