using System;
using System.IO;
using System.Net;
using System.Text;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Business.Services.Implementations
{
    /// <inheritdoc/>
    public class WebSiteTextReaderService : ITextReaderService
    {
        /// <inheritdoc/>
        public string ReadTextFrom(string source)
        {
            // Read data from website - https://stackoverflow.com/questions/4758283/reading-data-from-a-website-using-c-sharp
            WebClient web = null;
            Stream stream = null;
            StreamReader reader = null;
            var text = string.Empty;

            // For Russia letters - https://stackoverflow.com/questions/31328364/when-reading-a-file-i-get-as-input-instead-of-cyrillic-letters
            // No data is available for encoding 1252 - https://stackoverflow.com/questions/50858209/system-notsupportedexception-no-data-is-available-for-encoding-1252
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            try
            {
                web = new WebClient();
                stream = web.OpenRead(source);
                reader = new StreamReader(stream, Encoding.GetEncoding(1251));
                text = reader.ReadToEnd();
            }
            finally
            {
                web.Dispose();
                stream.Dispose();
                reader.Dispose();
            }

            return text;
        }
    }
}
