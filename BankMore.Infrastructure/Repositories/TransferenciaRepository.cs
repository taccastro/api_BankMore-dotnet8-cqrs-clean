using BankMore.Domain.Entities;
using BankMore.Domain.Interfaces;
using BankMore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Infrastructure.Repositories;

public class TransferenciaRepository : ITransferenciaRepository
{
    private readonly BankMoreDbContext _context;

    public TransferenciaRepository(BankMoreDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Transferencia transferencia)
    {
        await _context.Transferencias.AddAsync(transferencia);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteTransferenciaComIdentificacaoAsync(string identificacaoRequisicao)
    {
        return await _context.Transferencias
            .AnyAsync(t => t.IdentificacaoRequisicao == identificacaoRequisicao);
    }
}

