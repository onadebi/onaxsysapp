using System.ComponentModel.DataAnnotations;

namespace AppCore.Domain.AppCore.Dto
{
    public class EmailModelDTO
    {
        [Required]
        public required string ReceiverEmail { get; set; }
        [Required]
        public required string EmailSubject { get; set; }
        [Required]
        public required string EmailBody { get; set; }
    }

    public class EmailModelWithDataDTO : EmailModelDTO
    {
        [Required]
        public Dictionary<string,string>? EmailBodyData { get; set; }
    }
}
