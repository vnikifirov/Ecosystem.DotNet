using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGuestbook.DB.Mapping
{
    public class MessageMap : ClassMap<Message>
    {
        public MessageMap()
        {
            Id(x => x.ID).GeneratedBy.GuidComb();
            Map(x => x.Created);
            Map(x => x.Author);
            Map(x => x.Title);
            Map(x => x.Content);

            HasMany(x => x.Comments).Cascade.AllDeleteOrphan();
            HasMany(x => x.Ratings).Cascade.AllDeleteOrphan();
        }
    }
}
