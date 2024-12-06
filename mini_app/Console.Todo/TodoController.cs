using Spectre.Console;

namespace Console.Todo;

internal class TodoController(TodoService service)
{
    public void AddTodo()
    {
        var todoText = AnsiConsole.Ask<string>("Enter the TODO:");
        var listName = AnsiConsole.Ask<string>("Enter the list name (or press Enter for 'Uncategorized'):", "Uncategorized");
        service.AddTodo(todoText, listName);
        TodoView.DisplayMessage("TODO added successfully.");
    }

    public void ViewAllTodos()
    {
        var todos = service.GetTodos();
        TodoView.DisplayTodos(todos);
    }

    public void ViewTodosByList()
    {
        var listPattern = AnsiConsole.Ask<string>("Enter the list name (supports wildcard '*'):");
        var todos = service.GetTodosByList(listPattern);

        if (!todos.Any())
        {
            TodoView.DisplayMessage("No TODOs found matching this pattern.", "red");
            return;
        }

        TodoView.DisplayTodos(todos);
    }

    public void EditTodo()
    {
        var todos = service.GetTodos().ToList();
        if (!todos.Any())
        {
            TodoView.DisplayMessage("No TODOs available to edit.", "red");
            return;
        }

        var todoSelection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a TODO to edit:")
                .AddChoices(todos.Select(todo => todo.Text)));

        var selectedTodo = todos.First(todo => todo.Text == todoSelection);

        var newTitle = AnsiConsole.Ask("Enter the new title (or press Enter to keep current title):", selectedTodo.Text);
        var isCompleted = AnsiConsole.Confirm("Is this TODO completed?", selectedTodo.IsCompleted);

        selectedTodo.Text = newTitle;
        selectedTodo.IsCompleted = isCompleted;

        service.UpdateTodo(selectedTodo);

        TodoView.DisplayMessage("TODO updated successfully.");
    }
}