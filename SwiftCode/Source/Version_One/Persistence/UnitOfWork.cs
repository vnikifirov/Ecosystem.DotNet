using System.Threading.Tasks;
using bank_identification_code.Core.Interfaces;
using bank_identification_code.Core.Models;
using bank_identification_code.Persistence.Repositories;

namespace bank_identification_code.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BNKSEEKDbContext context;

        public UnitOfWork(BNKSEEKDbContext _context)
        {
            this.context = _context;
        }

        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}