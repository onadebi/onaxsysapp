using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using AppGlobal.Config;

namespace AppGlobal.Models.Logger;

[Table(nameof(AppActivityLog))]
public class AppActivityLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonElement(nameof(CallerMemberName))]
    public string? CallerMemberName { get; set; }


    [BsonRequired]
    [BsonElement(nameof(Operation))]
    public required string Operation { get; set; }

    [BsonRequired]
    [BsonElement(nameof(MessageData))]
    public required string MessageData { get; set; }

    [BsonRequired]
    [BsonElement(nameof(IsSuccessfulOperation))]
    public required bool IsSuccessfulOperation { get; set; }

    [BsonRequired]
    [BsonElement(nameof(AppName))]
    public required string AppName { get; set; }

    
    [BsonRequired]
    [BsonElement(nameof(CreatedAt))]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonRequired]
    [BsonElement(nameof(UpdatedAt))]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonRequired]
    [BsonElement(nameof(OperationBy))]
    public required string OperationBy { get; set; } = AppConstants.AppSystem;
}
