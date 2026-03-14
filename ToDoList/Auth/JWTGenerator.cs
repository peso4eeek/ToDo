using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Thinktecture;
using ToDoList.User;

namespace ToDoList.Auth;

public class JWTGenerator(JwtSecurityTokenHandler tokenHandler)
{
    public AccessToken GenerateAccessToken(Session session, AuthOptions authOptions)
    {
        var now = DateTime.UtcNow;
        var claims = new List<Claim>([
            new Claim("UserId",  session.UserId.ToString()), 
            new Claim("SessionId",  session.SessionId.ToString())
        ]);
        var jwt = new JwtSecurityToken(
            notBefore: now,
            claims: claims,
            expires: now.AddMinutes(authOptions.AccessTokenLifetimeMinutes),
            signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        );

        return AccessToken.Create(tokenHandler.WriteToken(jwt));
    }

    public RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return RefreshToken.Create(Convert.ToBase64String(randomNumber));
        }
        
    }
}


[ValueObject<string>]
public readonly partial struct AccessToken
{
    static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            validationError = new ValidationError("Empty token");
        }
    }
}
[ValueObject<string>]
public readonly partial struct RefreshToken;