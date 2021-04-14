namespace bank_identification_code.Core.Models
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  public class REGEntity
    {
        public string VKEY { get; set; }

        public string RGN { get; set; }

        public ICollection<BNKSEEKEntity> BNKSEEKEntitys { get; set; }

        public string CENTER { get; set; }

        public string NAME { get; set; }

        public string NAMET { get; set; }

        public REGEntity()
        {
            BNKSEEKEntitys = new Collection<BNKSEEKEntity>();
        }
    }
}