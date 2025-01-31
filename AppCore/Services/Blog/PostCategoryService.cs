using AppCore.Domain.Blog.Dto;
using AppCore.Domain.Blog.Entities;
using AppCore.Persistence;
using AppGlobal.Interface;
using AppGlobal.Services.DbAccess;
using Microsoft.Extensions.Logging;
using OnaxTools.Dto.Http;
using OnaxTools.Services.StackExchangeRedis.Interface;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace AppCore.Services.Blog;
public class PostCategoryService: IPostCategoryService
{
    private readonly ILogger<PostCategoryService> _logger;
    private readonly ISqlDataAccess sqlDataAccess;
    private readonly AppDbContext _context;
    private readonly ICacheService cacheService;

    public PostCategoryService(ILogger<PostCategoryService> logger, ISqlDataAccess sqlDataAccess, AppDbContext _context, ICacheService cacheService)
    {
        this._logger = logger;
        this.sqlDataAccess = sqlDataAccess;
        this._context = _context;
        this.cacheService = cacheService;
    }

    public Task<GenResponse<string?>> Add(PostCategoryCreateDto model, CancellationToken ct = default, [CallerMemberName] string? caller = null)
    {
        throw new NotImplementedException();
    }

    public Task<GenResponse<bool>> Delete(string Id, CancellationToken ct = default, [CallerMemberName] string? caller = null)
    {
        throw new NotImplementedException();
    }

    public Task<GenResponse<List<PostCategoryDto>>> FetchAll(int pageIndex = 0, int pageCount = 50, Expression<Func<PostCategory, bool>>? predicate = null, CancellationToken ct = default, [CallerMemberName] string? caller = null)
    {
        try
        {

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[{caller}]");
        }
        throw new NotImplementedException("Not implemented");
    }

    public Task<GenResponse<PostCategoryDto?>> FetchById(string id, Expression<Func<PostCategory, bool>>? predicate = null, CancellationToken ct = default, [CallerMemberName] string? caller = null)
    {
        throw new NotImplementedException();
    }

    public Task<GenResponse<bool>> Update(PostCategoryDto model, CancellationToken ct = default, [CallerMemberName] string? caller = null)
    {
        throw new NotImplementedException();
    }
}


public interface IPostCategoryService: ICommonBaseRepo<PostCategory,PostCategoryCreateDto,PostCategoryDto,PostCategoryDto>
{
}