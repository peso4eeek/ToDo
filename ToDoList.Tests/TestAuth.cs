namespace ToDoListTests;

internal static class TestAuth
{
    /// <summary>
    /// Должен совпадать с <c>testingJwtKeyFallback</c> в Program.cs для окружения Testing.
    /// </summary>
    private const string TestingJwtKeyFallback = "known-fallback-jwt-key-for-tests-32b!!";

    internal static string SigningKey
    {
        get
        {
            var fromEnv = Environment.GetEnvironmentVariable("JWT_TEST_SECRET_KEY")?.Trim();
            return !string.IsNullOrEmpty(fromEnv) ? fromEnv : TestingJwtKeyFallback;
        }
    }
}