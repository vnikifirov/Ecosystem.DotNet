using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGuestbook.DB.Mapping
{
    class RatingMap : ClassMap<Rating>
    {
        public RatingMap()
        {
            Id(x => x.ID).GeneratedBy.GuidComb();
            Map(x => x.Value);
        }
    }
}
