namespace ExtractionService.Common
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;

    /// <summary>
    ///  Implemntation of an Email Messager Service
    /// </summary>
    public class EmailMessager : IEmailMessager
    {
        private MailMessage _mailMessage = new MailMessage();

        private EmailMessager(string fromAddress)
        {
            _mailMessage.Sender = new MailAddress(fromAddress);
        }

        public static IEmailMessager CreateEmailFrom(string fromAddress)
        {
            return new EmailMessager(fromAddress);
        }

        public IEmailMessager From(string fromAddress)
        {
            return this;
        }

        public IEmailMessager To(params string[] toAddresses)
        {
            foreach (string toAddress in toAddresses)
            {
                _mailMessage.To.Add(new MailAddress(toAddress));
            }

            return this;
        }

        public IEmailMessager WithSubject(string subject)
        {
            _mailMessage.Subject = subject;

            return this;
        }

        public IEmailMessager WithBody(string body)
        {
            _mailMessage.Body = body;

            return this;
        }

        public void SendWithAttachment(string username, string password, string fileName)
        {
            byte[] data = System.IO.File.ReadAllBytes(fileName);

            using (var ms = new System.IO.MemoryStream(data))
            {            
                // Create the file attachment for e-mail message.
                var attachment = new Attachment(ms, MediaTypeNames.Application.Octet);

                // Add time stamp information for the file.
                ContentDisposition disposition = attachment.ContentDisposition;

                disposition.FileName = System.IO.Path.GetFileName(fileName);
                disposition.Size = data.Length;
                disposition.CreationDate = System.IO.File.GetCreationTime(fileName);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(fileName);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(fileName);

                // Add the file attachment to the list e-mail attachments.
                _mailMessage.Attachments.Add(attachment);

                // Send email
                Send(username, password);

                // Close MemoryStream
                ms.Close();
            }                     
        }

        public void Send(string username, string password)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient()
                {
                    Credentials = new NetworkCredential(username, password),
                };

                smtpClient.Send(_mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
