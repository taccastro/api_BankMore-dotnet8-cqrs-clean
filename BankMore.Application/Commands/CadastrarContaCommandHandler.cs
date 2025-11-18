using BankMore.Application.DTOs;
using BankMore.Application.Exceptions;
using BankMore.Domain.Interfaces;
using BankMore.Domain.ValueObjects;
using MediatR;

namespace BankMore.Application.Commands;

public class CadastrarContaCommandHandler : IRequestHandler<CadastrarContaCommand, CadastrarContaResponse>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public CadastrarContaCommandHandler(IContaCorrenteRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<CadastrarContaResponse> Handle(CadastrarContaCommand request, CancellationToken cancellationToken)
    {
        // Valida CPF
        Cpf cpf;
        try
        {
            cpf = new Cpf(request.Cpf);
        }
        catch (ArgumentException)
        {
            throw new BusinessException("CPF inválido", "INVALID_DOCUMENT");
        }

        // Verifica se já existe conta para este CPF
        var contaExistente = await _repository.ObterPorCpfAsync(cpf.Valor);
        if (contaExistente != null)
            throw new BusinessException("Conta já cadastrada para este CPF", "INVALID_DOCUMENT");

        var numeroConta = await _repository.ObterProximoNumeroContaAsync();
        var senhaHash = _passwordHasher.Hash(request.Senha);

        var conta = new Domain.Entities.ContaCorrente
        {
            Cpf = cpf.Valor,
            SenhaHash = senhaHash,
            NumeroConta = numeroConta,
            NomeTitular = "Titular", // TODO: implementar campo de nome no cadastro
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await _repository.AdicionarAsync(conta);

        return new CadastrarContaResponse { NumeroConta = numeroConta };
    }
}

