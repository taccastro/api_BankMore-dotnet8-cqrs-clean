using BankMore.Application.Exceptions;
using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Commands;

public class MovimentarContaCommandHandler : IRequestHandler<MovimentarContaCommand>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IMovimentoRepository _movimentoRepository;

    public MovimentarContaCommandHandler(
        IContaCorrenteRepository contaRepository,
        IMovimentoRepository movimentoRepository)
    {
        _contaRepository = contaRepository;
        _movimentoRepository = movimentoRepository;
    }

    public async Task Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
    {
        // Validações
        var conta = await _contaRepository.ObterPorIdAsync(request.ContaCorrenteId);

        if (conta == null)
            throw new BusinessException("Conta não encontrada", "INVALID_ACCOUNT");

        if (!conta.Ativo)
            throw new BusinessException("Conta inativa", "INACTIVE_ACCOUNT");

        if (request.Valor <= 0)
            throw new BusinessException("Valor deve ser positivo", "INVALID_VALUE");

        if (request.Tipo != 'C' && request.Tipo != 'D')
            throw new BusinessException("Tipo de movimento inválido", "INVALID_TYPE");

        // Validação específica para transferências
        if (request.ContaDestinoId.HasValue && request.Tipo != 'C')
            throw new BusinessException("Apenas crédito pode ser realizado em conta diferente", "INVALID_TYPE");

        // Idempotência - verifica se já existe movimento com essa identificação
        var jaExiste = await _movimentoRepository.ExisteMovimentoComIdentificacaoAsync(
            request.IdentificacaoRequisicao,
            request.ContaCorrenteId);

        if (jaExiste)
            return;

        var movimento = new Domain.Entities.Movimento
        {
            ContaCorrenteId = request.ContaCorrenteId,
            IdentificacaoRequisicao = request.IdentificacaoRequisicao,
            Valor = request.Valor,
            Tipo = request.Tipo,
            DataMovimento = DateTime.UtcNow
        };

        await _movimentoRepository.AdicionarAsync(movimento);
    }
}

