using BankMore.Application.DTOs;
using BankMore.Application.Exceptions;
using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Queries;

public class ObterSaldoQueryHandler : IRequestHandler<ObterSaldoQuery, SaldoResponse>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IMovimentoRepository _movimentoRepository;

    public ObterSaldoQueryHandler(
        IContaCorrenteRepository contaRepository,
        IMovimentoRepository movimentoRepository)
    {
        _contaRepository = contaRepository;
        _movimentoRepository = movimentoRepository;
    }

    public async Task<SaldoResponse> Handle(ObterSaldoQuery request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.ObterPorIdAsync(request.ContaCorrenteId);

        if (conta == null)
            throw new BusinessException("Conta não encontrada", "INVALID_ACCOUNT");

        if (!conta.Ativo)
            throw new BusinessException("Conta inativa", "INACTIVE_ACCOUNT");

        // Calcula saldo: soma dos créditos menos débitos
        var saldo = await _movimentoRepository.CalcularSaldoAsync(request.ContaCorrenteId);

        return new SaldoResponse
        {
            NumeroConta = conta.NumeroConta,
            NomeTitular = conta.NomeTitular,
            DataHoraConsulta = DateTime.UtcNow,
            Saldo = saldo
        };
    }
}

