namespace SwiftCode.Core.Utilities
{
    using System.IO;
    using System.Linq;

    public static class FileValidator
    {        
        public static bool IsSupported(string fileName, string[] AcceptedFileTypes)
        {
            return AcceptedFileTypes.Any(s => s == Path.GetExtension(fileName).ToLower());
        }
    }
}
