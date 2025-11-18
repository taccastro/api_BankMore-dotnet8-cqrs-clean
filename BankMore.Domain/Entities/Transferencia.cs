namespace BankMore.Domain.Entities;

public class Transferencia
{
    public int Id { get; set; }
    public string IdentificacaoRequisicao { get; set; } = string.Empty;
    public int ContaOrigemId { get; set; }
    public int ContaDestinoId { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataTransferencia { get; set; } = DateTime.UtcNow;

    public ContaCorrente ContaOrigem { get; set; } = null!;
    public ContaCorrente ContaDestino { get; set; } = null!;
}

