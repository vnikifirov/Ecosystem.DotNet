
namespace SwiftCode.Core.Persistence.Configs
{
    using SwiftCode.Core.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class PznEntityConfig : IEntityTypeConfiguration<PznEntity>
    {
        public void Configure(EntityTypeBuilder<PznEntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore(p => p.BnkseekEntitys);

            // Override Table Name
            builder.ToTable("PZN");

            // Make the default column type string
            builder.Property(p => p.VKEY).HasColumnType("char(8)");
            builder.Property(p => p.PZN).HasColumnType("char(2)");
            builder.Property(p => p.IMY);
            builder.Property(p => p.NAME);
            builder.Property(p => p.CB_DATE).HasColumnType("datetime2");
            builder.Property(p => p.CE_DATE).HasColumnType("datetime2");

            // Make the Primary key associated with property VKEY
            builder.HasKey(p => p.VKEY);
            builder.Property(p => p.VKEY).ValueGeneratedNever();

            // Make the Unique Key associated with property PZN
            builder.HasIndex(p => p.PZN).IsUnique();
        }
    }
}
