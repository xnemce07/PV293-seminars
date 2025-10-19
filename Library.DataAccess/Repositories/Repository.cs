using System.Linq.Expressions;
using Library.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> Entities;

    public Repository(DbContext context)
    {
        Context = context;
        Entities = Context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await Entities.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Entities.ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Entities.Where(predicate).ToListAsync();
    }

    public virtual async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Entities.SingleOrDefaultAsync(predicate);
    }

    public virtual void Add(TEntity entity)
    {
        Entities.Add(entity);
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        Entities.AddRange(entities);
    }

    public virtual void Remove(TEntity entity)
    {
        Entities.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        Entities.RemoveRange(entities);
    }

    public virtual void Update(TEntity entity)
    {
        Entities.Update(entity);
    }
}