using AppCore.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCore.GenericRepo;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        this._context = context;
        this._dbSet = _context.Set<T>();
    }
    #region Synchronous Methods
    public virtual T? GetById(object id)
    {
        return _dbSet.Find(id);
    }

    public virtual IEnumerable<T> GetAllPaged(Pagination PageData = default!)
    {
        return [.. _dbSet];
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate, Pagination PageData = default!)
    {
        return [.. _dbSet.Where(predicate).Skip((PageData.PageNumber == 1 ? 0: PageData.PageNumber) * PageData.PageCount).Take(PageData.PageCount
            )];
    }

    public virtual T? FirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.FirstOrDefault(predicate);
    }

    public virtual void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
    #endregion

    #region Asynchronous Methods
    public virtual async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllPagedAsync(Pagination PageData = default!)
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Pagination PageData = default!)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }
    #endregion
}
