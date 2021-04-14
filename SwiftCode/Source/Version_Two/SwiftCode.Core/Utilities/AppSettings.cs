namespace SwiftCode.Core.Utilities
{
    public sealed class AppSettings
    {
        public string[] AcceptedFileTypes { get; set; }
        public string FromEncoding { get; set; }
        public string ToEncoding { get; set; }
        public bool isNeedDecoding { get; set; }
        public string UploadFolder { get; set; }
    }
}
