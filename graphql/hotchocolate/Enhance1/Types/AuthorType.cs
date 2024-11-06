namespace Enhance1.Types;

public class AuthorType : ObjectType<Author>
{
}

public record Author(int Id, string Name);    

