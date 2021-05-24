using System;

namespace TestGuestbook.DB
{
    public class Comment
    {
        public virtual Guid ID { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual string Author { get; set; }
        public virtual string Content { get; set; }

        public Comment() {}

        public Comment(Data.Comment source)
        {
            this.ID = source.ID;
            this.Created = source.Created;
            this.Author = source.Author;
            this.Content = source.Content;
        }

        public virtual Data.Comment ToData(bool v)
        {
            return new Data.Comment()
            {
                ID = this.ID,
                Created = this.Created,
                Author = this.Author,
                Content = this.Content
            };
        }
    }
}