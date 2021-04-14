using bank_identification_code.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bank_identification_code.Persistence.Configs
{
    public class TNPEntityConfig : IEntityTypeConfiguration<TNPEntity>
    {
        public void Configure(EntityTypeBuilder<TNPEntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore( t => t.BNKSEEKEntitys);

            // Override Table Name
            builder.ToTable("TNP");

            // Make the default column type string
            builder.Property<string>(t => t.VKEY).HasColumnType("char(8)");
            builder.Property<string>(t => t.TNP).HasColumnType("char(1)").IsRequired(false);
            builder.Property<string>(t => t.FULLNAME).HasColumnType("char(45)");
            builder.Property<string>(t => t.SHORTNAME).HasColumnType("char(16)");

            // Make the Primary key associated with property VKEY
            builder.HasKey( t => t.VKEY);
            builder.Property( t => t.VKEY).ValueGeneratedNever();

            // // Make the Unique Key associated with property TNP
            builder.HasIndex(t => t.TNP).IsUnique();
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