
namespace SwiftCode.Core.Persistence.Configs
{
    using SwiftCode.Core.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class UerEntityConfig : IEntityTypeConfiguration<UerEntity>
    {
        public void Configure(EntityTypeBuilder<UerEntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore(u => u.BnkseekEntitys);

            // Override Table Name
            builder.ToTable("UER");

            // Make the default column type string
            builder.Property(u => u.VKEY).HasColumnType("char(8)");
            builder.Property(u => u.UER).HasColumnType("char(1)").IsRequired();
            builder.Property(u => u.UERNAME).HasColumnType("char(80)");

            // Make the Primary key associated with property VKEY
            builder.HasKey(t => t.VKEY);
            builder.Property(p => p.VKEY).ValueGeneratedNever();

            // // Make the Unique Key associated with property UER
            builder.HasIndex(t => t.UER).IsUnique();
        }
    }
}
