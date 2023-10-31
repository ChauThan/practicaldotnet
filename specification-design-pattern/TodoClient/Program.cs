using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Json;
using TodoClient;

using var context = new AppDbContext();
context.Database.EnsureCreated();

var lists = context.TodoLists.ToList();
Dump("Todo Lists", lists);

var items = context.TodoItems.ToList();
Dump("Todo Item", items);


var itemRepository = new Repository<TodoItem>(context);

var firstList = lists.First();
var firstListItems = itemRepository.Apply(new GetItemsByListId(firstList.Id));
Dump($"List - \"{firstList.Title}\"", firstListItems);

var completedItems = itemRepository.Apply(new GetCompletedItems());
Dump($"Completed", completedItems);

static void Dump(string header, object jsonObj)
{
    var jsonString = JsonSerializer.Serialize(jsonObj);
    AnsiConsole.Write(
        new Panel(new JsonText(jsonString))
            .Header(header)
            .Collapse()
            .RoundedBorder()
            .BorderColor(Color.Yellow));
}