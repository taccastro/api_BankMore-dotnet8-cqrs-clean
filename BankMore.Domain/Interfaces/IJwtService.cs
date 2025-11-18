namespace BankMore.Domain.Interfaces;

public interface IJwtService
{
    string GerarToken(int contaCorrenteId, string numeroConta);
    int? ObterContaCorrenteIdDoToken(string token);
}

