using Microsoft.EntityFrameworkCore;

using ToDoList.Infrastructure;

using AppUser = ToDoList.User.User;
using TodoTask = ToDoList.Task.Task;

namespace ToDoListTests;

// public class TaskServiceTests
// {
//     private static ToDoContext CreateContext()
//     {
//         var options = new DbContextOptionsBuilder<ToDoContext>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options;
//         return new ToDoContext(options);
//     }
//
//     private static async Task<AppUser> SeedUser(ToDoContext db)
//     {
//         var user = new AppUser
//         {
//             Name = "owner",
//             PassHash = "x",
//             Email = "o@o.o"
//         };
//         db.Users.Add(user);
//         await db.SaveChangesAsync();
//         return user;
//     }
//
//     [Fact]
//     public async Task Create_persists_task()
//     {
//         await using var db = CreateContext();
//         var user = await SeedUser(db);
//         var sut = new ToDoList.Task.TaskService(db);
//
//         var result = await sut.Create(
//             new ToDoList.Task.CreateTaskRequest
//             {
//                 Title = "t",
//                 Description = "d",
//                 DueDate = DateTime.UtcNow.AddDays(1),
//                 Priority = ToDoList.Task.TaskPriority.Medium
//             },
//             user);
//
//         Assert.True(result.IsSuccess);
//         Assert.Single(db.Tasks);
//         Assert.Equal("t", result.Value.Title);
//     }
//
//     [Fact]
//     public async Task GetTask_returns_failure_when_missing()
//     {
//         await using var db = CreateContext();
//         var user = await SeedUser(db);
//         var sut = new ToDoList.Task.TaskService(db);
//
//         var result = await sut.GetTask(ToDoList.Task.TaskId.Create(Guid.NewGuid()), user.UserId);
//
//         Assert.True(result.IsFailure);
//     }
//
//     [Fact]
//     public async Task SetStatus_updates_status()
//     {
//         await using var db = CreateContext();
//         var user = await SeedUser(db);
//         var task = new TodoTask
//         {
//             OwnerId = user.UserId,
//             Owner = user,
//             Title = "t",
//             Description = "d",
//             DueDate = DateTime.UtcNow.AddDays(1),
//             CreatedAt = DateTime.UtcNow,
//             Priority = ToDoList.Task.TaskPriority.Low,
//             Status = ToDoList.Task.TaskStatus.ToWork
//         };
//         db.Tasks.Add(task);
//         await db.SaveChangesAsync();
//
//         var sut = new ToDoList.Task.TaskService(db);
//
//         var result = await sut.SetStatus(task.TaskId, ToDoList.Task.TaskStatus.Done, user.UserId);
//
//         Assert.True(result.IsSuccess);
//         Assert.Equal(ToDoList.Task.TaskStatus.Done, result.Value.Status);
//     }
//
//     [Fact]
//     public async Task Delete_soft_deletes_task()
//     {
//         await using var db = CreateContext();
//         var user = await SeedUser(db);
//         var task = new TodoTask
//         {
//             OwnerId = user.UserId,
//             Owner = user,
//             Title = "t",
//             Description = "d",
//             DueDate = DateTime.UtcNow.AddDays(1),
//             CreatedAt = DateTime.UtcNow,
//             Priority = ToDoList.Task.TaskPriority.High,
//             Status = ToDoList.Task.TaskStatus.ToWork
//         };
//         db.Tasks.Add(task);
//         await db.SaveChangesAsync();
//
//         var sut = new ToDoList.Task.TaskService(db);
//
//         var result = await sut.Delete(task.TaskId, user.UserId);
//
//         Assert.True(result.IsSuccess);
//         var reloaded = await db.Tasks.IgnoreQueryFilters().SingleAsync(t => t.TaskId == task.TaskId);
//         Assert.NotNull(reloaded.DeletedAt);
//     }
//
//     [Fact]
//     public async Task Update_changes_fields()
//     {
//         await using var db = CreateContext();
//         var user = await SeedUser(db);
//         var task = new TodoTask
//         {
//             OwnerId = user.UserId,
//             Owner = user,
//             Title = "old",
//             Description = "oldd",
//             DueDate = DateTime.UtcNow.AddDays(2),
//             CreatedAt = DateTime.UtcNow,
//             Priority = ToDoList.Task.TaskPriority.Low,
//             Status = ToDoList.Task.TaskStatus.ToWork
//         };
//         db.Tasks.Add(task);
//         await db.SaveChangesAsync();
//
//         var sut = new ToDoList.Task.TaskService(db);
//         var newDue = DateTime.UtcNow.AddDays(10);
//
//         var result = await sut.Update(
//             task.TaskId,
//             new ToDoList.Task.UpdateTaskRequest
//             {
//                 Title = "new",
//                 Description = null,
//                 DueDate = newDue,
//                 Priority = ToDoList.Task.TaskPriority.High,
//                 Status = null
//             },
//             user.UserId);
//
//         Assert.True(result.IsSuccess);
//         Assert.Equal("new", result.Value.Title);
//         Assert.Equal("oldd", result.Value.Description);
//         Assert.Equal(newDue, result.Value.DueDate);
//         Assert.Equal(ToDoList.Task.TaskPriority.High, result.Value.Priority);
//     }
//
//     [Fact]
//     public async Task GetUserTasks_returns_only_owner_tasks()
//     {
//         await using var db = CreateContext();
//         var user = await SeedUser(db);
//         var other = new AppUser { Name = "o2", PassHash = "y", Email = "2@2.2" };
//         db.Users.Add(other);
//         await db.SaveChangesAsync();
//
//         db.Tasks.Add(new TodoTask
//         {
//             OwnerId = user.UserId,
//             Owner = user,
//             Title = "mine",
//             Description = "d",
//             DueDate = DateTime.UtcNow.AddDays(1),
//             CreatedAt = DateTime.UtcNow,
//             Priority = ToDoList.Task.TaskPriority.Medium,
//             Status = ToDoList.Task.TaskStatus.ToWork
//         });
//         db.Tasks.Add(new TodoTask
//         {
//             OwnerId = other.UserId,
//             Owner = other,
//             Title = "theirs",
//             Description = "d",
//             DueDate = DateTime.UtcNow.AddDays(1),
//             CreatedAt = DateTime.UtcNow,
//             Priority = ToDoList.Task.TaskPriority.Medium,
//             Status = ToDoList.Task.TaskStatus.ToWork
//         });
//         await db.SaveChangesAsync();
//
//         var sut = new ToDoList.Task.TaskService(db);
//
//         var result = await sut.GetUserTasks(user.UserId);
//
//         Assert.True(result.IsSuccess);
//         Assert.Single(result.Value);
//         Assert.Equal("mine", result.Value[0].Title);
//     }
// }