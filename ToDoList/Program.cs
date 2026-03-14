using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Thinktecture.AspNetCore.ModelBinding;
using Thinktecture.Text.Json.Serialization;
using ToDoList;
using ToDoList.Auth;
using ToDoList.Task;
using ToDoList.User;
using TaskStatus = ToDoList.Task.TaskStatus;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(LogEventLevel.Information,
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{Client}] [{Device}] {Message:lj}{NewLine}{Exception}",
        theme: AnsiConsoleTheme.Code)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var dataSourceBuilder = new  NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
dataSourceBuilder.MapEnum<TaskStatus>("task_status"); 
dataSourceBuilder.MapEnum<TaskPriority>("task_priority");
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseNpgsql(dataSource));

var authSection = builder.Configuration.GetSection("Auth");

builder.Services.Configure<AuthOptions>(authSection);
var authOptions = authSection.Get<AuthOptions>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = authOptions?.GetSymmetricSecurityKey(),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
builder.Services.AddAuthorization(); 
builder.Services.AddCors();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});

builder.Host.UseSerilog();


builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddControllers(options =>
    {
        options.ModelBinderProviders
            .Insert(0, new ThinktectureModelBinderProvider());
    })
    .AddJsonOptions(j =>
    {
        j.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        j.JsonSerializerOptions
            .Converters
            .Add(new ThinktectureJsonConverterFactory());
    });

builder.Services.AddSingleton<JWTGenerator>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddTransient<JwtSecurityTokenHandler>();
builder.Services.AddTransient<TaskService>();
builder.Services.AddTransient<UserService>();

var app = builder.Build();
app.MapScalarApiReference("/scalar", options =>
{
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpLogging();
app.MapControllers();
app.Run();
