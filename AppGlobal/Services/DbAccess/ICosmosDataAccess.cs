using System.Runtime.CompilerServices;

namespace AppGlobal.Services.DbAccess;

public interface ICosmosDataAccess
{
    //List<T> GetCollection<T>();
    //Task<List<T>> GetData<T>(Expression<Func<T, bool>> predicate = null, [CallerMemberName] string callerName = "");
    Task<T> InsertRecord<T>(T entry, string partitionKey, [CallerMemberName] string callerName = "");

}
