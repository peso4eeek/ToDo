using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ToDoList.Infrastructure;

namespace ToDoListTests;

public class ToDoListWebApplicationFactory : WebApplicationFactory<Program>
{
    public ToDoListWebApplicationFactory()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToDoContext>();
        db.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Testing:InMemoryDbName"] = Guid.NewGuid().ToString(),
                    ["Auth:Key"] = TestAuth.SigningKey
                });
        });
    }
}