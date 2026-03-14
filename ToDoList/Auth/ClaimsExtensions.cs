using System.Security.Claims;
using CSharpFunctionalExtensions;

namespace ToDoList.User;

public static class ClaimsExtensions
{
    public static UserId GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("UserId");

        if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
        {
            throw new InvalidOperationException("UserId not found");
        }
        var userIdGuid = Guid.Parse(userIdClaim.Value);
        return UserId.Create(userIdGuid);
    }
}