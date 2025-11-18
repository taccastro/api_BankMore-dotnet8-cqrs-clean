public class MovimentarContaDto
{
    public string IdentificacaoRequisicao { get; set; } = string.Empty;
    public int ContaCorrenteId { get; set; }
    public int? ContaDestinoId { get; set; }
    public decimal Valor { get; set; }
    public char Tipo { get; set; }
}
