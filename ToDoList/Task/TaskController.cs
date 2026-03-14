using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ToDoList.User;

namespace ToDoList.Task;


[ApiController]
[Authorize]
[Route("api/task")]
public class TaskController(TaskService taskService, UserService userService): ControllerBase
{
    [HttpPost]
    public async Task<Results<Ok<Task>, BadRequest<string>>> Create([FromBody] CreateTaskRequest request)
    {
        var userId = User.GetUserId();
        var user = await userService.GetById(userId);
        if (user.IsFailure) return TypedResults.BadRequest(user.Error);
        var res = await taskService.Create(request, user.Value);
        return res.IsFailure ? TypedResults.BadRequest("Can not create task") : TypedResults.Ok(res.Value);
    }
    
    [HttpDelete("{id}")]
    public async Task<Results<Ok, BadRequest<string>>> Delete([FromRoute] TaskId id)
    {
        var userId = User.GetUserId();
        var res = await taskService.Delete(id, userId);
        return res.IsFailure ? TypedResults.BadRequest(res.Error) : TypedResults.Ok();
    }
    
    [HttpGet("all-user")]
    public async Task<Results<Ok<List<Task>>, BadRequest<string>>> GetAllUserTask()
    {
        var userId = User.GetUserId();
        var result = await taskService.GetUserTasks(userId);
        return result.IsFailure ? TypedResults.BadRequest(result.Error) : TypedResults.Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<Results<Ok<Task>, BadRequest<string>>> GetTask([FromRoute] TaskId id)
    {
        var userId = User.GetUserId();
        var res = await taskService.GetTask(id, userId);
        return res.IsFailure ? TypedResults.BadRequest(res.Error) : TypedResults.Ok(res.Value);
    }

    [HttpPut("{id}")]
    public async Task<Results<Ok<Task>, BadRequest<string>>> UpdateTask([FromRoute] TaskId id,
        [FromBody] UpdateTaskRequest request)
    {
        var userId = User.GetUserId();
        var res = await taskService.Update(id, request, userId);
        return res.IsFailure ? TypedResults.BadRequest(res.Error) : TypedResults.Ok(res.Value);
    }

    [HttpPatch("{id}/status")]
    public async Task<Results<Ok<Task>, BadRequest<string>>> UpdateStatus([FromRoute] TaskId id,
        [FromBody] TaskStatus status)
    {
        var userId = User.GetUserId();
        var res = await taskService.SetStatus(id, status, userId);
        return res.IsFailure ? TypedResults.BadRequest(res.Error) : TypedResults.Ok(res.Value);
    }
}