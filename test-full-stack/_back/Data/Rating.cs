using System;
using System.Runtime.InteropServices;

namespace TestGuestbook.Data
{
    [Guid("6D07675D-1F17-482D-A576-2DDBE5C8C7DF")]
    [CJE.Serializable(CJE.Serializable.Politics.AllExceptExcluded)]
    public class Rating : CJE.ISerializable
    {
        public Guid MessageID { get; set; }
        public double Value { get; set; }
    }
}