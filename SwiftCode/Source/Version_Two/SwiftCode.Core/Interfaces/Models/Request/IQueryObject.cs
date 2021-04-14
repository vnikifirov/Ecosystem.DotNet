namespace SwiftCode.Core.Interfaces.Models.Request
{
    public interface IQueryObject
    {
        string PZN { get; set; }
        string NEWNUM { get; set; }
        string REGN { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }
}
