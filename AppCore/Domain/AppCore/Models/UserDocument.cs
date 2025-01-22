using AppCore.Domain.AppCore.Models.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Domain.AppCore.Models
{
    [Table(nameof(UserDocument), Schema = "profile")]
    public class UserDocument: CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong DocId { get; set; }

        [Required]
        public int UserProfileId { get; set; }

        [Required]
        public int AppDocumentId { get; set; }

        [Required]
        [StringLength(500)]
        public required string DocumentName { get; set; }

        [StringLength(500)]
        public required string FullDocumentNamePath { get; set; }

        /// <summary>
        /// This should be in list of acceptable types from enum [AppDocumentExtensionTypes]
        /// </summary>
        [Required]
        [StringLength(100)]
        public required string ExtensionType { get; set; }

        #region Navaigational properties
        public AppDocuments AppUserDocuments { get; set; } = default!;
        #endregion
    }
}
