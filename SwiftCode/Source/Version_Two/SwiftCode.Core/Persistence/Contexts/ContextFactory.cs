
namespace SwiftCode.Core.Persistence.Contexts
{
    using Microsoft.EntityFrameworkCore;
    using SwiftCode.Core.Interfaces.Models.Common;
    using SwiftCode.Core.Interfaces.Repositories;
    using SwiftCode.Core.Interfaces.Services;
    using SwiftCode.Core.Persistence.Entities;
    using SwiftCode.Core.Persistence.Repositories;

    public sealed class ContextFactory : IContextFactory
    {
        private  string _connectionString;

        public ContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IUnitOfWork GetUnitOfWork()
        {
            // Create a new context
            var context = new BnkseekDbContext(
                new DbContextOptionsBuilder<BnkseekDbContext>()
                    .UseSqlServer(_connectionString).Options);

            // Wrap a new context in IUnitOfWork
            return new UnitOfWork(context);
        }

        public IRepository<T> GetContextBasedOn<T>(IUnitOfWork unitOfWork)
            where T : BaseModel
        {
            // ? 1st Determinate a data type of T
            // ? 2nd If type is supported, just return context based on type
            if (typeof(T) == typeof(BnkseekEntity)) return (IRepository<T>)unitOfWork.Bnkseek;
            if (typeof(T) == typeof(PznEntity)) return (IRepository<T>)unitOfWork.PZN;
            if (typeof(T) == typeof(RegEntity)) return (IRepository<T>)unitOfWork.REG;
            if (typeof(T) == typeof(TnpEntity)) return (IRepository<T>)unitOfWork.TNP;
            if (typeof(T) == typeof(UerEntity)) return (IRepository<T>)unitOfWork.UER;

            // TODO More supported DataTypes ...

            return null;
        }
    }
}
