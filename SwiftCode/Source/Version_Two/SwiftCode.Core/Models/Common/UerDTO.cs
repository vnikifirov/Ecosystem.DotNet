namespace SwiftCode.Core.Models.Common
{
    public sealed class UerDTO 
    {
        #region Contructors

        public UerDTO(
            string uer,
            string uername)
        {
            UER = uer;
            UERNAME = uername;
        }

        #endregion

        #region Properties

        public string UER { get; set; }
        public string UERNAME { get; set; }

        #endregion
    }
}
