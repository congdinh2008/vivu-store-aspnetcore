namespace ViVuStore.Data.Repositories;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T, TContext> : IGenericRepository<T> where T : class where TContext : ViVuStoreDbContext
{
    protected readonly TContext DataContext;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(TContext dataContext)
    {
        this.DataContext = dataContext;

        // Find Property with typeof(T) on dataContext
        var typeOfDbSet = typeof(DbSet<T>);
        foreach (var prop in dataContext.GetType().GetProperties())
        {
            if (typeOfDbSet == prop.PropertyType)
            {
                if (prop.GetValue(dataContext, null) is DbSet<T> dbSet)
                {
                    DbSet = dbSet;
                }
                break;
            }
        }
        DbSet ??= dataContext.Set<T>();
    }

    #region Virtual Methods

    public virtual void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public virtual void Add(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Add(entity);
        }
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(T entity, bool isHardDelete = false)
    {
        DbSet.Remove(entity);
    }

    public virtual void Delete(IEnumerable<T> entities, bool isHardDelete = false)
    {
        foreach (var entity in entities)
        {
            Delete(entity, isHardDelete);
        }
    }

    public virtual void Delete(Expression<Func<T, bool>> where, bool isHardDelete = false)
    {
        var entities = GetQuery(where).AsEnumerable();
        foreach (var entity in entities)
        {
            Delete(entity, isHardDelete);
        }
    }

    public virtual T? GetById(Guid id)
    {
        return DbSet.Find(id);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual IQueryable<T> GetQuery()
    {
        return DbSet.AsQueryable();
    }

    public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> where)
    {
        return GetQuery().Where(where);
    }

    #endregion
}