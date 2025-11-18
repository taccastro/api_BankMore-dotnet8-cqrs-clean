using BankMore.Domain.Entities;
using BankMore.Domain.Interfaces;
using BankMore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Infrastructure.Repositories;

public class MovimentoRepository : IMovimentoRepository
{
    private readonly BankMoreDbContext _context;

    public MovimentoRepository(BankMoreDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Movimento movimento)
    {
        await _context.Movimentos.AddAsync(movimento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteMovimentoComIdentificacaoAsync(string identificacaoRequisicao, int contaCorrenteId)
    {
        if (string.IsNullOrWhiteSpace(identificacaoRequisicao))
            return false;

        return await _context.Movimentos
            .AsNoTracking()
            .AnyAsync(m =>
                m.IdentificacaoRequisicao == identificacaoRequisicao &&
                m.ContaCorrenteId == contaCorrenteId);
    }

    public async Task<decimal> CalcularSaldoAsync(int contaCorrenteId)
    {
        var creditos = await _context.Movimentos
            .Where(m => m.ContaCorrenteId == contaCorrenteId && m.Tipo == 'C')
            .SumAsync(m => (double)m.Valor);

        var debitos = await _context.Movimentos
            .Where(m => m.ContaCorrenteId == contaCorrenteId && m.Tipo == 'D')
            .SumAsync(m => (double)m.Valor);

        return (decimal)creditos - (decimal)debitos;
    }
}
