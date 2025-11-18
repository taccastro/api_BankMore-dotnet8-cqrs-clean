using BankMore.Application.DTOs;
using BankMore.Application.Exceptions;
using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IContaCorrenteRepository repository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var conta = await ObterContaAsync(request.NumeroContaOuCpf);

        if (conta == null)
            throw new BusinessException("Credenciais inválidas", "USER_UNAUTHORIZED");

        if (!_passwordHasher.Verify(request.Senha, conta.SenhaHash))
            throw new BusinessException("Credenciais inválidas", "USER_UNAUTHORIZED");

        var token = _jwtService.GerarToken(conta.Id, conta.NumeroConta);

        return new LoginResponse { Token = token };
    }

    private async Task<Domain.Entities.ContaCorrente?> ObterContaAsync(string numeroContaOuCpf)
    {
        // Tenta buscar por número da conta primeiro
        var conta = await _repository.ObterPorNumeroContaAsync(numeroContaOuCpf);
        if (conta != null)
            return conta;

        // Se não encontrar, tenta por CPF
        return await _repository.ObterPorCpfAsync(numeroContaOuCpf);
    }
}

