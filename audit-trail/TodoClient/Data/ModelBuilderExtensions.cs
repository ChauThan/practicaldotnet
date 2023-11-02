using Microsoft.EntityFrameworkCore;

namespace TodoClient;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        var listOne = Guid.NewGuid();
        var listTwo = Guid.NewGuid();
        modelBuilder.Entity<TodoList>().HasData(
            new TodoList
            {
                Id = listOne,
                Title = "Getting started"
            },
            new TodoList
            {
                Id = listTwo,
                Title = "Another list"
            }
        );

        var now = DateTimeOffset.Now;
        var items = new List<TodoItem>();
        for (int i = 10; i > 0; i--)
        {
            items.Add(new TodoItem
            {
                ListId = i % 2 == 0 ? listOne : listTwo,
                Title = $"item {i}",
                Content = $"item {i}",
                CreatedAt = now.AddDays(-i),
                DueTo = now.AddDays(-i),
                Done = i % 3 == 0
            });
        }

        modelBuilder.Entity<TodoItem>().HasData(items);
    }
}
