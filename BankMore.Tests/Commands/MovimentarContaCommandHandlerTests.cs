using BankMore.Application.Commands;
using BankMore.Application.Exceptions;
using BankMore.Domain.Entities;
using BankMore.Domain.Interfaces;
using Moq;
using Xunit;

namespace BankMore.Tests.Commands;

public class MovimentarContaCommandHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _contaRepositoryMock;
    private readonly Mock<IMovimentoRepository> _movimentoRepositoryMock;
    private readonly MovimentarContaCommandHandler _handler;

    public MovimentarContaCommandHandlerTests()
    {
        _contaRepositoryMock = new Mock<IContaCorrenteRepository>();
        _movimentoRepositoryMock = new Mock<IMovimentoRepository>();
        _handler = new MovimentarContaCommandHandler(_contaRepositoryMock.Object, _movimentoRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveCriarMovimentoComSucesso()
    {
        var command = new MovimentarContaCommand(
            identificacaoRequisicao: "req123",
            contaCorrenteId: 1,
            contaDestinoId: null,
            valor: 100,
            tipo: 'C'
        );

        var conta = new ContaCorrente { Id = 1, Ativo = true };
        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(conta);
        _movimentoRepositoryMock.Setup(x => x.ExisteMovimentoComIdentificacaoAsync("req123", 1)).ReturnsAsync(false);

        await _handler.Handle(command, CancellationToken.None);

        _movimentoRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Movimento>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecaoQuandoContaNaoExiste()
    {
        var command = new MovimentarContaCommand(
            identificacaoRequisicao: "req123",
            contaCorrenteId: 999,
            contaDestinoId: null,
            valor: 100,
            tipo: 'C'
        );

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(999)).ReturnsAsync((ContaCorrente?)null);

        await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DeveLancarExcecaoQuandoContaInativa()
    {
        var command = new MovimentarContaCommand(
            identificacaoRequisicao: "req123",
            contaCorrenteId: 1,
            contaDestinoId: null,
            valor: 100,
            tipo: 'C'
        );

        var conta = new ContaCorrente { Id = 1, Ativo = false };
        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(conta);

        await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DeveSerIdempotente()
    {
        var command = new MovimentarContaCommand(
            identificacaoRequisicao: "req123",
            contaCorrenteId: 1,
            contaDestinoId: null,
            valor: 100,
            tipo: 'C'
        );

        var conta = new ContaCorrente { Id = 1, Ativo = true };
        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(conta);
        _movimentoRepositoryMock.Setup(x => x.ExisteMovimentoComIdentificacaoAsync("req123", 1)).ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        _movimentoRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Movimento>()), Times.Never);
    }
}
