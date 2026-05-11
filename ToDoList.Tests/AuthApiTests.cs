using System.Net;
using System.Net.Http.Json;

using ToDoList.Auth;

namespace ToDoListTests;

[Collection("Api")]
public class AuthApiTests
{
    private readonly HttpClient _client;

    public AuthApiTests(ToDoListWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_returns_400_when_passwords_differ()
    {
        var name = $"u_{Guid.NewGuid():N}";
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("a@a.a", "one", name, "two"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_returns_ok_when_valid()
    {
        var name = $"u_{Guid.NewGuid():N}";
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("b@b.b", "pwd", name, "pwd"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_returns_400_when_credentials_invalid()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("no_such_user", "pwd"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_returns_ok_after_register()
    {
        var name = $"u_{Guid.NewGuid():N}";
        await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("c@c.c", "pwd", name, "pwd"));

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(name, "pwd"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthDataJson>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.AccessToken));
        Assert.False(string.IsNullOrEmpty(body.RefreshToken));
    }

    [Fact]
    public async Task Refresh_returns_400_for_invalid_token()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", "not-a-valid-refresh-body");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_returns_ok_after_login()
    {
        var name = $"u_{Guid.NewGuid():N}";
        await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("r@r.r", "pwd", name, "pwd"));
        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(name, "pwd"));
        login.EnsureSuccessStatusCode();
        var auth = await login.Content.ReadFromJsonAsync<AuthDataJson>();
        Assert.NotNull(auth);

        var refresh = await _client.PostAsJsonAsync(
            "/api/auth/refresh",
            RefreshToken.Create(auth.RefreshToken));

        refresh.EnsureSuccessStatusCode();
        var next = await refresh.Content.ReadFromJsonAsync<AuthDataJson>();
        Assert.NotNull(next);
        Assert.NotEqual(auth.RefreshToken, next.RefreshToken);
    }

    private sealed class AuthDataJson
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}