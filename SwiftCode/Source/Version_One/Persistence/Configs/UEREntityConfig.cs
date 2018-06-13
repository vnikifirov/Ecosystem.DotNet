using bank_identification_code.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bank_identification_code.Persistence.Configs
{
  public class UEREntityConfig : IEntityTypeConfiguration<UEREntity>
    {
        public void Configure(EntityTypeBuilder<UEREntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore( u => u.BNKSEEKEntitys);

            // Override Table Name
            builder.ToTable("UER");

            // Make the default column type string
            builder.Property<string>(u => u.VKEY).HasColumnType("char(8)");
            builder.Property<string>(u => u.UER).HasColumnType("char(1)").IsRequired();
            builder.Property<string>(u => u.UERNAME).HasColumnType("char(80)");

            // Make the Primary key associated with property VKEY
            builder.HasKey( t => t.VKEY);
            builder.Property( p => p.VKEY).ValueGeneratedNever();

            // // Make the Unique Key associated with property UER
            builder.HasIndex(t => t.UER).IsUnique();
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