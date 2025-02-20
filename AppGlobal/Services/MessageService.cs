using System.Net.Mail;
using AppGlobal.Config;
using AppGlobal.Config.Communication;
using AppGlobal.Services.DbAccess;
using AppGlobal.Services.PubSub;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;

namespace AppGlobal.Services;

public class MessageService : IMessageService
{
    private readonly EmailConfig _emailConfig;
    private readonly AppSettings _appsettings;
    private readonly ILogger<MessageService> _logger;
    private readonly IMongoDataAccess _mongoAccess;
    private readonly IMessageBrokerService _messageBroker;
    public MessageService(IOptions<EmailConfig> emailConfig, ILogger<MessageService> logger, IOptions<AppSettings> appsettings, IMongoDataAccess mongoAccess, IMessageBrokerService messageBroker)
    {
        _emailConfig = emailConfig.Value;
        _logger = logger;
        _mongoAccess = mongoAccess;
        _messageBroker = messageBroker;
        _appsettings = appsettings.Value;
    }


    public async Task<GenResponse<string>> InsertNewMessageAndSendMail(EmailModelDTO entity, MessageBox msg)
    {
        try
        {
            var msgResp = await _mongoAccess.InsertRecord<MessageBox>(msg);
            if (!string.IsNullOrWhiteSpace(msg.Id))
            {
                if (await this.SendEmail(entity))
                {
                    _logger.LogInformation($"Email for {entity.ReceiverEmail} with token {msg.MessageData} for operation {msg.Operation} successfully sent.");
                    msg.IsProcessed = true;
                    msg.UpdatedAt = DateTime.Now;
                    var filterDefinition = Builders<MessageBox>.Filter.Eq(x => x.Id, msg.Id);
                    _ = _mongoAccess.GetCollection<MessageBox>().FindOneAndReplaceAsync<MessageBox>(filterDefinition, msg);
                }
                else
                {
                    _logger.LogError($"Email failed to send for {entity.ReceiverEmail} with token {msg.MessageData} for operation {msg.Operation}");
                    return GenResponse<string>.Failed("Message saved to database, but Email not sent.");
                }
                if (msg != null && !string.IsNullOrWhiteSpace(msg.Id))
                {
                    _ = _messageBroker.RabbitPublish<object>(new { operation = msg.Operation, email = msg.EmailReceiver, token = msg.MessageData });
                }
                return GenResponse<string>.Success(msg!.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(message: ex.Message);
            return GenResponse<string>.Failed("Oops, a server error occured. Kindly try again.", StatusCodeEnum.ServerError);
        }
        return GenResponse<string>.Failed("Failed to process request", StatusCodeEnum.NotImplemented);
    }

    public async Task<GenResponse<string>> InsertNewMessage(MessageBox msg)
    {
        try
        {
            MessageBox obj = await _mongoAccess.InsertRecord<MessageBox>(msg);
            if (!string.IsNullOrWhiteSpace(obj.Id))
            {
                _ = _messageBroker.RabbitPublish<object>(new { MessageId = obj.Id });
                return GenResponse<string>.Success(obj.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(message: ex.Message);
            return GenResponse<string>.Failed("Oops, a server error occured. Kindly try again.", StatusCodeEnum.ServerError);
        }
        return GenResponse<string>.Failed("Failed to process request", StatusCodeEnum.NotImplemented);
    }


    public async Task<bool> SendEmail(EmailModelDTO payload)
    {
        try
        {
            MailMessage mailMessage = new()
            {
                From = new MailAddress(_emailConfig.SenderEmail, _emailConfig.SenderName)
            };
            mailMessage.To.Clear();
            var emails = payload.ReceiverEmail.Split(new char[] { ',', ';', ':', '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var addr in emails)
            {
                mailMessage.To.Add(new MailAddress(addr));
            }
            mailMessage.Subject = payload.EmailSubject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = payload.EmailBody;

            //Configure SMTP Client and send message
            string envPwd = Environment.GetEnvironmentVariable(_emailConfig.SmtpPassword, EnvironmentVariableTarget.Process) ?? _emailConfig.SmtpPassword;
            using (var client = new SmtpClient())
            {
                if (_emailConfig.IsDevelopment)
                {
                    #region TEST
                    // set up the Gmail server
                    //NetworkCredential networkCred = new NetworkCredential(_emailConfig.SenderGMAIL, _emailConfig.Password);
                    client.Host = _emailConfig.SmtpHost;
                    client.EnableSsl = true;
                    client.Port = _emailConfig.SmtpPort;
                    client.UseDefaultCredentials = false;
                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Credentials = new System.Net.NetworkCredential(_emailConfig.SenderEmail, envPwd);
                    await client.SendMailAsync(mailMessage);
                    client.Dispose();
                    #endregion

                }
                else
                {
                    #region PRODUCTION
                    client.Host = _emailConfig.SmtpHost;
                    client.Port = _emailConfig.SmtpPort;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(_emailConfig.SenderEmail, envPwd);
                    client.Send(mailMessage);
                    client.Dispose();
                    #endregion
                }
            }
            //If execution gets here, message has been sent; hence, log and return successful response
            _logger.LogInformation("[MessageServiceRepository][SendEmail] SendEmail Success: Email sent successfully!");
            return true;
        }
        catch (Exception ex)
        {
            //Error occured, log and return failed response
            var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            OnaxTools.Logger.LogException(ex, $"[MessageServiceRepository][SendEmail] ==> SendMail Exception: {msg}");
            return false;
        }
    }



}


public interface IMessageService
{
    Task<GenResponse<string>> InsertNewMessageAndSendMail(EmailModelDTO entity, MessageBox msg);
    Task<GenResponse<string>> InsertNewMessage(MessageBox msg);
    Task<bool> SendEmail(EmailModelDTO payload);
}