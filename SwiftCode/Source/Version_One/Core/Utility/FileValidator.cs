
namespace bank_identification_code.Core.Utility
{
    using System.IO;
    using System.Linq;

    public static class FileValidator
    {
        public static bool IsSupported(string[] AcceptedFileTypes, string fileName)
        {
            return AcceptedFileTypes.Any(s=> s == Path.GetExtension(fileName).ToLower());
        }
    }
}