namespace bank_identification_code.Core.Models
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;

  public class TNPEntity
    {
        public string VKEY { get; set; }

        public string TNP { get; set; }

        public ICollection<BNKSEEKEntity> BNKSEEKEntitys { get; set; }

        public string FULLNAME { get; set; }

        public string SHORTNAME { get; set; }

        public TNPEntity()
        {
            BNKSEEKEntitys = new Collection<BNKSEEKEntity>();
        }
    }
}