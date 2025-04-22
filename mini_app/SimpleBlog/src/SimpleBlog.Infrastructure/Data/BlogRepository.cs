using Microsoft.EntityFrameworkCore;
using SimpleBlog.Domain.Blogs;

namespace SimpleBlog.Infrastructure.Data;

public class BlogRepository(AppDbContext context) : IBlogRepository
{
    public async Task<Blog> GetByIdAsync(Guid id)
    {
        var blog = await context.Blogs.FindAsync(id)
            ?? throw new KeyNotFoundException("Blog not found");
        return blog;
    }

    public async Task<IEnumerable<Blog>> GetAllAsync()
    {
        return await context.Blogs.ToListAsync();
    }

    public async Task AddAsync(Blog blog)
    {
        await context.Blogs.AddAsync(blog);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Blog blog)
    {
        context.Blogs.Update(blog);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        context.Blogs.Remove(await GetByIdAsync(id));
        await context.SaveChangesAsync();
    }
}