namespace ToDoListTests;

internal static class TestAuth
{
    internal static string SigningKey => Environment.GetEnvironmentVariable("JWT_TEST_SECRET_KEY")
        ?? "default-signing-key-if-missing";
}