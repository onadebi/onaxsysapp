using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AppGlobal.Config.Communication;

[Table(nameof(MessageBox))]
[BsonIgnoreExtraElements]
public class MessageBox
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonProperty(PropertyName = nameof(Id))]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [BsonElement(nameof(AppName))]
    [JsonProperty(PropertyName = nameof(AppName))]
    [StringLength(maximumLength: 100)]
    public required string AppName { get; set; }

    [Required]
    [BsonElement(nameof(Operation))]
    [JsonProperty(PropertyName = nameof(Operation))]
    [StringLength(maximumLength: 100)]
    public required string Operation { get; set; }

    /// <summary>
    /// Email || SMS || WhatsApp || ... This defaults to Email
    /// </summary>
    [BsonElement(nameof(Channel))]
    [JsonProperty(PropertyName = nameof(Channel))]
    [StringLength(maximumLength: 100)]
    public string Channel { get; set; } = "Email";

    [BsonElement(nameof(Description))]
    [JsonProperty(PropertyName = nameof(Description))]
    [StringLength(maximumLength: 250)]
    public string? Description { get; set; }

    [BsonElement(nameof(MessageData))]
    [JsonProperty(PropertyName = nameof(MessageData))]
    [StringLength(maximumLength: 500)]
    public required string MessageData { get; set; }

    #region
    //TODO: Move below properties to seperate table TokenBox
    //[BsonElement(nameof(ExpireAt))]
    //public DateTime ExpireAt { get; set; } = DateTime.UtcNow.AddDays(1);
    #endregion

    [Required]
    [BsonElement(nameof(EmailReceiver))]
    [JsonProperty(PropertyName = nameof(EmailReceiver))]
    [StringLength(maximumLength: 100)]
    public required string EmailReceiver { get; set; }


    [BsonRepresentation(BsonType.DateTime)]
    [JsonProperty(PropertyName = nameof(CreatedAt))]
    [BsonElement(nameof(CreatedAt))]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [BsonElement(nameof(UpdatedAt))]
    [JsonProperty(PropertyName = nameof(UpdatedAt))]
    public DateTime? UpdatedAt { get; set; }


    [BsonRepresentation(BsonType.DateTime)]
    [BsonElement(nameof(ExpiredAt))]
    [JsonProperty(PropertyName = nameof(ExpiredAt))]
    public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddDays(1);


    [BsonElement(nameof(UserId))]
    [JsonProperty(PropertyName = nameof(UserId))]
    [StringLength(maximumLength: 100)]
    public string? UserId { get; set; }

    [Required]
    [BsonElement(nameof(ForQueue))]
    [JsonProperty(PropertyName = nameof(ForQueue))]
    public bool ForQueue { get; set; } = false;

    [Required]
    [BsonElement(nameof(IsProcessed))]
    [JsonProperty(PropertyName = nameof(IsProcessed))]
    public bool IsProcessed { get; set; } = false;

    [Required]
    [BsonElement(nameof(IsUsed))]
    [JsonProperty(PropertyName = nameof(IsUsed))]
    public bool IsUsed { get; set; } = false;

    /// <summary>
    /// Default status of 0 indicates not yet used. -1: Pending/Expired. 1: Used/Processed
    /// </summary>
    [Required]
    [BsonElement(nameof(CompletedStatus))]
    [JsonProperty(PropertyName = nameof(CompletedStatus))]
    public short CompletedStatus { get; set; } = 0;



}
