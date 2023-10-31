namespace TodoClient;

public class GetItemsByListId : Specification<TodoItem>
{
    public GetItemsByListId(Guid listId) : base(item => item.ListId == listId)
    {
        AddOrderBy(s => s.DueTo);
    }
}
