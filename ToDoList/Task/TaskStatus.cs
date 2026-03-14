using System.Text.Json.Serialization;

namespace ToDoList.Task;
[JsonConverter(typeof(JsonStringEnumConverter))]

public enum TaskStatus
{
     ToWork,
     InProgress,
     Done,
     Postponed
}