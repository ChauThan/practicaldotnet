using SimpleBlog.Domain.Blogs;

namespace SimpleBlog.Application.Blogs;

public interface IBlogService
{
    Task<Blog> GetByIdAsync(Guid id);
    Task<IEnumerable<Blog>> GetAllAsync();
    Task AddAsync(Blog blog);
    Task UpdateAsync(Blog blog);
    Task DeleteAsync(Guid id);
}