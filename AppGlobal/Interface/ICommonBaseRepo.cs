using OnaxTools.Dto.Http;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace AppGlobal.Interface;


public interface ICommonBaseRepo<TEntity, TCreateEntity, TUpdateEntity, TFetchEntity>
        where TCreateEntity : class
        where TUpdateEntity : class
        where TFetchEntity : class
        where TEntity : class
{
    Task<GenResponse<string?>> Add(TCreateEntity model, CancellationToken ct = default, [CallerMemberName] string? caller = null);
    Task<GenResponse<bool>> Update(TUpdateEntity model, CancellationToken ct = default, [CallerMemberName] string? caller = null);
    Task<GenResponse<List<TFetchEntity>>> FetchAll(int pageIndex = 0, int pageCount = 50, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default, [CallerMemberName] string? caller = null);
    Task<GenResponse<TFetchEntity?>> FetchById(string id, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default, [CallerMemberName] string? caller = null);
    Task<GenResponse<bool>> Delete(string Id, CancellationToken ct = default, [CallerMemberName] string? caller = null);

}
