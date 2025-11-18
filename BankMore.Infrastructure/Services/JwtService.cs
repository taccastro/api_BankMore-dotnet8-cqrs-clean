using BankMore.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankMore.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:SecretKey"] ?? "MinhaChaveSecretaSuperSeguraParaJWT2024";
        _issuer = configuration["Jwt:Issuer"] ?? "BankMore";
        _audience = configuration["Jwt:Audience"] ?? "BankMore";
    }

    public string GerarToken(int contaCorrenteId, string numeroConta)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var claims = new[]
        {
            new Claim("contaCorrenteId", contaCorrenteId.ToString()),
            new Claim("numeroConta", numeroConta),
            new Claim(ClaimTypes.NameIdentifier, contaCorrenteId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24), // Token válido por 24 horas
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public int? ObterContaCorrenteIdDoToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var contaCorrenteIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "contaCorrenteId");

            if (contaCorrenteIdClaim != null && int.TryParse(contaCorrenteIdClaim.Value, out var id))
                return id;

            return null;
        }
        catch
        {
            return null;
        }
    }
}

