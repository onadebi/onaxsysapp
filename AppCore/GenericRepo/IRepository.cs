using System.Linq.Expressions;

namespace AppCore.GenericRepo
{
    public interface IRepository<T> where T : class
    {
        #region Sync methods
        T? GetById(object id);
        IEnumerable<T> GetAllPaged(Pagination PageData = default!);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate, Pagination PageData = default!);
        T? FirstOrDefault(Expression<Func<T, bool>> predicate);

        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        #endregion

        #region Async methods
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllPagedAsync(Pagination PageData = default!);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Pagination PageData = default!);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        #endregion
    }
}
