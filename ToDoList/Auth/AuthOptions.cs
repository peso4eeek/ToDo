using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ToDoList.Auth;

public class AuthOptions
{
    public string Key { get; set; } = null!;

    public int AccessTokenLifetimeMinutes { get; set; } = 7;
    
    public int RefreshTokenLifetimeMinutes { get; set; } = 60;

    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}