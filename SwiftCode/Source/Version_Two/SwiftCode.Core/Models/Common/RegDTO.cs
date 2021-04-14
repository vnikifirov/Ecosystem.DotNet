namespace SwiftCode.Core.Models.Common
{
    public sealed class RegDTO 
    {       
        #region Contructors

        public RegDTO(
            string center,
            string name,
            string namet,
            string rgn)
        {
            CENTER = center;
            NAME = name;
            NAMET = namet;
            RGN = rgn;
        }

        #endregion

        #region Properties

        public string CENTER { get; set; }
        public string NAME { get; set; }
        public string NAMET { get; set; }
        public string RGN { get; private set; }

        #endregion
    }
}
