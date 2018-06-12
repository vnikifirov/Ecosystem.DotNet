namespace ExtractionService
{
    using System.IO;
    using System.Text;

    public class LogMessageHelper
    {        
        private const int MAX_LENGTH_FILE = 64;

        public static string GetMessage(string[] files)
        {
            var strBuilder = new StringBuilder();

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                strBuilder.AppendFormat("{0,-32}{1,16} bytes", fileInfo.Name, fileInfo.Length)
                          .AppendLine();
            }

            return strBuilder.AppendLine().ToString();
        }
    }
}
