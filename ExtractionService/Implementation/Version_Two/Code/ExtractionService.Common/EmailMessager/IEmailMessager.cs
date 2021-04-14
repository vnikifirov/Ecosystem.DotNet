namespace ExtractionService.Common
{
    /// <summary>
    ///  Interface the Email Messager Service
    /// </summary>
    public interface IEmailMessager
    {
        IEmailMessager From(string fromAddress);
        IEmailMessager To(params string[] toAddresses);
        IEmailMessager WithSubject(string subject);
        IEmailMessager WithBody(string body);
        void SendWithAttachment(string username, string password, string fileName);
        void Send(string username, string password);
    }
}
