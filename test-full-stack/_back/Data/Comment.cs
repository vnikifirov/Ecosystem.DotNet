using System;
using System.Runtime.InteropServices;

namespace TestGuestbook.Data
{
    [Guid("6D07675D-1F17-482D-A576-2DDBE5C8C7DF")]
    [CJE.Serializable(CJE.Serializable.Politics.AllExceptExcluded)]
    public class Comment : CJE.ISerializable
    {
        public Guid ID { get; set; }
        public Guid MessageID { get; set; }
        public DateTime Created { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
    }
}