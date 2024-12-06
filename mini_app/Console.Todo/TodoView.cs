using Spectre.Console;

namespace Console.Todo;

internal class TodoView
{
    public static void DisplayTodos(IEnumerable<TodoItem> todos)
    {
        var groupedTodos = todos
            .GroupBy(todo => todo.ListName)
            .ToDictionary(group => group.Key, group => group.ToList());

        var table = new Table().Centered();
        table.AddColumn("List Name");
        table.AddColumn("TODO Items");

        foreach (var group in groupedTodos)
        {
            var todosText = group.Value.Count == 0
                ? "(No TODOs)"
                : string.Join("\n", group.Value.Select(todo => $"{todo.Text} [dim]({(todo.IsCompleted ? "Completed" : "Pending")})[/] [blue]({todo.CreatedAt:G})[/]"));

            table.AddRow($"[bold]{group.Key}[/]", todosText);
        }

        AnsiConsole.Write(table);
    }

    public static void DisplayMessage(string message, string color = "green")
    {
        AnsiConsole.MarkupLine($"[{color}]{message}[/]");
    }
}