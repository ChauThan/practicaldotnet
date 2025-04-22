using Microsoft.EntityFrameworkCore;
using SimpleBlog.Domain.Blogs;

namespace SimpleBlog.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}