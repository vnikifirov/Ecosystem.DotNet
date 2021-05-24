using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGuestbook.DB.Mapping
{
    class CommentMap : ClassMap<Comment>
    {
        public CommentMap()
        {
            Id(x => x.ID).GeneratedBy.GuidComb();            
            Map(x => x.Created);
            Map(x => x.Author);            
            Map(x => x.Content);            
        }
    }
}
