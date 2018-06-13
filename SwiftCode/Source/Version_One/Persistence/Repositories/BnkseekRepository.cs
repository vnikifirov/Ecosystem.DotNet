namespace bank_identification_code.Persistence.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using bank_identification_code.Core;
    using Microsoft.EntityFrameworkCore;
    using bank_identification_code.Core.Models;
    using bank_identification_code.Core.Interfaces;

    public class BnkseekRepository : IRepository<BNKSEEKEntity>
    {
        private readonly BNKSEEKDbContext context;

        public BnkseekRepository(BNKSEEKDbContext context)
        {
            this.context = context;
        }

        public async void AddAsync(BNKSEEKEntity model)
        {
            await context.BNKSEEKRecords.AddAsync(model);
        }

        public async void AddAsync(IEnumerable<BNKSEEKEntity> model)
        {
            await context.BNKSEEKRecords.AddRangeAsync(model);
        }

        public Task<List<BNKSEEKEntity>> GetAllAsync(bool includeReleted = false)
        {
            if (!includeReleted) return context.BNKSEEKRecords.ToListAsync();
            return context.BNKSEEKRecords.Include( b => b.PZNEntity )
                    .Include( b => b.REGEntity )
                    .Include( b => b.TNPEntity )
                    .Include( b => b.UEREntity )
                    .ToListAsync();
        }

        public Task<BNKSEEKEntity> GetByVKEYAsync(string VKEY, bool includeReleted = false)
        {
            if (!includeReleted) return context.BNKSEEKRecords.FirstOrDefaultAsync( b => b.VKEY.Trim() == VKEY.Trim());
            return context.BNKSEEKRecords
                    .Include( b => b.PZNEntity )
                    .Include( b => b.REGEntity )
                    .Include( b => b.TNPEntity )
                    .Include( b => b.UEREntity )
                    .FirstOrDefaultAsync( b => b.VKEY == VKEY);
        }

        public void Remove(BNKSEEKEntity model)
        {
            context.Remove(model);
        }

        public void Update(BNKSEEKEntity model)
        {
            context.Entry(model).State = EntityState.Modified;
        }
    }
}