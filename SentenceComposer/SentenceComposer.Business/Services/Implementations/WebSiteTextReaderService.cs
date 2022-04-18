using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Business.Services.Implementations
{
    /// <inheritdoc/>
    public class WebSiteTextReaderService : ITextReaderService
    {
        private readonly int _russiaEncoding = 1251;

        /// <inheritdoc/>
        public string ReadTextFrom(string source)
        {
            // Read data from website - https://stackoverflow.com/questions/4758283/reading-data-from-a-website-using-c-sharp
            // For Russia letters - https://stackoverflow.com/questions/31328364/when-reading-a-file-i-get-as-input-instead-of-cyrillic-letters
            // No data is available for encoding 1252 - https://stackoverflow.com/questions/50858209/system-notsupportedexception-no-data-is-available-for-encoding-1252
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            using var web = new WebClient();
            using var stream = web.OpenRead(source);
            using var reader = new StreamReader(stream, Encoding.GetEncoding(_russiaEncoding));
            return reader.ReadToEnd();
        }

        /// <inheritdoc/>
        public async Task<string> ReadTextFromAsync(string source)
        {
            // Read data from website - https://stackoverflow.com/questions/4758283/reading-data-from-a-website-using-c-sharp
            // For Russia letters - https://stackoverflow.com/questions/31328364/when-reading-a-file-i-get-as-input-instead-of-cyrillic-letters
            // No data is available for encoding 1252 - https://stackoverflow.com/questions/50858209/system-notsupportedexception-no-data-is-available-for-encoding-1252
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            using var web = new WebClient();
            var uri = new Uri(source);
            using var stream = await web.OpenReadTaskAsync(uri);
            using var reader = new StreamReader(stream, Encoding.GetEncoding(_russiaEncoding));
            return await reader.ReadToEndAsync();
        }
    }
}
