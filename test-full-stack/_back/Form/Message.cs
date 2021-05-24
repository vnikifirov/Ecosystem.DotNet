using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestGuestbook.Form
{
    public class Message : CJE.Form.FormData
    {
        public Message(CJE.Form.DataRaw data) : base(data, true) { }

        [CJE.Form.Value("ID", typeof(CJE.Form.Values.GuidParser))]
        public Guid ID;

        [CJE.Form.Value("Created", typeof(CJE.Form.Values.DateTimeParser))]
        public DateTime Created;

        [CJE.Form.Value("Author", typeof(CJE.Form.Values.StringParser))]
        public string Author;

        [CJE.Form.Value("Title", typeof(CJE.Form.Values.StringParser))]
        public string Title;

        [CJE.Form.Value("Content", typeof(CJE.Form.Values.StringParser))]
        public string Content;

        public Data.Message ToData()
        {
            return new Data.Message()
            {
                ID = this.ID,
                Created = this.Created,
                Author = this.Author,
                Title = this.Title,
                Content = this.Content,
            };
        }
    }
}
