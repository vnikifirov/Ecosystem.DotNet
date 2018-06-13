
namespace SwiftCode.Core.Persistence.Repositories
{
    using System;
    using System.Threading.Tasks;
    using SwiftCode.Core.Interfaces.Repositories;
    using SwiftCode.Core.Persistence.Contexts;

    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BnkseekDbContext _context;

        public UnitOfWork(BnkseekDbContext _context)
        {
            this._context = _context;

            Bnkseek = new BnkseekRepository(_context);
            PZN = new PznRepository(_context);
            REG = new RegRepository(_context);
            TNP = new TnpRepository(_context);
            UER = new UerRepository(_context);
        }

        public IBnkseekRepository Bnkseek { get; private set; }

        public IPznRepository PZN { get; private set; }

        public IRegRepository REG { get; private set; }

        public ITnpRepository TNP { get; private set; }

        public IUerRepository UER { get; private set; }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
