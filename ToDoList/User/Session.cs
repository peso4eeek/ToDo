using System.Text.Json.Serialization;
using Thinktecture;

namespace ToDoList.User;

[ValueObject<Guid>]
public readonly partial struct SessionId;
public class Session
{
    public SessionId SessionId { get; init; }
    public required UserId UserId { get; init; }
    
    [JsonIgnore] public User User { get; init; }
    public required Auth.RefreshToken RefreshToken { get; set; }
    
    public bool IsActive { get; set; }
}