using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace bank_identification_code.Core.Models
{
    public class UEREntity
    {
        public string VKEY { get; set; }

        public string UER { get; set; }

        public ICollection<BNKSEEKEntity> BNKSEEKEntitys { get; set; }

        public string UERNAME { get; set; }

        public UEREntity()
        {
            BNKSEEKEntitys = new Collection<BNKSEEKEntity>();
        }
    }
}