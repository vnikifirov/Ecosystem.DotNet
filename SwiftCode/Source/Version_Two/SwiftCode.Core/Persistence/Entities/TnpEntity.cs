
namespace SwiftCode.Core.Persistence.Entities
{
    using SwiftCode.Core.Interfaces.Models.Common;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public sealed class TnpEntity : BaseModel
    {        
        public string TNP { get; set; }

        public ICollection<BnkseekEntity> BnkseekEntitys { get; set; }

        public string FULLNAME { get; set; }

        public string SHORTNAME { get; set; }

        public TnpEntity()
        {
            BnkseekEntitys = new Collection<BnkseekEntity>();
        }
    }
}
