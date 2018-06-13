using bank_identification_code.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bank_identification_code.Persistence.Configs
{
    public class REGEntityConfig : IEntityTypeConfiguration<REGEntity>
    {
        public void Configure(EntityTypeBuilder<REGEntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore( r => r.BNKSEEKEntitys);

            // Override Table Name
            builder.ToTable("REG");

            // Make the default column type string
            builder.Property<string>(r => r.VKEY).HasColumnType("char(8)");
            builder.Property<string>(r => r.RGN).HasColumnType("char(2)").IsRequired();
            builder.Property<string>(r => r.CENTER).HasColumnType("char(45)");
            builder.Property<string>(r => r.NAME).HasColumnType("char(45)");
            builder.Property<string>(r => r.NAMET).HasColumnType("char(45)");

            // Make the Primary key associated with property VKEY
            builder.HasKey( r => r.VKEY);
            builder.Property( p => p.VKEY).ValueGeneratedNever();

            // Make the Unique Key associated with property RGN
            builder.HasIndex(r => r.RGN).IsUnique();
        }
    }
}


/**

        public string RGN { get; set; }

        public ICollection<BNKSEEKEntity> BNKSEEKEntity { get; set; }

        public string CENTER { get; set; }

        public string NAME { get; set; }

        public string NAMET { get; set; }


 */