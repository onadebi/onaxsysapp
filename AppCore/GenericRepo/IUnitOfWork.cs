namespace AppCore.GenericRepo;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    int SaveChanges();
    Task<int> SaveChangesAsync();
}
