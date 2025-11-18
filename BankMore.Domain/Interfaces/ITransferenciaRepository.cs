namespace BankMore.Domain.Interfaces;

public interface ITransferenciaRepository
{
    Task AdicionarAsync(Entities.Transferencia transferencia);
    Task<bool> ExisteTransferenciaComIdentificacaoAsync(string identificacaoRequisicao);
}

