using System.Security.Claims;

using ToDoList.User;

namespace ToDoListTests;

public class ClaimsExtensionsTests
{
    [Fact]
    public void GetUserId_returns_user_id_when_claim_present()
    {
        var id = Guid.NewGuid();
        var principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("UserId", id.ToString())]));

        var userId = principal.GetUserId();

        Assert.Equal(UserId.Create(id), userId);
    }

    [Fact]
    public void GetUserId_throws_when_claim_missing()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        Assert.Throws<InvalidOperationException>(() => principal.GetUserId());
    }
}