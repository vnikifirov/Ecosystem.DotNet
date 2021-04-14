
namespace SwiftCode.Core.Persistence.Entities
{
    using SwiftCode.Core.Interfaces.Models.Common;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public sealed class PznEntity : BaseModel
    {
        public string PZN { get; set; }

        public ICollection<BnkseekEntity> BnkseekEntitys { get; set; }

        public string IMY { get; set; }

        public string NAME { get; set; }

        public DateTime CB_DATE { get; set; }

        public DateTime CE_DATE { get; set; }

        public PznEntity()
        {
            BnkseekEntitys = new Collection<BnkseekEntity>();
        }
    }
}
