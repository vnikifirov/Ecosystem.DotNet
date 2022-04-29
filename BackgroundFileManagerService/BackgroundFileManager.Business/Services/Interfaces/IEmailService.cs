using System.Threading;
using System.Threading.Tasks;

namespace BackgroundFileService.Business.Services.Interfaces
{
    /// <summary>
    /// Email Service which is sending emails 
    /// </summary>
    public interface IEmailService
    {
        IEmailService From(string fromAddress);
        IEmailService To(params string[] toAddresses);
        IEmailService WithSubject(string subject);
        IEmailService WithBody(string body);
        Task SendWithAttachmentAsync(string username, string password, string fileName, CancellationToken cancellationToken);
        Task SendAsync(string username, string password, CancellationToken cancellationToken);
    }
}
