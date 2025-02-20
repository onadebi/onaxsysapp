using AppGlobal.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Domain.AppCore.Models.TransactionModels;

[Table(nameof(TransactionsPlatform))]
public class TransactionsPlatform: CommonEntity
{
    [Key]
    public override Guid Guid { get; set; }

    [Required]
    public required string PlatformName { get; set; }

    public string? PlatformTransactionId { get; set; }

    #region Navigation properties
    public ICollection<Transactions> TransactionsList { get; set; } = [];
    #endregion
}
