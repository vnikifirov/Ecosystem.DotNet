namespace SwiftCode.Core.Interfaces.Repositories
{
    using SwiftCode.Core.Interfaces.Models.Request;
    using SwiftCode.Core.Models.Common;
    using SwiftCode.Core.Models.Response;
    using SwiftCode.Core.Persistence.Entities;
    using System.Threading.Tasks;

    public interface IBnkseekRepository : IRepository<BnkseekEntity>
    {
        Task<QueryResult<BnkseekEntity>> GetByQueryAsync(IQueryObject queryObj = null, bool includeReleted = false);
    }
}
