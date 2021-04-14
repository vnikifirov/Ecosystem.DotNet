namespace bank_identification_code.Persistence.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using bank_identification_code.Core.Models;
    using bank_identification_code.Core.Interfaces;
    public class REGRepository : IRepository<REGEntity>
    {
        private readonly BNKSEEKDbContext context;

        public REGRepository(BNKSEEKDbContext context)
        {
            this.context = context;
        }

        public async void AddAsync(REGEntity model)
        {
            await context.REGRecords.AddAsync(model);
        }

        public async void AddAsync(IEnumerable<REGEntity> model)
        {
            await context.REGRecords.AddRangeAsync(model);
        }

        public Task<List<REGEntity>> GetAllAsync(bool includeReleted = false)
        {
            if (!includeReleted) return context.REGRecords.ToListAsync();
            return context.REGRecords
                    .Include( r => r.BNKSEEKEntitys)
                    .ToListAsync();
        }

        public Task<REGEntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return context.REGRecords.FirstOrDefaultAsync( b => b.VKEY == VKEY);
            return context.REGRecords
                    .Include( r => r.BNKSEEKEntitys)
                    .FirstOrDefaultAsync( b => b.VKEY == VKEY);
        }

        public void Remove(REGEntity model)
        {
            context.Remove(model);
        }

        public void Update(REGEntity model)
        {
            context.Entry(model).State = EntityState.Modified;
        }
    }
}