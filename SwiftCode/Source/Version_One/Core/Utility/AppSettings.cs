
namespace bank_identification_code.Core.Utility
{
    public class AppSettings
    {
        public string[] AcceptedFileTypes { get; set; }

        public string BaseEncoding { get; set; }

        public string DestEncoding { get; set; }
        public bool isNeedDecoding { get; set; }
    }
}