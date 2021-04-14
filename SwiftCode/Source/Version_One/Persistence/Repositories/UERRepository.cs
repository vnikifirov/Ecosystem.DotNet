namespace bank_identification_code.Persistence.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using bank_identification_code.Core.Models;
    using Microsoft.EntityFrameworkCore;
    using bank_identification_code.Core.Interfaces;
    public class UERRepository : IRepository<UEREntity>
    {
        private readonly BNKSEEKDbContext context;

        public UERRepository(BNKSEEKDbContext context)
        {
            this.context = context;
        }

        public async void AddAsync(UEREntity model)
        {
            await context.UERRecords.AddAsync(model);
        }

        public async void AddAsync(IEnumerable<UEREntity> model)
        {
            await context.UERRecords.AddRangeAsync(model);
        }

        public Task<List<UEREntity>> GetAllAsync(bool includeReleted = false)
        {
            if (!includeReleted) return context.UERRecords.ToListAsync();
            return context.UERRecords
                    .Include( u => u.BNKSEEKEntitys)
                    .ToListAsync();
        }

        public Task<UEREntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return context.UERRecords.FirstOrDefaultAsync( u => u.VKEY == VKEY);
            return context.UERRecords
                    .Include( u => u.BNKSEEKEntitys)
                    .FirstOrDefaultAsync( u => u.VKEY == VKEY);
        }

        public void Remove(UEREntity model)
        {
            context.Remove(model);
        }

        public void Update(UEREntity model)
        {
            context.Entry(model).State = EntityState.Modified;
        }
    }
}