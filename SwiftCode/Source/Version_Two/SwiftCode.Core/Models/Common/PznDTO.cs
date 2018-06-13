namespace SwiftCode.Core.Models.Common
{
    public sealed class PznDTO 
    {
        #region Contructors

        public PznDTO(
            string imy,
            string name,
            string pzn)
        {
            IMY = imy;
            NAME = name;
            PZN = pzn;
        }

        #endregion

        #region Fields

        private string _pzn;        

        #endregion

        #region Properties

        public string IMY { get; set; }
        public string NAME { get; set; }

        public string PZN
        {
            get => string.IsNullOrWhiteSpace(_pzn) ? null : _pzn;
            private set
            {
                _pzn = value;
            }
        }

        #endregion
    }
}
