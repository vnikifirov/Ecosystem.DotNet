using System;

namespace TestGuestbook.DB
{
    public class Rating
    {
        public virtual Guid ID { get; set; }
        public virtual Guid MessageID { get; set; }
        public virtual double Value { get; set; }

        public Rating() { }
        public Rating(Data.Rating source)
        {
            this.MessageID = source.MessageID;                        
            this.Value = source.Value;
        }

        public virtual Data.Rating ToData()
        {
            return new Data.Rating
            {
                MessageID = this.MessageID,
                Value = this.Value
            };
        }
    }
}