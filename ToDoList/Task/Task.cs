using System.Text.Json.Serialization;
using Thinktecture;
using ToDoList.User;

namespace ToDoList.Task;
[ValueObject<Guid>]
public readonly partial struct TaskId;
public class Task
{
    public TaskId TaskId { get; init; }
    
    public required UserId OwnerId { get; init; }
    
    [JsonIgnore] public User.User Owner { get; init; }
    public required string Title { get; set; }
    
    public required string Description { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public TaskPriority Priority { get; set; }
    
    public TaskStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? DeletedAt { get; set; }
}


