using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WhatIsThisThing.Server.Auth;

public class TokenService
{
    private readonly IOptions<JwtConfig> _jwtConfig;

    public TokenService(IOptions<JwtConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig;
    }

    public string GenerateJwtToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.Value.SecurityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, username));
        //claims.Add(new Claim(ClaimTypes.Role, permission));

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Value.Issuer,
            audience: _jwtConfig.Value.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}