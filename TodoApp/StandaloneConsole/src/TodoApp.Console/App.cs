namespace TodoApp.Console;

public class App
{
    public async Task RunAsync(string[] args)
    {
        System.Console.WriteLine("Hello world");

        await Task.CompletedTask;
    }
}
