
namespace SwiftCode.Core.Persistence.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SwiftCode.Core.Interfaces.Repositories;
    using SwiftCode.Core.Persistence.Contexts;
    using SwiftCode.Core.Persistence.Entities;

    public sealed class UerRepository : IUerRepository
    {
        private readonly BnkseekDbContext _context;

        public UerRepository(BnkseekDbContext context)
        {
            _context = context;
        }

        public async void AddAsync(IEnumerable<UerEntity> entities)
        {
            await _context.UerRecords.AddRangeAsync(entities);
        }

        public async void AddAsync(UerEntity entity)
        {
            await _context.UerRecords.AddAsync(entity);
        }

        public async Task<IEnumerable<UerEntity>> GetAllAsync(bool includeReleted = false)
        {
            if (!includeReleted) return await _context.UerRecords.ToListAsync();
            return await _context.UerRecords
                    .Include(u => u.BnkseekEntitys)
                    .ToListAsync();
        }

        public Task<UerEntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return _context.UerRecords.FirstOrDefaultAsync(u => u.VKEY == VKEY);
            return _context.UerRecords
                    .Include(u => u.BnkseekEntitys)
                    .FirstOrDefaultAsync(u => u.VKEY == VKEY);
        }

        public void Remove(UerEntity entity)
        {
            _context.Remove(entity);
        }

        public void Update(UerEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
