using Console.Todo;
using Spectre.Console;

var service = new TodoService("todo.db");
var controller = new TodoController(service);

while (true)
{
    AnsiConsole.Clear();
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold yellow]TODO APP[/] - Select an option:")
            .AddChoices("Add a TODO", "View all TODOs", "View TODOs by List", "Edit TODO", "Exit"));

    switch (choice)
    {
        case "Add a TODO":
            controller.AddTodo();
            break;
        case "View all TODOs":
            controller.ViewAllTodos();
            break;
        case "View TODOs by List":
            controller.ViewTodosByList();
            break;
        case "Edit TODO":
            controller.EditTodo();
            break;
        case "Exit":
            return;
    }

    AnsiConsole.MarkupLine("[grey](Press any key to return to menu)[/]");
    System.Console.ReadKey();
}