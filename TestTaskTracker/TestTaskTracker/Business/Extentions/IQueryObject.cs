using System;
namespace Business.Extentions
{
    public interface IQueryObject
    {
        public bool IsSortAcsending { get; set; }
        public string SortBy { get; set; }
    }
}
