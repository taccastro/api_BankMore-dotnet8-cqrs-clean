using BankMore.Application.Commands;
using BankMore.Application.DTOs;
using BankMore.Application.Exceptions;
using BankMore.Application.Queries;
using BankMore.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContaCorrenteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IContaCorrenteRepository _contaRepository;

    public ContaCorrenteController(
        IMediator mediator,
        IContaCorrenteRepository contaRepository)
    {
        _mediator = mediator;
        _contaRepository = contaRepository;
    }

    [HttpPost("cadastrar")]
    [ProducesResponseType(typeof(CadastrarContaResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarContaRequest request)
    {
        try
        {
            var command = new CadastrarContaCommand
            {
                Cpf = request.Cpf,
                Senha = request.Senha
            };

            var resultado = await _mediator.Send(command);
            return Ok(resultado);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Mensagem = ex.Message,
                TipoFalha = ex.TipoFalha
            });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var command = new LoginCommand
            {
                NumeroContaOuCpf = request.NumeroContaOuCpf,
                Senha = request.Senha
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return Unauthorized(new ErrorResponse
            {
                Mensagem = ex.Message,
                TipoFalha = ex.TipoFalha
            });
        }
    }

    [HttpPost("inativar")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Inativar([FromBody] InativarContaRequest request)
    {
        try
        {
            var contaCorrenteId = ObterContaCorrenteIdDoToken();
            if (!contaCorrenteId.HasValue)
                return StatusCode(403);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = contaCorrenteId.Value,
                Senha = request.Senha
            };

            await _mediator.Send(command);
            return NoContent();
        }
        catch (BusinessException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Mensagem = ex.Message,
                TipoFalha = ex.TipoFalha
            });
        }
    }

    [HttpPost("movimentacao")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Movimentacao([FromBody] MovimentacaoRequest request)
    {
        try
        {
            var contaCorrenteId = ObterContaCorrenteIdDoToken();
            if (!contaCorrenteId.HasValue)
                return StatusCode(403);

            var contaId = request.NumeroConta != null
                ? await ObterContaIdPorNumero(request.NumeroConta)
                : contaCorrenteId.Value;

            if (!contaId.HasValue)
                return BadRequest(new ErrorResponse
                {
                    Mensagem = "Conta não encontrada",
                    TipoFalha = "INVALID_ACCOUNT"
                });

            
            var command = new MovimentarContaCommand(
                request.IdentificacaoRequisicao,
                contaId.Value,
                contaId.Value != contaCorrenteId.Value ? contaId.Value : null,
                request.Valor,
                request.Tipo
            );

            await _mediator.Send(command);
            return NoContent();
        }
        catch (BusinessException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Mensagem = ex.Message,
                TipoFalha = ex.TipoFalha
            });
        }
    }

    [HttpGet("saldo")]
    [Authorize]
    [ProducesResponseType(typeof(SaldoResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Saldo()
    {
        try
        {
            var contaCorrenteId = ObterContaCorrenteIdDoToken();
            if (!contaCorrenteId.HasValue)
                return StatusCode(403);

            var query = new ObterSaldoQuery
            {
                ContaCorrenteId = contaCorrenteId.Value
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Mensagem = ex.Message,
                TipoFalha = ex.TipoFalha
            });
        }
    }

    private int? ObterContaCorrenteIdDoToken()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "contaCorrenteId");
        if (claim == null)
            return null;

        if (int.TryParse(claim.Value, out var contaId))
            return contaId;

        return null;
    }

    private async Task<int?> ObterContaIdPorNumero(string numeroConta)
    {
        var conta = await _contaRepository.ObterPorNumeroContaAsync(numeroConta);
        return conta?.Id;
    }
}

public class InativarContaRequest
{
    public string Senha { get; set; } = string.Empty;
}

public class MovimentacaoRequest
{
    public string IdentificacaoRequisicao { get; set; } = string.Empty;
    public string? NumeroConta { get; set; }
    public decimal Valor { get; set; }
    public char Tipo { get; set; }
}

