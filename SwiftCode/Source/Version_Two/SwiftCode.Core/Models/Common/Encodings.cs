
namespace SwiftCode.Core.Models.Common
{
    public sealed class Encodings
    {
        public Encodings(string fromEncoding, string toEncoding)
        {
            FromEncoding = fromEncoding;
            ToEncoding = toEncoding;
        }

        public string FromEncoding { get; private set; }
        public string ToEncoding { get; private set; }
    }
}
