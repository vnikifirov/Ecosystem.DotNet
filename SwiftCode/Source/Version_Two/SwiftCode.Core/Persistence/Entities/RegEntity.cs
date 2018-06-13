
namespace SwiftCode.Core.Persistence.Entities
{
    using SwiftCode.Core.Interfaces.Models.Common;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public sealed class RegEntity : BaseModel
    {    
        public string RGN { get; set; }

        public ICollection<BnkseekEntity> BnkseekEntitys { get; set; }

        public string CENTER { get; set; }

        public string NAME { get; set; }

        public string NAMET { get; set; }

        public RegEntity()
        {
            BnkseekEntitys = new Collection<BnkseekEntity>();
        }
    }
}
