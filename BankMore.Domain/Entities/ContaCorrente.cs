namespace BankMore.Domain.Entities;

public class ContaCorrente
{
    public int Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string NumeroConta { get; set; } = string.Empty;
    public string NomeTitular { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public ICollection<Movimento> Movimentos { get; set; } = new List<Movimento>();
    public ICollection<Transferencia> TransferenciasOrigem { get; set; } = new List<Transferencia>();
    public ICollection<Transferencia> TransferenciasDestino { get; set; } = new List<Transferencia>();
}

