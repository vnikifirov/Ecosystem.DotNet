
namespace SwiftCode.Core.Persistence.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using SwiftCode.Core.Persistence.Contexts;
    using System.Collections.Generic;
    using SwiftCode.Core.Models.Response;
    using SwiftCode.Core.Interfaces.Repositories;
    using SwiftCode.Core.Interfaces.Models.Request;
    using SwiftCode.Core.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using SwiftCode.Core.Extentions;

    public sealed class BnkseekRepository : IBnkseekRepository
    {
        private readonly BnkseekDbContext _context;

        public BnkseekRepository(BnkseekDbContext context)
        {
            _context = context;
        }

        public async void AddAsync(BnkseekEntity entity)
        {
            await _context.BnkseekRecords.AddAsync(entity);
        }

        public async void AddAsync(IEnumerable<BnkseekEntity> entities)
        {
            await _context.BnkseekRecords.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<BnkseekEntity>> GetAllAsync(bool includeReleted = false)
        {
            IQueryable<BnkseekEntity> query = null;
            if (!includeReleted) query = _context.BnkseekRecords;
            else
            {
                // ? If it's true then returning and the related data, also.
                query = _context.BnkseekRecords
                    .Include(b => b.PznEntity)
                    .Include(b => b.RegEntity)
                    .Include(b => b.TnpEntity)
                    .Include(b => b.UerEntity);
            }

            return await query.ToListAsync();
        }

        public async Task<QueryResult<BnkseekEntity>> GetByQueryAsync(IQueryObject queryObj = null, bool includeReleted = false)
        {
            IQueryable<BnkseekEntity> query = null;
            if (!includeReleted) query = _context.BnkseekRecords;
            else
            {
                // ? If it's true then returning and the related data, also.
                query = _context.BnkseekRecords
                    .Include(b => b.PznEntity)
                    .Include(b => b.RegEntity)
                    .Include(b => b.TnpEntity)
                    .Include(b => b.UerEntity);
            }

            // ? Filering out by specified filters in the query object
            if (queryObj != null && !string.IsNullOrWhiteSpace(queryObj.NEWNUM))
                query = query.Where(b => b.NEWNUM.ToUpper().Contains(queryObj.NEWNUM.ToUpper()));

            if (queryObj != null && !string.IsNullOrWhiteSpace(queryObj.REGN))
                query = query.Where(b => b.REGN.ToUpper().Contains(queryObj.REGN.ToUpper()));

            if (queryObj != null && !string.IsNullOrWhiteSpace(queryObj.PZN))
                query = query.Where(b => b.PZN.ToUpper() == queryObj.PZN.ToUpper());

            return new QueryResult<BnkseekEntity>
            {
                TotalItems = await query.CountAsync(),
                Items = query.ApplyPaging(queryObj)
            };
        }

        public Task<BnkseekEntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return _context.BnkseekRecords.FirstOrDefaultAsync(b => b.VKEY.Trim() == VKEY.Trim());
            return _context.BnkseekRecords
                    .Include(b => b.PznEntity)
                    .Include(b => b.RegEntity)
                    .Include(b => b.TnpEntity)
                    .Include(b => b.UerEntity)
                    .FirstOrDefaultAsync(b => b.VKEY == VKEY);
        }

        public void Remove(BnkseekEntity entity)
        {
            _context.Remove(entity);
        }

        public void Update(BnkseekEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
