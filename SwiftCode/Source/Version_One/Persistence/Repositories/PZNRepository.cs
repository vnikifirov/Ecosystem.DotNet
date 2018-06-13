namespace bank_identification_code.Persistence.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using bank_identification_code.Core.Models;
    using bank_identification_code.Core.Interfaces;
    public class PZNRepository : IRepository<PZNEntity>
    {
        private readonly BNKSEEKDbContext context;

        public PZNRepository(BNKSEEKDbContext context)
        {
            this.context = context;
        }

        public async void AddAsync(PZNEntity model)
        {
            await context.PZNRecords.AddAsync(model);
        }

        public async void AddAsync(IEnumerable<PZNEntity> model)
        {
            await context.PZNRecords.AddRangeAsync(model);
        }

        public Task<List<PZNEntity>> GetAllAsync(bool includeReleted = false)
        {
            if (!includeReleted) return context.PZNRecords.ToListAsync();
            return context.PZNRecords
                    .Include( p => p.BNKSEEKEntitys)
                    .ToListAsync();
        }

        public Task<PZNEntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return context.PZNRecords.FirstOrDefaultAsync( p => p.VKEY == VKEY);
            return context.PZNRecords
                    .Include( p => p.BNKSEEKEntitys)
                    .FirstOrDefaultAsync( p => p.VKEY == VKEY);
        }

        public void Remove(PZNEntity model)
        {
            context.Remove(model);
        }

        public void Update(PZNEntity model)
        {
            context.Entry(model).State = EntityState.Modified;
        }
    }
}