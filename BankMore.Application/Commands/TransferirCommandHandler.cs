using BankMore.Application.Exceptions;
using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Commands;

public class TransferirCommandHandler : IRequestHandler<TransferirCommand>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly ITransferenciaRepository _transferenciaRepository;
    private readonly IMovimentoRepository _movimentoRepository;
    private readonly IMediator _mediator;

    public TransferirCommandHandler(
        IContaCorrenteRepository contaRepository,
        ITransferenciaRepository transferenciaRepository,
        IMovimentoRepository movimentoRepository,
        IMediator mediator)
    {
        _contaRepository = contaRepository;
        _transferenciaRepository = transferenciaRepository;
        _movimentoRepository = movimentoRepository;
        _mediator = mediator;
    }

    public async Task Handle(TransferirCommand request, CancellationToken cancellationToken)
    {
        // Idempotência - se já processou essa transferência, retorna
        var jaProcessado = await _transferenciaRepository.ExisteTransferenciaComIdentificacaoAsync(request.IdentificacaoRequisicao);
        if (jaProcessado)
            return;

        // Valida conta origem
        var contaOrigem = await _contaRepository.ObterPorIdAsync(request.ContaOrigemId);
        if (contaOrigem == null || !contaOrigem.Ativo)
            throw new BusinessException("Conta origem inválida ou inativa", "INVALID_ACCOUNT");

        // Valida conta destino
        var contaDestino = await _contaRepository.ObterPorNumeroContaAsync(request.NumeroContaDestino);
        if (contaDestino == null || !contaDestino.Ativo)
            throw new BusinessException("Conta destino inválida ou inativa", "INVALID_ACCOUNT");

        if (request.Valor <= 0)
            throw new BusinessException("Valor deve ser positivo", "INVALID_VALUE");

        try
        {
            // Débito na conta origem
            await _mediator.Send(new MovimentarContaCommand(
                identificacaoRequisicao: $"{request.IdentificacaoRequisicao}-D",
                contaCorrenteId: request.ContaOrigemId,
                contaDestinoId: null,
                valor: request.Valor,
                tipo: 'D'
            ), cancellationToken);

            // Crédito na conta destino
            await _mediator.Send(new MovimentarContaCommand(
                identificacaoRequisicao: $"{request.IdentificacaoRequisicao}-C",
                contaCorrenteId: contaDestino.Id,
                contaDestinoId: contaDestino.Id,
                valor: request.Valor,
                tipo: 'C'
            ), cancellationToken);

            // Registra a transferência
            var transferencia = new Domain.Entities.Transferencia
            {
                IdentificacaoRequisicao = request.IdentificacaoRequisicao,
                ContaOrigemId = request.ContaOrigemId,
                ContaDestinoId = contaDestino.Id,
                Valor = request.Valor,
                DataTransferencia = DateTime.UtcNow
            };

            await _transferenciaRepository.AdicionarAsync(transferencia);
        }
        catch (BusinessException)
        {
            // Estorna o débito em caso de falha
            await _mediator.Send(new MovimentarContaCommand(
                identificacaoRequisicao: $"{request.IdentificacaoRequisicao}-ESTORNO",
                contaCorrenteId: request.ContaOrigemId,
                contaDestinoId: null,
                valor: request.Valor,
                tipo: 'C'
            ), cancellationToken);
            throw;
        }
    }
}
