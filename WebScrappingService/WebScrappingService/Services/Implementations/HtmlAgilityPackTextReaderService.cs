using System.Collections.Generic;
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
        private Dictionary<string, string> _headers;
        private HtmlWeb.PreRequestHandler _handler;
        
        public HtmlAgilityPackTextReaderService(Dictionary<string, string> headers)
        {
            _headers = headers;

            Init();
        }

        private void Init()
        {
            _handler = delegate (HttpWebRequest request)
            {
                foreach (var header in _headers)
                    request.Headers.Add(header.Key, header.Value);
                request.CookieContainer = new System.Net.CookieContainer();
                return true;
            };
        }
        
        /// <inheritdoc/>
        public string ReadTextFrom(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            var sb = new StringBuilder();
            // StackOverflow about HtmlAgilityPack - https://stackoverflow.com/questions/18065526/pulling-data-from-a-webpage-parsing-it-for-specific-pieces-and-displaying-it 
            var web = new HtmlWeb();

            web.PreRequest += _handler;

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

            var sb = new StringBuilder();
            var web = new HtmlWeb();

            web.PreRequest += _handler;

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
