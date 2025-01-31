namespace AppGlobal.Config.Communication;

public class EmailModelDTO
    {
        public required string ReceiverEmail { get; set; }
        public required string EmailSubject { get; set; }
        public required string EmailBody { get; set; }
    }

    public class EmailModelWithDataDTO : EmailModelDTO
    {
        public Dictionary<string,string> EmailBodyData { get; set; } = [];
    }