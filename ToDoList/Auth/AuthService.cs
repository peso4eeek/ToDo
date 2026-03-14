using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ToDoList.User;

namespace ToDoList.Auth;

public class AuthService(IOptions<AuthOptions> options, ToDoContext dbContext, JWTGenerator jwtGenerator)
{

    public async Task<Result> Register(RegisterRequest registerData)
    {
        if (!string.Equals(registerData.Password, registerData.PasswordRepeat))
        {
            return Result.Failure("Passwords do not match");
        }
        var passHash = BCrypt.Net.BCrypt.HashPassword(registerData.Password);
        var user = new User.User
        {
            Email = registerData.Email,
            Name = registerData.Name,
            PassHash = passHash,
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<AuthData>> Login(LoginRequest loginData)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Name == loginData.Name);
        if (user == null)
        {
            return Result.Failure<AuthData>("Invalid username or password");
        }
        
        if (!BCrypt.Net.BCrypt.Verify(loginData.Password, user.PassHash ))
        {
            return Result.Failure<AuthData>("Invalid username or password");
        }

        var refreshToken = jwtGenerator.GenerateRefreshToken();
        var session = new Session
        {
            UserId = user.UserId,
            RefreshToken = refreshToken,
            IsActive = true,
        };
        await dbContext.Sessions.AddAsync(session);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(session).ReloadAsync();

        var accessToken = jwtGenerator.GenerateAccessToken(session, options.Value);
        return Result.Success(new AuthData(accessToken, refreshToken));
    }

    public async Task<Result<AuthData>> Refresh(RefreshToken refreshToken)
    {
        var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
        if (session == null) 
        {
            return Result.Failure<AuthData>("Invalid refresh token");
        }
        var newRefreshToken = jwtGenerator.GenerateRefreshToken();
        session.RefreshToken = newRefreshToken;
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(session).ReloadAsync();
        
        var accessToken = jwtGenerator.GenerateAccessToken(session, options.Value);
        return Result.Success(new AuthData(accessToken, refreshToken));
    }
}

public record RegisterRequest(string Email, string Password, string Name, string PasswordRepeat);

public record LoginRequest(string Name, string Password);
public record AuthData(AccessToken AccessToken, RefreshToken RefreshToken);