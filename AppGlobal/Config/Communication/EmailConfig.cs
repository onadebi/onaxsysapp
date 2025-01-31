using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppGlobal.Config.Communication;

public class EmailConfig
{

    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string SmtpService { get; set; }
    public required string SmtpPassword { get; set; }
    public int SmtpPort { get; set; }
    public required string SmtpHost { get; set; }
    public bool IsDevelopment { get; set; }
}
