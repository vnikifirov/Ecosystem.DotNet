using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Business.Services.Interfaces;

namespace Business.Services.Implementations
{
    /// <inheritdoc/>
    public class WebSiteTextReaderService : ITextReaderService
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
            var web = new HtmlAgilityPack.HtmlWeb();
            HtmlDocument doc = web.Load(source);

            // Grab all text from html with Html Agility Pack - https://html-agility-pack.net/knowledge-base/4182594/grab-all-text-from-html-with-html-agility-pack
            /*foreach (var node in root.DescendantNodesAndSelf())
            {
                if (!node.HasChildNodes)
                {
                    string text = node.InnerText;
                    if (!string.IsNullOrEmpty(text))
                        sb.AppendLine(text.Trim());
                }
            }*/

            doc.DocumentNode
                .DescendantsAndSelf()
                .ToList()
                .ForEach(node =>
                {
                    if (!string.IsNullOrWhiteSpace(node.InnerText))
                        sb.AppendLine(node.InnerText.Trim());
                });

            return sb.ToString();
        }
    }
}
