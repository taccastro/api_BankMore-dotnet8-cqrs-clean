using BankMore.Application.Exceptions;
using BankMore.Application.Queries;
using BankMore.Domain.Entities;
using BankMore.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BankMore.Tests.Queries;

public class ObterSaldoQueryHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _contaRepositoryMock;
    private readonly Mock<IMovimentoRepository> _movimentoRepositoryMock;
    private readonly ObterSaldoQueryHandler _handler;

    public ObterSaldoQueryHandlerTests()
    {
        _contaRepositoryMock = new Mock<IContaCorrenteRepository>();
        _movimentoRepositoryMock = new Mock<IMovimentoRepository>();
        _handler = new ObterSaldoQueryHandler(_contaRepositoryMock.Object, _movimentoRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSaldoComSucesso()
    {
        var query = new ObterSaldoQuery { ContaCorrenteId = 1 };

        var conta = new ContaCorrente
        {
            Id = 1,
            NumeroConta = "100001",
            NomeTitular = "Titular",
            Ativo = true
        };

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(conta);
        _movimentoRepositoryMock.Setup(x => x.CalcularSaldoAsync(1)).ReturnsAsync(500);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.NumeroConta.Should().Be("100001");
        result.Saldo.Should().Be(500);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecaoQuandoContaNaoExiste()
    {
        var query = new ObterSaldoQuery { ContaCorrenteId = 999 };

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(999)).ReturnsAsync((ContaCorrente?)null);

        await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(query, CancellationToken.None));
    }
}

