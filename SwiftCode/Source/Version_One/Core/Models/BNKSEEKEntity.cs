namespace bank_identification_code.Core.Models
{
    using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class BNKSEEKEntity
    {
        private string _real;
        private string _pzn;
        private string _tnp;

        public string VKEY { get; set; }

        public string REAL
        {
            get => string.IsNullOrWhiteSpace(_real) ? null : _real;
            set
            {
                _real = value;
            }
        }
        // public REALEntity REALEntity { get; set; }

        public string PZN
        {
            get => string.IsNullOrWhiteSpace(_pzn) ? null : _pzn;
            set
            {
                _pzn = value;
            }
        }

        public PZNEntity PZNEntity { get; set; }

        public string UER { get; set; }

        public UEREntity UEREntity { get; set; }

        public string RGN { get; set; }

        public REGEntity REGEntity { get; set; }

        public string IND { get; set; }

        public string TNP
        {
            get => string.IsNullOrWhiteSpace(_tnp) ? null : _tnp;
            set
            {
                _tnp = value;
            }
        }

        public TNPEntity TNPEntity { get; set; }

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