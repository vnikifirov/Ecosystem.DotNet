
namespace SwiftCode.Core.Interfaces.Repositories
{
    using System;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        IBnkseekRepository Bnkseek { get; }
        IPznRepository PZN { get; }
        IRegRepository REG { get; }
        ITnpRepository TNP { get; }
        IUerRepository UER { get; }
        Task CompleteAsync();
    }
}
