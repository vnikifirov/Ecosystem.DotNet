
namespace SwiftCode.Core.Persistence.Contexts
{
    using SwiftCode.Core.Persistence.Entities;
    using SwiftCode.Core.Persistence.Configs;
    using Microsoft.EntityFrameworkCore;

    public sealed class BnkseekDbContext : DbContext
    {
        public BnkseekDbContext(DbContextOptions<BnkseekDbContext> options)
            : base(options)
        {
        }

        public DbSet<BnkseekEntity> BnkseekRecords { get; set; }
        public DbSet<PznEntity> PznRecords { get; set; }
        public DbSet<RegEntity> RegRecords { get; set; }
        public DbSet<TnpEntity> TnpRecords { get; set; }
        public DbSet<UerEntity> UerRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ? NOTE: A better is using fluent API to create tables instread of DataAnatations
            modelBuilder.HasDefaultSchema("dbo");

            // ? Create a Pzn Table
            modelBuilder.ApplyConfiguration(new PznEntityConfig());
            // ? Create a Reg Table
            modelBuilder.ApplyConfiguration(new RegEntityConfig());
            // ? Create a Tnp Table
            modelBuilder.ApplyConfiguration(new TnpEntityConfig());
            // ? Create a Uer Table
            modelBuilder.ApplyConfiguration(new UerEntityConfig());
            // ? Create a Bnkseek Table
            modelBuilder.ApplyConfiguration(new BnkseekEntityConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}
