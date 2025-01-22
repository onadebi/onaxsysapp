namespace AppCore.Domain.AppCore.Dto
{
    public class EmailConfig
    {
        public EmailConfig() { }

        public required string SenderName { get; set; }
        public required string SenderEmail { get; set; }
        public required string SmtpService { get; set; }
        public required string SmtpPassword { get; set; }
        public required int SmtpPort { get; set; }
        public required string SmtpHost { get; set; }
        public bool IsDevelopment { get; set; }
    }
}
