using System;

namespace TestGuestbook.Form
{
    internal class Rating : CJE.Form.FormData
    {
        public Rating(CJE.Form.DataRaw data) : base(data, true) { }

        [CJE.Form.Value("MessageID", typeof(CJE.Form.Values.GuidParser))]
        public Guid MessageID;

        [CJE.Form.Value("Value", typeof(CJE.Form.Values.DoubleParser))]
        public double Value;

        public Data.Rating ToData()
        {
            return new Data.Rating
            {
                MessageID = this.MessageID,
                Value = this.Value
            };
        }
    }
}