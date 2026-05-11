namespace ToDoListTests;

[CollectionDefinition("Api", DisableParallelization = true)]
public class ApiCollection : ICollectionFixture<ToDoListWebApplicationFactory>
{
}