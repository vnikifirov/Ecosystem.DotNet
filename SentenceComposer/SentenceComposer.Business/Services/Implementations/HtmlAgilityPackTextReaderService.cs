using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SentenceComposer.Business.Services.Interfaces;

namespace SentenceComposer.Business.Services.Implementations
{
    public class HtmlAgilityPackTextReaderService : ITextReaderService
    {
        /// <inheritdoc/>
        public string ReadTextFrom(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            // For Russia letters you have to use 1251 - https://stackoverflow.com/questions/31328364/when-reading-a-file-i-get-as-input-instead-of-cyrillic-letters
            // No data is available for encoding 1252 - https://stackoverflow.com/questions/50858209/system-notsupportedexception-no-data-is-available-for-encoding-1252
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var sb = new StringBuilder();
            // StackOverflow about HtmlAgilityPack - https://stackoverflow.com/questions/18065526/pulling-data-from-a-webpage-parsing-it-for-specific-pieces-and-displaying-it 
            var web = new HtmlWeb();
            var HTMLdoc = web.Load(source);

            HTMLdoc.DocumentNode
                .DescendantsAndSelf()
                .ToList()
                .ForEach(node =>
                {
                    // Only if HTML node is type Text
                    if (node.NodeType == HtmlNodeType.Text)
                        // Only if HTML node contains text and isn't empty
                        if (!string.IsNullOrWhiteSpace(node.InnerText))
                            // Take only text from node
                            sb.AppendLine(node.InnerText.Trim());

                });

            return sb.ToString();
        }

        /// <inheritdoc/>
        public async Task<string> ReadTextFromAsync(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            // For Russia letters you have to use 1251 - https://stackoverflow.com/questions/31328364/when-reading-a-file-i-get-as-input-instead-of-cyrillic-letters
            // No data is available for encoding 1252 - https://stackoverflow.com/questions/50858209/system-notsupportedexception-no-data-is-available-for-encoding-1252
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var sb = new StringBuilder();
            // StackOverflow about HtmlAgilityPack - https://stackoverflow.com/questions/18065526/pulling-data-from-a-webpage-parsing-it-for-specific-pieces-and-displaying-it 
            var web = new HtmlWeb();
            var HTMLdoc = await web.LoadFromWebAsync(source);

            HTMLdoc.DocumentNode
                .DescendantsAndSelf()
                .ToList()
                .ForEach(node =>
                {
                    // Only if HTML node is type Text
                    if (node.NodeType == HtmlNodeType.Text)
                        // Only if HTML node contains text and isn't empty
                        if (!string.IsNullOrWhiteSpace(node.InnerText))
                            // Take only text from node
                            sb.AppendLine(node.InnerText.Trim());

                });

            return sb.ToString();
        }
    }
}
