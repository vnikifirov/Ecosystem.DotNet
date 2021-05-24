using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TestGuestbook.Data
{
    [Guid("6D07675D-1F17-482D-A576-2DDBE5C8C7DF")]
    [CJE.Serializable(CJE.Serializable.Politics.AllExceptExcluded)]
    public class Message : CJE.ISerializable
    {
        public Guid ID { get; set; }
        public DateTime Created { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public IList<Data.Comment> Comments { get; set; }
        public int CommentsCount { get; set; }
        public double Rating { get; set; }

    }
}
