using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using ToDoList.Auth;

namespace ToDoListTests;

[Collection("Api")]
public class TaskApiTests
{
    private readonly ToDoListWebApplicationFactory _factory;

    public TaskApiTests(ToDoListWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_and_list_tasks_happy_path()
    {
        var client = _factory.CreateClient();
        var name = $"taskuser_{Guid.NewGuid():N}";
        await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("e@e.e", "pwd", name, "pwd"));
        var login = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(name, "pwd"));
        login.EnsureSuccessStatusCode();
        var auth = await login.Content.ReadFromJsonAsync<AuthDataJson>();
        Assert.NotNull(auth);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var due = DateTime.UtcNow.AddDays(3);
        var create = await client.PostAsJsonAsync(
            "/api/task",
            new ToDoList.Task.CreateTaskRequest
            {
                Title = "title",
                Description = "desc",
                DueDate = due,
                Priority = ToDoList.Task.TaskPriority.High
            });

        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        using var createDoc = await JsonDocument.ParseAsync(await create.Content.ReadAsStreamAsync());
        var taskId = createDoc.RootElement.GetProperty("taskId").GetString();
        Assert.False(string.IsNullOrEmpty(taskId));

        var list = await client.GetAsync("/api/task/all-user");
        list.EnsureSuccessStatusCode();
        using var listDoc = await JsonDocument.ParseAsync(await list.Content.ReadAsStreamAsync());
        Assert.Equal(JsonValueKind.Array, listDoc.RootElement.ValueKind);
        Assert.Equal(1, listDoc.RootElement.GetArrayLength());

        var one = await client.GetAsync($"/api/task/{taskId}");
        one.EnsureSuccessStatusCode();

        var patch = await client.PatchAsJsonAsync($"/api/task/{taskId}/status", ToDoList.Task.TaskStatus.Done);
        patch.EnsureSuccessStatusCode();

        var put = await client.PutAsJsonAsync(
            $"/api/task/{taskId}",
            new ToDoList.Task.UpdateTaskRequest { Title = "updated" });
        put.EnsureSuccessStatusCode();

        var del = await client.DeleteAsync($"/api/task/{taskId}");
        del.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Task_endpoints_return_401_without_token()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/task/all-user");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Task_mutations_return_400_when_task_missing()
    {
        var client = _factory.CreateClient();
        var name = $"nf_{Guid.NewGuid():N}";
        await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("nf@nf.nf", "pwd", name, "pwd"));
        var login = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(name, "pwd"));
        login.EnsureSuccessStatusCode();
        var auth = await login.Content.ReadFromJsonAsync<AuthDataJson>();
        Assert.NotNull(auth);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var missingId = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync($"/api/task/{missingId}")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.DeleteAsync($"/api/task/{missingId}")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.PutAsJsonAsync($"/api/task/{missingId}", new ToDoList.Task.UpdateTaskRequest { Title = "x" }))
            .StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.PatchAsJsonAsync($"/api/task/{missingId}/status", ToDoList.Task.TaskStatus.Done))
            .StatusCode);
    }

    private sealed class AuthDataJson
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}