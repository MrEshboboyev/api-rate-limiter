using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.Security;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

internal sealed class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    #region Private fields

    // Holds the JWT options configuration
    private readonly JwtOptions _options = options.Value;

    #endregion

    public string Generate(User user)
    {
        #region Create Claims List

        // Create a list of claims for the JWT
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value)
        };

        #endregion

        #region Create signing credentials

        // Create signing credentials using the secret key from options
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        #endregion

        #region New Jwt Security token

        // Create the JWT security token
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        #endregion

        #region Create Jwt Token

        // Write the token to a string
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        #endregion

        return tokenValue;
    }
}