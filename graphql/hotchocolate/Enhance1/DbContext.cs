namespace Enhance1;

public class DbContext
{
    private readonly IList<Models.Author> _authors =
    [
        new(1, "Mark Seemann"),
        new(2, "Martin Kleppmann")
    ];

    private readonly IList<Models.Book> _books =
    [
        new(1, "Code That Fits in Your Head", 1),
        new(2, "Dependency Injection Principles, Practices, and Patterns", 1),
        new(3, "Designing Data-Intensive Applications", 2)
    ];
    
    public ICollection<Models.Author> GetAuthors()  => _authors;
    public Models.Author? GetAuthorById(int id)  => _authors.FirstOrDefault(a => a.Id == id);
    public void AddAuthor(Models.Author author) => _authors.Add(author);
    
    public ICollection<Models.Book> GetBooks() => _books;
    public Models.Book? GetBookById(int id)  => _books.FirstOrDefault(b => b.Id == id);
    public ICollection<Models.Book> GetBooksByAuthorId(int id)  => _books.Where(b => b.AuthorId == id).ToList();
    public void AddBook(Models.Book book) => _books.Add(book);
}

public static class Models
{
    public record Book(int Id, string Title, int AuthorId);
    public record Author(int Id, string Name);
}