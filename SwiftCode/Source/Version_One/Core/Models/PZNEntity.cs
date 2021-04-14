namespace bank_identification_code.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    public class PZNEntity
    {
        public string VKEY { get; set; }
        public string PZN { get; set; }

        public ICollection<BNKSEEKEntity> BNKSEEKEntitys { get; set; }

        public string IMY { get; set; }

        public string NAME { get; set; }

        public DateTime CB_DATE { get; set; }

        public DateTime CE_DATE { get; set; }

        public PZNEntity()
        {
            BNKSEEKEntitys = new Collection<BNKSEEKEntity>();
        }
    }
}