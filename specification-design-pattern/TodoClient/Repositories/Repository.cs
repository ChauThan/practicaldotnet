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

    public IEnumerable<TEntity> Apply(Specification<TEntity> specification)
    {
        var queryable = _dbContext.Set<TEntity>().AsQueryable();

        if (specification.Criteria is not null)
        {
            queryable = queryable.Where(specification.Criteria);
        }

        foreach (var includeExpression in specification.IncludeExpressions)
        {
            queryable = queryable.Include(includeExpression);
        }

        if (specification.OrderByExpression is not null)
        {
            queryable = queryable.OrderBy(specification.OrderByExpression);
        }

        if (specification.OrderByDescendingExpression is not null)
        {
            queryable = queryable.OrderByDescending(specification.OrderByDescendingExpression);
        }

        return queryable.ToList();
    }
}
