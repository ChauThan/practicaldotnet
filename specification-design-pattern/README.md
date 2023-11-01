# Specification design pattern
In computer programming, the specification pattern is a particular software design pattern, whereby business rules can be recombined by chaining the business rules together using boolean logic. The pattern is frequently used in the context of domain-driven design.[Wiki](https://en.wikipedia.org/wiki/Specification_pattern#:~:text=In%20computer%20programming%2C%20the%20specification,context%20of%20domain%2Ddriven%20design.)

## How to use
Create a new specification class for a given entity
```
public class GetItemsByListId : Specification<TodoItem>
{
    public GetItemsByListId(Guid listId) : base(item => item.ListId == listId)
    {
        AddOrderBy(s => s.DueTo);
    }
}
```

Then use it
```
using var context = new AppDbContext();
var itemRepository = new Repository<TodoItem>(context);

var firstListItems = itemRepository.Apply(new GetItemsByListId("<input-id-here>"));
```