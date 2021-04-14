
namespace SwiftCode.Core.Persistence.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SwiftCode.Core.Interfaces.Repositories;
    using SwiftCode.Core.Persistence.Contexts;
    using SwiftCode.Core.Persistence.Entities;

    public sealed class PznRepository : IPznRepository
    {
        private readonly BnkseekDbContext _context;

        public PznRepository(BnkseekDbContext context)
        {
            _context = context;
        }

        public async void AddAsync(PznEntity entity)
        {
            await _context.PznRecords.AddAsync(entity);
        }

        public async void AddAsync(IEnumerable<PznEntity> entities)
        {
            await _context.PznRecords.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<PznEntity>> GetAllAsync(bool includeReleted = false)
        {
            if (!includeReleted) return await _context.PznRecords.ToListAsync();
            return await _context.PznRecords
                    .Include(p => p.BnkseekEntitys)
                    .ToListAsync();
        }

        public Task<PznEntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return _context.PznRecords.FirstOrDefaultAsync(p => p.VKEY == VKEY);
            return _context.PznRecords
                    .Include(p => p.BnkseekEntitys)
                    .FirstOrDefaultAsync(p => p.VKEY == VKEY);
        }

        public void Remove(PznEntity entity)
        {
            _context.Remove(entity);
        }

        public void Update(PznEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
