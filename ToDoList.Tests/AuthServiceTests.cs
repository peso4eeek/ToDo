using System.IdentityModel.Tokens.Jwt;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using ToDoList.Auth;
using ToDoList.Infrastructure;

using AppUser = ToDoList.User.User;

namespace ToDoListTests;

public class AuthServiceTests
{
    private static ToDoContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ToDoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ToDoContext(options);
    }

    private static AuthService CreateSut(ToDoContext db)
    {
        var authOptions = Options.Create(new AuthOptions { Key = TestAuth.SigningKey });
        var jwt = new JWTGenerator(new JwtSecurityTokenHandler());
        return new AuthService(authOptions, db, jwt);
    }

    [Fact]
    public async Task Register_returns_failure_when_passwords_differ()
    {
        await using var db = CreateContext();
        var sut = CreateSut(db);

        var result = await sut.Register(new RegisterRequest(
            "a@b.c",
            "secret1",
            "user1",
            "secret2"));

        Assert.True(result.IsFailure);
        Assert.Equal("Passwords do not match", result.Error);
    }

    [Fact]
    public async Task Register_persists_user_when_passwords_match()
    {
        await using var db = CreateContext();
        var sut = CreateSut(db);

        var result = await sut.Register(new RegisterRequest(
            "x@y.z",
            "same",
            "reguser",
            "same"));

        Assert.True(result.IsSuccess);
        Assert.Single(db.Users);
        Assert.Equal("reguser", db.Users.Single().Name);
    }

    [Fact]
    public async Task Login_returns_failure_for_unknown_user()
    {
        await using var db = CreateContext();
        var sut = CreateSut(db);

        var result = await sut.Login(new LoginRequest("nobody", "pwd"));

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Login_returns_tokens_and_stores_session()
    {
        await using var db = CreateContext();
        db.Users.Add(new AppUser
        {
            Name = "loginuser",
            PassHash = BCrypt.Net.BCrypt.HashPassword("correct"),
            Email = "e@e.e"
        });
        await db.SaveChangesAsync();

        var sut = CreateSut(db);

        var result = await sut.Login(new LoginRequest("loginuser", "correct"));

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.AccessToken.ToString());
        Assert.NotEmpty(result.Value.RefreshToken.ToString());
        Assert.Single(db.Sessions);
    }

    [Fact]
    public async Task Refresh_returns_failure_for_unknown_token()
    {
        await using var db = CreateContext();
        var sut = CreateSut(db);

        var bad = RefreshToken.Create(Convert.ToBase64String(Guid.NewGuid().ToByteArray()));
        var result = await sut.Refresh(bad);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Refresh_rotates_refresh_token()
    {
        await using var db = CreateContext();
        db.Users.Add(new AppUser
        {
            Name = "u",
            PassHash = BCrypt.Net.BCrypt.HashPassword("p"),
            Email = "m@m.m"
        });
        await db.SaveChangesAsync();

        var sut = CreateSut(db);
        var login = await sut.Login(new LoginRequest("u", "p"));
        Assert.True(login.IsSuccess);
        var oldRefresh = login.Value.RefreshToken;

        var refreshed = await sut.Refresh(oldRefresh);

        Assert.True(refreshed.IsSuccess);
        Assert.NotEqual(oldRefresh.ToString(), refreshed.Value.RefreshToken.ToString());
    }
}