using BankMore.Application.Commands;
using BankMore.Application.Exceptions;
using BankMore.Domain.Entities;
using BankMore.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BankMore.Tests.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _repositoryMock = new Mock<IContaCorrenteRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtServiceMock = new Mock<IJwtService>();
        _handler = new LoginCommandHandler(_repositoryMock.Object, _passwordHasherMock.Object, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task Handle_DeveFazerLoginComSucesso()
    {
        var command = new LoginCommand
        {
            NumeroContaOuCpf = "100001",
            Senha = "senha123"
        };

        var conta = new ContaCorrente
        {
            Id = 1,
            NumeroConta = "100001",
            SenhaHash = "hash123"
        };

        _repositoryMock.Setup(x => x.ObterPorNumeroContaAsync("100001")).ReturnsAsync(conta);
        _passwordHasherMock.Setup(x => x.Verify("senha123", "hash123")).Returns(true);
        _jwtServiceMock.Setup(x => x.GerarToken(1, "100001")).Returns("token123");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Token.Should().Be("token123");
    }

    [Fact]
    public async Task Handle_DeveLancarExcecaoQuandoCredenciaisInvalidas()
    {
        var command = new LoginCommand
        {
            NumeroContaOuCpf = "100001",
            Senha = "senhaErrada"
        };

        _repositoryMock.Setup(x => x.ObterPorNumeroContaAsync("100001")).ReturnsAsync((ContaCorrente?)null);
        _repositoryMock.Setup(x => x.ObterPorCpfAsync("100001")).ReturnsAsync((ContaCorrente?)null);

        await Assert.ThrowsAsync<BusinessException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

