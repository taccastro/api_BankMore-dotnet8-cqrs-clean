using BankMore.Application.Commands;
using BankMore.Application.Exceptions;
using BankMore.Domain.Entities;
using BankMore.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BankMore.Tests.Commands;

public class CadastrarContaCommandHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly CadastrarContaCommandHandler _handler;

    public CadastrarContaCommandHandlerTests()
    {
        _repositoryMock = new Mock<IContaCorrenteRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _handler = new CadastrarContaCommandHandler(_repositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task Handle_DeveCadastrarContaComSucesso()
    {
        var command = new CadastrarContaCommand
        {
            Cpf = "11144477735",
            Senha = "senha123"
        };

        _repositoryMock.Setup(x => x.ObterPorCpfAsync(It.IsAny<string>())).ReturnsAsync((ContaCorrente?)null);
        _repositoryMock.Setup(x => x.ObterProximoNumeroContaAsync()).ReturnsAsync("100001");
        _passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>())).Returns("hash123");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.NumeroConta.Should().Be("100001");
        _repositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<ContaCorrente>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveLancarExcecaoQuandoCpfInvalido()
    {
        var command = new CadastrarContaCommand
        {
            Cpf = "123",
            Senha = "senha123"
        };

        await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DeveLancarExcecaoQuandoContaJaExiste()
    {
        var command = new CadastrarContaCommand
        {
            Cpf = "11144477735",
            Senha = "senha123"
        };

        var contaExistente = new ContaCorrente { Cpf = "11144477735" };
        _repositoryMock.Setup(x => x.ObterPorCpfAsync(It.IsAny<string>())).ReturnsAsync(contaExistente);

        await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

