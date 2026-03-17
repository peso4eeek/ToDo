using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using ToDoList.Infrastructure;
using ToDoList.User;

namespace ToDoList.Task;

public class TaskService(ToDoContext dbContext)
{
    public async Task<Result<Task>> Create(CreateTaskRequest request, User.User user)
    {
        var task = new Task
        {
            OwnerId = user.UserId,
            Owner = user,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow,
            Priority = request.Priority,
            Status = TaskStatus.ToWork,
        };
        
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(task).ReloadAsync();
        return Result.Success(task);
    }

    public async Task<Result<Task>> SetStatus(TaskId taskId, TaskStatus status, UserId userId)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.OwnerId == userId);
        if (task == null) return Result.Failure<Task>("Task not found");
        task.Status = status;
        await dbContext.SaveChangesAsync();
        return Result.Success(task);
    }

    public async Task<Result<List<Task>>> GetUserTasks(UserId userId)
    {
        return Result.Success(await dbContext.Tasks.Where(t => t.OwnerId == userId).ToListAsync());
    }

    public async Task<Result<Task>> GetTask(TaskId taskId,  UserId userId)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId &&  t.OwnerId == userId);
        return task == null ? Result.Failure<Task>("Task not found") : Result.Success(task);
    }

    public async Task<Result> Delete(TaskId taskId, UserId userId)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId &&  t.OwnerId == userId);
        if (task == null) return Result.Failure("Task not found");
        task.DeletedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<Task>> Update(TaskId taskId, UpdateTaskRequest request, UserId userId)
    {
        var task =  await dbContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId &&  t.OwnerId == userId);
        if (task == null) return Result.Failure<Task>("Task not found");
        task.Title = request.Title ?? task.Title;
        task.Description = request.Description ?? task.Description;
        task.DueDate = request.DueDate ?? task.DueDate;
        task.Priority = request.Priority ?? task.Priority;
        task.Status = request.Status ?? task.Status;
        await dbContext.SaveChangesAsync();
        return Result.Success(task);
    }
}

public record CreateTaskRequest
{
    public required string Title { get; init; }
    
    public required string Description  { get; init; }
    
    public required DateTime DueDate { get; init; }
    
    public required TaskPriority Priority { get; init; }
}

public record UpdateTaskRequest
{ 
    public string? Title { get; init; }
    
    public string? Description  { get; init; }
    
    public DateTime? DueDate { get; init; }
    
    public TaskPriority? Priority { get; init; }
    
    public TaskStatus? Status { get; init; }
}