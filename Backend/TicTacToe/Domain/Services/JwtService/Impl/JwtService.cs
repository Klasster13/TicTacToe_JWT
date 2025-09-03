using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain.Services.JwtService.Impl;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;


    public string GenerateAccessToken(User user)
    {
        var claims = CreateClaims(user);

        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:AccessSecret"]
                ?? throw new InvalidDataException("Jwt Access key configuration is invalid.")
                ));

        var signingCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

        var expireTime = DateTime.UtcNow.AddMinutes(
            Convert.ToDouble(_configuration["Jwt:AccessExpirationMinutes"]
            ?? throw new InvalidDataException("Jwt Access expire time configuration is invalid.")
            ));

        return CreateToken(claims, expireTime, signingCredentials);
    }


    public string GenerateRefreshToken(User user)
    {
        var claims = CreateClaims(user);

        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:RefreshSecret"]
                ?? throw new InvalidDataException("Jwt Access key configuration is invalid.")
                ));

        var signingCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

        var expireTime = DateTime.UtcNow.AddDays(
            Convert.ToDouble(_configuration["Jwt:RefreshExpirationDays"]
            ?? throw new InvalidDataException("Jwt Refresh expire time configuration is invalid.")
            ));

        return CreateToken(claims, expireTime, signingCredentials);
    }


    public ClaimsPrincipal GetClaims(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token), "Token cannot be null or empty");

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims;

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        }
        catch
        {
            throw new SecurityTokenException("Invalid token format");
        }
    }


    private static List<Claim> CreateClaims(User user) =>
    [
        new (ClaimTypes.NameIdentifier, user.Id.ToString()),
        new (ClaimTypes.Name, user.Login)
    ];


    private string CreateToken(List<Claim> claims, DateTime expireTime, SigningCredentials signingCredentials)
    {
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expireTime,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public bool ValidateAccessToken(string accessToken)
    {
        var secretKey = _configuration["Jwt:AccessSecret"];

        if (string.IsNullOrEmpty(secretKey)) return false;

        return ValidateToken(accessToken, secretKey);
    }


    public bool ValidateRefreshToken(string refreshToken)
    {
        var secretKey = _configuration["Jwt:RefreshSecret"];

        if (string.IsNullOrEmpty(secretKey)) return false;

        return ValidateToken(refreshToken, secretKey);
    }


    private bool ValidateToken(string token, string secretKey)
    {
        if (string.IsNullOrEmpty(token)) return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, parameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
