
namespace SwiftCode.Core.Persistence.Configs
{
    using SwiftCode.Core.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class TnpEntityConfig : IEntityTypeConfiguration<TnpEntity>
    {
        public void Configure(EntityTypeBuilder<TnpEntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore(t => t.BnkseekEntitys);

            // Override Table Name
            builder.ToTable("TNP");

            // Make the default column type string
            builder.Property(t => t.VKEY).HasColumnType("char(8)");
            builder.Property(t => t.TNP).HasColumnType("char(1)").IsRequired(false);
            builder.Property(t => t.FULLNAME).HasColumnType("char(45)");
            builder.Property(t => t.SHORTNAME).HasColumnType("char(16)");

            // Make the Primary key associated with property VKEY
            builder.HasKey(t => t.VKEY);
            builder.Property(t => t.VKEY).ValueGeneratedNever();

            // // Make the Unique Key associated with property TNP
            builder.HasIndex(t => t.TNP).IsUnique();
        }
    }
}
