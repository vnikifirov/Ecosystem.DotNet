using Microsoft.EntityFrameworkCore;
using bank_identification_code.Core.Models;
using bank_identification_code.Persistence.Configs;

namespace bank_identification_code.Persistence
{
    public class BNKSEEKDbContext : DbContext
    {
        public BNKSEEKDbContext(DbContextOptions<BNKSEEKDbContext> options)
            : base(options)
        {
        }

        public DbSet<BNKSEEKEntity> BNKSEEKRecords { get; set; }
        public DbSet<PZNEntity> PZNRecords { get; set; }
        public DbSet<REGEntity> REGRecords { get; set; }
        public DbSet<TNPEntity> TNPRecords { get; set; }
        public DbSet<UEREntity> UERRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // NOTE: A better is using fluent API to create tables instread of DataAnatations
            modelBuilder.HasDefaultSchema("dbo");

            // Create a PZN Table
            modelBuilder.ApplyConfiguration(new PZNEntityConfig());
            // Create a REG Table
            modelBuilder.ApplyConfiguration(new REGEntityConfig());
            // // Create a TNP Table
            modelBuilder.ApplyConfiguration(new TNPEntityConfig());
            // // Create a UER Table
            modelBuilder.ApplyConfiguration(new UEREntityConfig());
            // // Create a BNK TableSEEK
            modelBuilder.ApplyConfiguration(new BNKSEEKEntityConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}
