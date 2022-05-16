using System;
using Business.Extentions;
using Context.Models;

namespace Business.Services.Domain.Responses
{
    public class ProjectQuery : IQueryObject
    {
        // Sorting
        public bool IsSortAcsending { get; set; }
        public string SortBy { get; set; }

        // Range or paging
        public int PageSize { get; set; }
        public int Page { get; set; }

        // Filtering by Exact value
        public string Name { get; set; }
        public int? Priority { get; internal set; }
        public int? ProjectId { get; set; }
        public ProjectStatus? Status { get; set; }
    }
}
