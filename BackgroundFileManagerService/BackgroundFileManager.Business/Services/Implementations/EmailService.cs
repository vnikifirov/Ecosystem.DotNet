using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using BackgroundFileService.Business.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BackgroundFileService.Business.Services.Implementations
{
    /// <inheritdoc/>
    public class EmailService : IEmailService
    {
        private readonly MailMessage _mailMessage;

        /// <summary>
        /// Logging
        /// </summary>
        private readonly ILogger _logger;

        private EmailService(ILogger logger, string fromAddress)
        {
            _logger = logger;
            _mailMessage = new MailMessage();
            _mailMessage.Sender = new MailAddress(fromAddress);
        }

        public IEmailService From(string fromAddress) => this;

        public IEmailService To(params string[] toAddresses)
        {
            foreach (string toAddress in toAddresses)
                _mailMessage.To.Add(new MailAddress(toAddress));

            return this;
        }

        public IEmailService WithSubject(string subject)
        {
            _mailMessage.Subject = subject;

            return this;
        }

        public IEmailService WithBody(string body)
        {
            _mailMessage.Body = body;

            return this;
        }

        // TODO: Mtake support of async / await  
        public async Task SendWithAttachmentAsync(string username, string password, string fileName, CancellationToken cancellationToken)
        {
            byte[] data;

            try
            {
                data = await System.IO.File.ReadAllBytesAsync(fileName, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            using var ms = new System.IO.MemoryStream(data);
 
            // Create the file attachment for e-mail message.
            var attachment = new Attachment(ms, MediaTypeNames.Application.Octet);

            // Add time stamp information for the file.
            ContentDisposition disposition = attachment.ContentDisposition;

            try
            {

                disposition.FileName = System.IO.Path.GetFileName(fileName);
                disposition.Size = data.Length;
                disposition.CreationDate = System.IO.File.GetCreationTime(fileName);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(fileName);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            // Add the file attachment to the list e-mail attachments.
            _mailMessage.Attachments.Add(attachment);

            // Send email
            await SendAsync(username, password, cancellationToken);

            // Close MemoryStream
            ms.Close();
        }

        public async Task SendAsync(string username, string password, CancellationToken cancellationToken)
        {
            try
            {
                var smtpClient = new SmtpClient()
                {
                    Credentials = new NetworkCredential(username, password),
                };

                await smtpClient.SendMailAsync(_mailMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
