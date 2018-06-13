
namespace SwiftCode.Core.Models.Common
{
    using System;
    using SwiftCode.Core.Interfaces.Models.Common;

    public sealed class BnkseekDTO : BaseModel
    {
        #region Constructors

        public BnkseekDTO(
            string vkey,
            string real,
            PznDTO pzn,
            UerDTO uer,
            RegDTO reg,
            string ind,
            TnpDTO tnp,
            string nnp,
            string adr,
            string rkc,
            string namep,
            string newnum,
            string telef,
            string regn,
            string okpo,
            DateTime dtizm,
            string ksnp,
            DateTime datein,
            DateTime datech)             
        {
            VKEY = vkey;
            REAL = real;
            PZN = pzn;
            UER = uer;
            REG = reg;
            IND = ind;
            TNP = tnp;
            NNP = nnp;
            ADR = adr;
            RKC = rkc;
            NAMEP = namep;
            NEWNUM = newnum;
            TELEF = telef;
            REGN = regn;
            OKPO = okpo;
            DTIZM = dtizm;
            KSNP = ksnp;
            DATEIN = datein;
            DATECH = datech;
        }

        #endregion

        #region Fields

        private string _real;        

        #endregion

        #region Properties

        public string REAL
        {
            get => string.IsNullOrWhiteSpace(_real) ? null : _real;
            private set
            {
                _real = value;
            }
        }        

        public PznDTO PZN { get; set; }

        public UerDTO UER { get; set; }

        public RegDTO REG { get; set; }

        public string IND { get; set; }

        public TnpDTO TNP { get; set; }

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

        #endregion
    }
}
