
namespace SwiftCode.Core.Persistence.Entities
{
    using System;
    using SwiftCode.Core.Interfaces.Models.Common;

    public sealed class BnkseekEntity : BaseModel
    {
        public string REAL { get; set; }

        public string PZN { get; set; }

        public PznEntity PznEntity { get; set; }

        public string UER { get; set; }

        public UerEntity UerEntity { get; set; }

        public string RGN { get; set; }

        public RegEntity RegEntity { get; set; }

        public string IND { get; set; }

        public string TNP { get; set; }

        public TnpEntity TnpEntity { get; set; }

        public string NNP { get; set; }

        public string ADR { get; set; }

        public string RKC { get; set; }

        public string NAMEP { get; set; }

        public string NAMEN { get; set; }

        public string NEWNUM { get; set; }

        public string NEWKS { get; set; }

        public string PERMFO { get; set; }

        public string SROK { get; set; }

        public string AT1 { get; set; }

        public string AT2 { get; set; }

        public string TELEF { get; set; }

        public string REGN { get; set; }

        public string OKPO { get; set; }

        public DateTime DT_IZM { get; set; }

        public string CKS { get; set; }

        public string KSNP { get; set; }

        public DateTime DATE_IN { get; set; }

        public DateTime DATE_CH { get; set; }

        public string VKEYDEL { get; set; }

        public DateTime DT_IZMR { get; set; }
    }
}
