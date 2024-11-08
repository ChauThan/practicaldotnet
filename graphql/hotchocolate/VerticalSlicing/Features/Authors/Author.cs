using System.Runtime.InteropServices;
using SimpleApp.Features.Books;

namespace SimpleApp.Features.Authors;

public class Author(int id, string name)
{
    public int Id { get; } = id;

    public string Name { get; } = name;

    public ICollection<Book> Books { get;  } = new List<Book>();
};