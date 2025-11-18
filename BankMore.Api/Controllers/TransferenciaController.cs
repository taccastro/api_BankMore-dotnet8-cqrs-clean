using BankMore.Application.Commands;
using BankMore.Application.DTOs;
using BankMore.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransferenciaController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransferenciaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Transferir([FromBody] TransferenciaRequest request)
    {
        try
        {
            var contaCorrenteId = ObterContaCorrenteIdDoToken();
            if (!contaCorrenteId.HasValue)
                return StatusCode(403);

            var command = new TransferirCommand
            {
                IdentificacaoRequisicao = request.IdentificacaoRequisicao,
                ContaOrigemId = contaCorrenteId.Value,
                NumeroContaDestino = request.NumeroContaDestino,
                Valor = request.Valor
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

    private int? ObterContaCorrenteIdDoToken()
    {
        var contaIdClaim = User.Claims.FirstOrDefault(c => c.Type == "contaCorrenteId");
        if (contaIdClaim != null && int.TryParse(contaIdClaim.Value, out var id))
            return id;
        return null;
    }
}

public class TransferenciaRequest
{
    public string IdentificacaoRequisicao { get; set; } = string.Empty;
    public string NumeroContaDestino { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

