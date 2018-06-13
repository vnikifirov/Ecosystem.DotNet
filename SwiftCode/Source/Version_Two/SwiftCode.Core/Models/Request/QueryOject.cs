namespace SwiftCode.Core.Models.Request
{
    using SwiftCode.Core.Interfaces.Models.Request;

    public sealed class QueryOject : IQueryObject
    {
        #region Properties

        public string PZN { get; set; }
        public string NEWNUM { get; set; }
        public string REGN { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        #endregion
    }
}
