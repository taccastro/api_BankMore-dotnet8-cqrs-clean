namespace BankMore.Application.DTOs;

public class SaldoResponse
{
    public string NumeroConta { get; set; } = string.Empty;
    public string NomeTitular { get; set; } = string.Empty;
    public DateTime DataHoraConsulta { get; set; }
    public decimal Saldo { get; set; }
}

