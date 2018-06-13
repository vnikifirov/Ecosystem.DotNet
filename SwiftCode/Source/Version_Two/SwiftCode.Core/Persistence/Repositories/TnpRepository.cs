
namespace SwiftCode.Core.Persistence.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SwiftCode.Core.Interfaces.Repositories;
    using SwiftCode.Core.Persistence.Contexts;
    using SwiftCode.Core.Persistence.Entities;

    public sealed class TnpRepository : ITnpRepository
    {
        private readonly BnkseekDbContext _context;

        public TnpRepository(BnkseekDbContext context)
        {
            _context = context;
        }

        public async void AddAsync(TnpEntity entity)
        {
            await _context.TnpRecords.AddAsync(entity);
        }

        public async void AddAsync(IEnumerable<TnpEntity> entities)
        {
            await _context.TnpRecords.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<TnpEntity>> GetAllAsync(bool includeReleted = false)
        {
            if (!includeReleted) return await _context.TnpRecords.ToListAsync();
            return await _context.TnpRecords
                    .Include(t => t.BnkseekEntitys)
                    .ToListAsync();
        }

        public Task<TnpEntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return _context.TnpRecords.FirstOrDefaultAsync(t => t.VKEY == VKEY);
            return _context.TnpRecords
                    .Include(t => t.BnkseekEntitys)
                    .FirstOrDefaultAsync(t => t.VKEY == VKEY);
        }

        public void Remove(TnpEntity entity)
        {
            _context.Remove(entity);
        }

        public void Update(TnpEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
