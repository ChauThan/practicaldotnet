using System.Text.Json;
using TodoApp.Console.Data;

public class DataContextTests(DataContextFixture fixture) : IClassFixture<DataContextFixture>
{
    private readonly DataContextFixture _fixture = fixture;

    [Fact]
    public void Constructor_ShouldLoadData_WhenFileExists()
    {
        // Act
        var dataContext = new DataContext();

        // Assert
        Assert.NotNull(dataContext.TodoItems);
        Assert.NotNull(dataContext.TodoLists);
    }

    [Fact]
    public void Constructor_ShouldCreateNewData_WhenFileDoesNotExist()
    {
        // Arrange
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Act
        var dataContext = new DataContext();

        // Assert
        Assert.NotNull(dataContext.TodoItems);
        Assert.NotNull(dataContext.TodoLists);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSaveData_ToFile()
    {
        // Arrange
        var dataContext = new DataContext();
        var expectedData = new DataContext.Data([], []);
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");

        // Act
        await dataContext.SaveChangesAsync();
        var savedData = JsonSerializer.Deserialize<DataContext.Data>(File.ReadAllText(filePath));

        // Assert
        Assert.Equal(expectedData.TodoItems, savedData?.TodoItems);
        Assert.Equal(expectedData.TodoLists, savedData?.TodoLists);
    }
}
