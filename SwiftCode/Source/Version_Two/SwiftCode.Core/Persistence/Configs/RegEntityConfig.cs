
namespace SwiftCode.Core.Persistence.Configs
{
    using SwiftCode.Core.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class RegEntityConfig : IEntityTypeConfiguration<RegEntity>
    {
        public void Configure(EntityTypeBuilder<RegEntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore(r => r.BnkseekEntitys);

            // Override Table Name
            builder.ToTable("REG");

            // Make the default column type string
            builder.Property(r => r.VKEY).HasColumnType("char(8)");
            builder.Property(r => r.RGN).HasColumnType("char(2)").IsRequired();
            builder.Property(r => r.CENTER).HasColumnType("char(45)");
            builder.Property(r => r.NAME).HasColumnType("char(45)");
            builder.Property(r => r.NAMET).HasColumnType("char(45)");

            // Make the Primary key associated with property VKEY
            builder.HasKey(r => r.VKEY);
            builder.Property(p => p.VKEY).ValueGeneratedNever();

            // Make the Unique Key associated with property RGN
            builder.HasIndex(r => r.RGN).IsUnique();
        }
    }
}
