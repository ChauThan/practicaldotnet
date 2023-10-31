namespace TodoClient;

public class GetCompletedItems : Specification<TodoItem>
{
    public GetCompletedItems() : base(
        i => i.Done
    )
    {
        AddOrderByDescending(s => s.DueTo);
    }
}
