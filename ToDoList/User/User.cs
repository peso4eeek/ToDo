using Thinktecture;

namespace ToDoList.User;

[ValueObject<Guid>]
public readonly partial struct UserId;
public class User
{
    public UserId UserId { get; init; }
    
    public required string Name { get; set; }
    
    public required string PassHash { get; set; }
    
    public string Email { get; set; } = string.Empty;
}