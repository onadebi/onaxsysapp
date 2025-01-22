using Dapper;
using Npgsql;
using System.Data;
using System.Runtime.CompilerServices;

namespace AppGlobal.Services.DbAccess;
public class SqlDataAccess : ISqlDataAccess
{
    private readonly string _conString;

    public SqlDataAccess(string conString)
    {
        this._conString = conString;
    }

    public async Task<IEnumerable<T>> GetData<T, U>(string queryString, U parameters, CommandType commandType = CommandType.Text, [CallerMemberName] string callerName = "")
    {
        using IDbConnection conn = new NpgsqlConnection(_conString);
        IEnumerable<T> objResp = Array.Empty<T>();
        try
        {
            objResp = await conn.QueryAsync<T>(queryString, param: parameters, commandType: commandType);
        }
        catch (Exception ex)
        {
           throw new Exception(callerName, ex);
        }
        return objResp;
    }

    public async Task<int> SaveData<T>(string queryString, T parameters, CommandType commandType = CommandType.Text, [CallerMemberName] string callerName = "")
    {
        int countOfRecordsModified = 0;
        try
        {
            using IDbConnection conn = new NpgsqlConnection(_conString);
            conn.Open();
            countOfRecordsModified = await conn.ExecuteAsync(queryString, param: parameters, commandType: commandType);
        }
        catch (Exception ex)
        {
            throw new Exception(callerName, ex);
        }
        return countOfRecordsModified;
    }
}


public interface ISqlDataAccess
{
    Task<IEnumerable<T>> GetData<T, U>(string queryString, U parameters, CommandType commandType = CommandType.Text, [CallerMemberName] string callerName = "");
    Task<int> SaveData<T>(string queryString, T parameters, CommandType commandType = CommandType.Text, [CallerMemberName] string callerName = "");
}
