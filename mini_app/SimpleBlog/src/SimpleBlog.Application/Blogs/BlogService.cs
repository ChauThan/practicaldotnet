using SimpleBlog.Domain.Blogs;

namespace SimpleBlog.Application.Blogs;

internal class BlogService(IBlogRepository blogRepository) : IBlogService
{
    public async Task<Blog> GetByIdAsync(Guid id)
    {
        return await blogRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Blog>> GetAllAsync()
    {
        return await blogRepository.GetAllAsync();
    }

    public async Task AddAsync(Blog blog)
    {
        await blogRepository.AddAsync(blog);
    }

    public async Task UpdateAsync(Blog blog)
    {
        await blogRepository.UpdateAsync(blog);
    }

    public async Task DeleteAsync(Guid id)
    {
        await blogRepository.DeleteAsync(id);
    }
}