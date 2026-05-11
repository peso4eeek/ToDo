using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Options;

using ToDoList.Auth;
namespace ToDoListTests;

public class JWTGeneratorTests
{
    [Fact]
    public void GenerateRefreshToken_returns_non_empty_value()
    {
        var sut = new JWTGenerator(new JwtSecurityTokenHandler());

        var token = sut.GenerateRefreshToken();

        Assert.False(string.IsNullOrWhiteSpace(token.ToString()));
    }

    [Fact]
    public void GenerateAccessToken_returns_jwt_with_claims()
    {
        var sut = new JWTGenerator(new JwtSecurityTokenHandler());
        var options = Options.Create(new AuthOptions { Key = TestAuth.SigningKey });
        var session = new ToDoList.User.Session
        {
            UserId = ToDoList.User.UserId.Create(Guid.NewGuid()),
            RefreshToken = sut.GenerateRefreshToken(),
            IsActive = true
        };

        var access = sut.GenerateAccessToken(session, options.Value);

        Assert.False(string.IsNullOrWhiteSpace(access.ToString()));
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(access.ToString());
        Assert.Contains(jwt.Claims, c => c.Type == "UserId");
        Assert.Contains(jwt.Claims, c => c.Type == "SessionId");
    }
}