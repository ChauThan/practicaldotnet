using System.Text.Json;
using TodoApp.Console.Data;
using TodoApp.Console.Models;

public class DataContextFixture : IDisposable
{
    public DataContextFixture()
    {
        SetupFile();
    }

    private void SetupFile()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        File.WriteAllText(filePath, JsonSerializer.Serialize(new DataContext.Data([], [])));
    }

    public void Dispose()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
