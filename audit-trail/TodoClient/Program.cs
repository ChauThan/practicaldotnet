using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Json;
using TodoClient;

using var context = new AppDbContext();
context.Database.EnsureCreated();

var audits = context.Audits.ToList();
Dump("Audit", audits);

var lists = context.TodoLists.ToList();
foreach (var list in lists)
{
    list.Title = $"{list.Title} + Updated";
    context.SaveChanges();
}

audits = context.Audits.ToList();
Dump("Audit", audits);

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