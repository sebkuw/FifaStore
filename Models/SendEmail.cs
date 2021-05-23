using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace FifaStore.Models
{
    public class SendEmail
    {
        public static bool EmailSend(string SenderEmail, string Subject, string Message, bool IsBodyHtml)
        {
            bool status = false;
            try
            {
                string HostAddress = ConfigurationManager.AppSettings["Host"].ToString();
                string FormEmailId = ConfigurationManager.AppSettings["MailFrom"].ToString();
                string Password = ConfigurationManager.AppSettings["Password"].ToString();
                string Port = ConfigurationManager.AppSettings["Port"].ToString();
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(FormEmailId),
                    Subject = Subject,
                    Body = Message,
                    IsBodyHtml = IsBodyHtml,
                };
                mailMessage.To.Add(new MailAddress(SenderEmail));

                SmtpClient smtp = new SmtpClient
                {
                    Port = int.Parse(Port),
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(mailMessage.From.Address, Password),
                    Host = HostAddress,
                    EnableSsl = true
                };

                smtp.Send(mailMessage);
                status = true;
                return status;
            }
            catch (Exception e)
            {
                return status;
            }
        }
    }
}