using CSharpFunctionalExtensions;
using ToDoList.Infrastructure;

namespace ToDoList.User;

public class UserService(ToDoContext dbContext)
{
    public async Task<Result<User>> GetById(UserId userId)
    {
        var user = await dbContext.Users.FindAsync(userId);
        return user == null ? Result.Failure<User>("User not found") : Result.Success(user);
    }
}