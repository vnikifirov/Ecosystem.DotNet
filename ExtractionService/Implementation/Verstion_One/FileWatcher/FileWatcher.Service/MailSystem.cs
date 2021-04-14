using System;
using System.Net;
using System.Net.Mail;

namespace FileWatcher
{
    /// <summary>
    ///  The interface implemented for the sender of the service
    /// </summary>
    public interface IEmail
    {
        IEmail From(string fromAddress);
        IEmail To(params string[] toAddresses);
        IEmail WithSubject(string subject);
        IEmail WithBody(string body);
        void Send(string username, string password, int timeout = 100);
        //void Send(string username, string password, string host, int timeout = 100);
    }

    /// <summary>
    ///  The MailSystem class is mail system
    /// </summary>
    public class MailSystem : IEmail
    {
        private MailMessage _mailMessage = new MailMessage();

        private MailSystem(string fromAddress)
        {
            _mailMessage.Sender = new MailAddress(fromAddress);
        }

        public static IEmail CreateEmailFrom(string fromAddress)
        {
            return new MailSystem(fromAddress);
        }

        public IEmail From(string fromAddress)
        {
            return this;
        }

        public IEmail To(params string[] toAddresses)
        {
            foreach (string toAddress in toAddresses)
            {
                _mailMessage.To.Add(new MailAddress(toAddress));
            }

            return this;
        }

        public IEmail WithSubject(string subject)
        {
            _mailMessage.Subject = subject;

            return this;
        }

        public IEmail WithBody(string body)
        {
            _mailMessage.Body = body;

            return this;
        }

        public void Send(string username, string password, int timeout = 100)
        {
            try
            {
                // Set up the mail server
                SmtpClient smtpClient = new SmtpClient()
                {
                    Credentials = new NetworkCredential(username, password),
                    Timeout = timeout
                };

                // Send the email
                smtpClient.SendAsync(_mailMessage, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void Send(string username, string password, string host, int timeout = 100)
        //{
        //    try
        //    {
        //        // Set up the mail server
        //        SmtpClient smtpClient = new SmtpClient
        //        {
        //            Host = host,
        //            Credentials = new NetworkCredential(username, password),
        //            Timeout = timeout
        //        };            
        //        // Send the email
        //        smtpClient.SendAsync(_mailMessage, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

    }
}
