using Microsoft.Azure.Cosmos;
using System.Runtime.CompilerServices;

namespace AppGlobal.Services.DbAccess;
public class CosmosDataAccess : ICosmosDataAccess
{
    private Container _container;

    public CosmosDataAccess(CosmosClient dbClient,
            string databaseName,
            string containerName)
    {
        this._container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task<T> InsertRecord<T>(T entry, string partitionKey, [CallerMemberName] string callerName = "")
    {
        ItemResponse<T> objResp = await _container.UpsertItemAsync<T>(entry, partitionKey: new PartitionKey(partitionKey));
        return objResp;
    }
}

