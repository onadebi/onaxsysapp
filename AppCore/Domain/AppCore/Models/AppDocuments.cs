using AppCore.Domain.AppCore.Config;
using AppCore.Domain.AppCore.Models.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Domain.AppCore.Models
{
    [Table(nameof(AppDocuments), Schema = SchemaConstants.ProfileSchema)]
    public class AppDocuments : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocId { get; set; }

        [Required]
        [StringLength(250)]
        public required string DocumentName { get; set; }

        [StringLength(250)]
        public string? DocumentDescription{get; set;}
        
        [Required]
        [StringLength(250)]
        public string[] DocumentAllowedFormats { get; set; } = Array.Empty<string>();

        [Required]
        public decimal MaxMbFileSize { get; set; }

        public int Category { get; set; } = 0;

        #region Navaigational properties
        public ICollection<UserDocument> UserAppDocument { get; set; } = [];
        #endregion

    }
}
