using Microsoft.EntityFrameworkCore;

using ToDoList.Infrastructure;

using AppUser = ToDoList.User.User;

namespace ToDoListTests;

public class UserServiceTests
{
    [Fact]
    public async Task GetById_returns_failure_when_user_missing()
    {
        var options = new DbContextOptionsBuilder<ToDoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new ToDoContext(options);
        var sut = new ToDoList.User.UserService(db);

        var result = await sut.GetById(ToDoList.User.UserId.Create(Guid.NewGuid()));

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task GetById_returns_user_when_present()
    {
        var options = new DbContextOptionsBuilder<ToDoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new ToDoContext(options);
        var user = new AppUser { Name = "n", PassHash = "h", Email = "e@e.e" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var sut = new ToDoList.User.UserService(db);

        var result = await sut.GetById(user.UserId);

        Assert.True(result.IsSuccess);
        Assert.Equal("n", result.Value.Name);
    }
}