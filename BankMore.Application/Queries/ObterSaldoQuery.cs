using BankMore.Application.DTOs;
using MediatR;

namespace BankMore.Application.Queries;

public class ObterSaldoQuery : IRequest<SaldoResponse>
{
    public int ContaCorrenteId { get; set; }
}

