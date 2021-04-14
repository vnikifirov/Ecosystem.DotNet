namespace bank_identification_code.Persistence.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using bank_identification_code.Core.Models;
    using Microsoft.EntityFrameworkCore;
    using bank_identification_code.Core.Interfaces;
    public class TNPRepository : IRepository<TNPEntity>
    {
        private readonly BNKSEEKDbContext context;

        public TNPRepository(BNKSEEKDbContext context)
        {
            this.context = context;
        }

        public async void AddAsync(TNPEntity model)
        {
            await context.TNPRecords.AddAsync(model);
        }

        public async void AddAsync(IEnumerable<TNPEntity> model)
        {
            await context.TNPRecords.AddRangeAsync(model);
        }

        public Task<List<TNPEntity>> GetAllAsync(bool includeReleted = false)
        {
            if (!includeReleted) return context.TNPRecords.ToListAsync();
            return context.TNPRecords
                    .Include( t => t.BNKSEEKEntitys)
                    .ToListAsync();
        }

        public Task<TNPEntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return context.TNPRecords.FirstOrDefaultAsync( t => t.VKEY == VKEY);
            return context.TNPRecords
                    .Include( t => t.BNKSEEKEntitys)
                    .FirstOrDefaultAsync( t => t.VKEY == VKEY);
        }

        public void Remove(TNPEntity model)
        {
            context.Remove(model);
        }

        public void Update(TNPEntity model)
        {
            context.Entry(model).State = EntityState.Modified;
        }
    }
}