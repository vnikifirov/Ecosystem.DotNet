namespace bank_identification_code.Controllers.Resources
{
    using System;

    public class BNKSEEKResource
    {
        public string VKEY { get; set; }

        public string REAL { get; set; }

        public PZNResource PZN { get; set; }

        public UERResource UER { get; set; }

        public REGResource REG { get; set; }

        public string IND { get; set; }

        public TNPResource TNP { get; set; }

        public string NNP { get; set; }

        public string ADR { get; set; }

        public string RKC { get; set; }

        public string NAMEP { get; set; }

        public string NEWNUM { get; set; }

        public string TELEF { get; set; }

        public string REGN { get; set; }

        public string OKPO { get; set; }

        public DateTime DTIZM { get; set; }

        public string KSNP { get; set; }

        public DateTime DATEIN { get; set; }

        public DateTime DATECH { get; set; }
    }
}