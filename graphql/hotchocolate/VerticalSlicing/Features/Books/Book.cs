using SimpleApp.Features.Authors;

namespace SimpleApp.Features.Books;

public class Book(int id, string title, int authorId)
{
    public int Id { get; } = id;
    public string Title { get; } = title;
    
    public int AuthorId { get; } = authorId;
    
    public Author? Author { get; }
};