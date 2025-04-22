namespace SimpleBlog.Domain.Blogs;

public interface IBlogRepository
{
    Task<Blog> GetByIdAsync(Guid id);
    Task<IEnumerable<Blog>> GetAllAsync();
    Task AddAsync(Blog blog);
    Task UpdateAsync(Blog blog);
    Task DeleteAsync(Guid id);
}