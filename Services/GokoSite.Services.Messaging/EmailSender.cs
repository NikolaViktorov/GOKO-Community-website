namespace GokoSite.Services.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;

    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string from, string fromName, string to, string subject, string htmlContent, IEnumerable<EmailAttachment> attachments = null)
        {
            MailAddress toM = new MailAddress(to);
            MailAddress fromM = new MailAddress(from);

            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = htmlContent;

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("locahlost", 25);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                await client.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
