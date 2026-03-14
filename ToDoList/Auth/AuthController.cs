using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Auth;

[ApiController]

[Route("api/auth")]
public class AuthController(AuthService authService): ControllerBase
{
    [HttpPost("login")]
    public async Task<Results<Ok<AuthData>, BadRequest<string>>> Login([FromBody] LoginRequest request)
    {
        var authData = await authService.Login(request);
        
        return authData.IsSuccess ? TypedResults.Ok(authData.Value) : TypedResults.BadRequest(authData.Error);
    }

    [HttpPost("register")]
    public async Task<Results<Ok, BadRequest>> Register([FromBody] RegisterRequest request)
    {
        var res = await authService.Register(request);

        return res.IsSuccess ? TypedResults.Ok() : TypedResults.BadRequest();
    }

    [HttpPost("refresh")]
    public async Task<Results<Ok<AuthData>, BadRequest<string>>> Refresh([FromBody] RefreshToken token)
    {
        var res = await authService.Refresh(token);
        return res.IsSuccess ? TypedResults.Ok(res.Value) : TypedResults.BadRequest(res.Error);
    }
 }