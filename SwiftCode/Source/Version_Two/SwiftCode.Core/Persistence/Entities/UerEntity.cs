namespace SwiftCode.Core.Persistence.Entities
{
    using SwiftCode.Core.Interfaces.Models.Common;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public sealed class UerEntity : BaseModel
    {
        public string UER { get; set; }

        public ICollection<BnkseekEntity> BnkseekEntitys { get; set; }

        public string UERNAME { get; set; }

        public UerEntity()
        {
            BnkseekEntitys = new Collection<BnkseekEntity>();
        }
    }
}
