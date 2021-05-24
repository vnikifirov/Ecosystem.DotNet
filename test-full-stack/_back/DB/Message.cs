using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGuestbook.DB
{
    public class Message
    {
        public virtual Guid ID { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual string Author { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }

        public virtual IList<Comment> Comments { get; set; }
        public virtual IList<Rating> Ratings { get; set; }

        public Message() { }
        public Message(Data.Message source)
        {
            this.ID = source.ID;
            this.Created = source.Created;
            this.Author = source.Author;
            this.Title = source.Title;
            this.Content = source.Content;
        }

        public virtual Data.Message ToData(bool withComments, bool withRating)
        {
            return new Data.Message()
            {
                ID = this.ID,
                Created = this.Created,
                Author = this.Author,
                Title = this.Title,
                Content = this.Content,

                Comments = (withComments && this.Comments != null && NHibernate.NHibernateUtil.IsInitialized(this.Comments)) ? this.Comments.Select(x => x.ToData(false)).ToList() : new List<Data.Comment>(),
                CommentsCount = (this.Comments != null && NHibernate.NHibernateUtil.IsInitialized(this.Comments)) ? this.Comments.Count : -1,
                Rating = (withRating && this.Ratings != null && this.Ratings.Count > 0) ? this.Ratings.Average(x => x.Value) : -1d,
            };
        }
    }
}
