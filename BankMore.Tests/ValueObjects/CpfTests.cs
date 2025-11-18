using BankMore.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BankMore.Tests.ValueObjects;

public class CpfTests
{
    [Fact]
    public void Cpf_DeveAceitarCpfValido()
    {
        var cpf = new Cpf("11144477735");
        cpf.Valor.Should().Be("11144477735");
    }

    [Fact]
    public void Cpf_DeveLancarExcecaoQuandoInvalido()
    {
        Assert.Throws<ArgumentException>(() => new Cpf("123"));
    }

    [Fact]
    public void Cpf_DeveRemoverFormatacao()
    {
        var cpf = new Cpf("111.444.777-35");
        cpf.Valor.Should().Be("11144477735");
    }
}

