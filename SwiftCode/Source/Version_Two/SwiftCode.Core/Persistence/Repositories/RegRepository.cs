
namespace SwiftCode.Core.Persistence.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SwiftCode.Core.Interfaces.Repositories;
    using SwiftCode.Core.Persistence.Contexts;
    using SwiftCode.Core.Persistence.Entities;

    public sealed class RegRepository : IRegRepository
    {
        private readonly BnkseekDbContext _context;

        public RegRepository(BnkseekDbContext context)
        {
            _context = context;
        }

        public async void AddAsync(RegEntity entity)
        {
            await _context.RegRecords.AddAsync(entity);
        }

        public async void AddAsync(IEnumerable<RegEntity> entities)
        {
            await _context.RegRecords.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<RegEntity>> GetAllAsync(bool includeReleted = false)
        {
            if (!includeReleted) return await _context.RegRecords.ToListAsync();
            return await _context.RegRecords
                    .Include(r => r.BnkseekEntitys)
                    .ToListAsync();
        }

        public Task<RegEntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return _context.RegRecords.FirstOrDefaultAsync(b => b.VKEY == VKEY);
            return _context.RegRecords
                    .Include(r => r.BnkseekEntitys)
                    .FirstOrDefaultAsync(b => b.VKEY == VKEY);
        }

        public void Remove(RegEntity entity)
        {
            _context.Remove(entity);
        }

        public void Update(RegEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
