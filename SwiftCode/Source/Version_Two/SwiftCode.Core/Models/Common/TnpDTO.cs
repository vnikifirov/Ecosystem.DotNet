namespace SwiftCode.Core.Models.Common
{
    public sealed class TnpDTO 
    {
        #region Fields

        private string _tnp;

        #endregion

        #region Contructors

        public TnpDTO()
        {
        }

        public TnpDTO(
            string fullname,
            string shorname,
            string tnp)
        {
            FULLNAME = fullname;
            SHORTNAME = shorname;
            TNP = tnp;
        }

        #endregion

        #region Properties

        public string FULLNAME { get; set; }
        public string SHORTNAME { get; set; }
        public string TNP
        {
            get => string.IsNullOrWhiteSpace(_tnp) ? null : _tnp;
            private set
            {
                _tnp = value;
            }
        }

        #endregion
    }
}
