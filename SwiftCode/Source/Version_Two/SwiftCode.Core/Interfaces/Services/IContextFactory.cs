
namespace SwiftCode.Core.Interfaces.Services
{
    using SwiftCode.Core.Interfaces.Models.Common;
    using SwiftCode.Core.Interfaces.Repositories;

    public interface IContextFactory
    {
        IUnitOfWork GetUnitOfWork();
        IRepository<T> GetContextBasedOn<T>(IUnitOfWork unitOfWork) where T : BaseModel;
    }
}
