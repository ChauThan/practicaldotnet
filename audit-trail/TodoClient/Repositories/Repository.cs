using Microsoft.EntityFrameworkCore;

namespace TodoClient;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly AppDbContext _dbContext;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);
        _dbContext.SaveChanges();
    }

    public void Update(TEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        _dbContext.SaveChanges();
    }

    public void Delete(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
        _dbContext.SaveChanges();
    }

    public TEntity? GetById(Guid id)
    {
        return _dbContext.Set<TEntity>().FirstOrDefault(e => e.Id == id);
    }
}
