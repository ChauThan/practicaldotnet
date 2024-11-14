namespace EFCore.Models;

public class Post
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ImageUrl { get; set; }
}