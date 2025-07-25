﻿using AppGlobal.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Domain.AppCore.Models.TransactionModels;

[Table(nameof(Transactions))]
public class Transactions: CommonEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public override Guid Guid { get; set; }

    [Required]
    public required Guid TransactionPlatformId { get; set; }

    [Required]
    public required string PlatformTransactionId { get; set; }

    [Required]
    public required decimal Amount { get; set; }

    [Required]
    public int Credits { get; set; }

    [Required]
    public required string UserGuid { get; set; }

    public string? Plan { get; set; }

    public bool CompletionStatus { get; set; }

    #region Navigation properties
    public TransactionsPlatform? TransactionsPlatform { get; set; }
    public UserProfile? UserProfileGuid { get; set; }
    #endregion
}
