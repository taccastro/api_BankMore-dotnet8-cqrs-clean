namespace BankMore.Domain.Entities;

public class Movimento
{
    public int Id { get; set; }
    public int ContaCorrenteId { get; set; }
    public string IdentificacaoRequisicao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public char Tipo { get; set; } // C = Crédito, D = Débito
    public DateTime DataMovimento { get; set; } = DateTime.UtcNow;

    public ContaCorrente ContaCorrente { get; set; } = null!;
}

