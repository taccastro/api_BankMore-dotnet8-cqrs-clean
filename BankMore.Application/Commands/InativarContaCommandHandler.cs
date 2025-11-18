using BankMore.Application.Exceptions;
using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Commands;

public class InativarContaCommandHandler : IRequestHandler<InativarContaCommand>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public InativarContaCommandHandler(IContaCorrenteRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(InativarContaCommand request, CancellationToken cancellationToken)
    {
        var conta = await _repository.ObterPorIdAsync(request.ContaCorrenteId);

        if (conta == null)
            throw new BusinessException("Conta não encontrada", "INVALID_ACCOUNT");

        if (!_passwordHasher.Verify(request.Senha, conta.SenhaHash))
            throw new BusinessException("Senha inválida", "USER_UNAUTHORIZED");

        conta.Ativo = false;
        await _repository.AtualizarAsync(conta);
    }
}

