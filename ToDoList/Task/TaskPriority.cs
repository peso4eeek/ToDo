using System.Text.Json.Serialization;

namespace ToDoList.Task;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskPriority
{
    High,
    Medium,
    Low
}