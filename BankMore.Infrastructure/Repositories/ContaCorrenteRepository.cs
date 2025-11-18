using BankMore.Domain.Entities;
using BankMore.Domain.Interfaces;
using BankMore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Infrastructure.Repositories;

public class ContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly BankMoreDbContext _context;
    public ContaCorrenteRepository(BankMoreDbContext context)
    {
        _context = context;
    }

    public async Task<ContaCorrente?> ObterPorCpfAsync(string cpf)
    {
        return await _context.ContasCorrentes
            .FirstOrDefaultAsync(c => c.Cpf == cpf);
    }

    public async Task<ContaCorrente?> ObterPorNumeroContaAsync(string numeroConta)
    {
        return await _context.ContasCorrentes
            .FirstOrDefaultAsync(c => c.NumeroConta == numeroConta);
    }

    public async Task<ContaCorrente?> ObterPorIdAsync(int id)
    {
        return await _context.ContasCorrentes
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<string> ObterProximoNumeroContaAsync()
    {
        var ultimaConta = await _context.ContasCorrentes
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        if (ultimaConta == null)
            return "100000"; // Primeira conta

        // Tenta incrementar o número da conta
        if (int.TryParse(ultimaConta.NumeroConta, out var numero))
            return (numero + 1).ToString();

        // Fallback: gera número baseado em timestamp
        return DateTime.UtcNow.Ticks.ToString().Substring(0, 10);
    }

    public async Task AdicionarAsync(ContaCorrente conta)
    {
        await _context.ContasCorrentes.AddAsync(conta);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(ContaCorrente conta)
    {
        _context.ContasCorrentes.Update(conta);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteMovimentoComIdentificacaoAsync(string identificacaoRequisicao, int contaCorrenteId)
    {
        return await _context.Movimentos
            .AnyAsync(m => m.IdentificacaoRequisicao == identificacaoRequisicao &&
                          m.ContaCorrenteId == contaCorrenteId);
    }
}

