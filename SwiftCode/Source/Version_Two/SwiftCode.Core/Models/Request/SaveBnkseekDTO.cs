
namespace SwiftCode.Core.Models.Request
{
    using System;
    using SwiftCode.Core.Interfaces.Models.Common;

    public sealed class SaveBnkseekDTO : BaseModel
    {
        #region Constructors

        public SaveBnkseekDTO(
            string vkey,
            string pzn,
            string uer,
            string ind,
            string tnp,
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
            PZN = pzn;
            UER = uer;
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

        #region Properties

        public string PZN { get; set; }
        public string UER { get; set; }
        public string RGN { get; set; }
        public string IND { get; set; }
        public string TNP { get; set; }
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
