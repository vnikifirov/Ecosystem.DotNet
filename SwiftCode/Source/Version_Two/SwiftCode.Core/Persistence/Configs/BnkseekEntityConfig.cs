
namespace SwiftCode.Core.Persistence.Configs
{
    using SwiftCode.Core.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class BnkseekEntityConfig : IEntityTypeConfiguration<BnkseekEntity>
    {
        public void Configure(EntityTypeBuilder<BnkseekEntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore(b => b.PznEntity);
            builder.Ignore(b => b.RegEntity);
            builder.Ignore(b => b.TnpEntity);
            builder.Ignore(b => b.UerEntity);

            // Override Table Name
            builder.ToTable("BNKSEEK");

            // Make the default column as a string type. Also, they're keys
            builder.Property(b => b.VKEY).HasColumnType("char(8)");
            builder.Property(b => b.REAL).HasColumnType("char(4)");
            builder.Property(b => b.PZN).HasColumnType("char(2)");
            builder.Property(b => b.UER).HasColumnType("char(1)");
            builder.Property(b => b.RGN).HasColumnType("char(2)");
            builder.Property(b => b.TNP).HasColumnType("char(1)");
            builder.Property(b => b.VKEYDEL).HasColumnType("char(8)");

            // Specify a column names
            builder.Property(b => b.IND).HasColumnType("char(6)");
            builder.Property(b => b.NNP).HasColumnType("char(25)");
            builder.Property(b => b.ADR).HasColumnType("char(30)");
            builder.Property(b => b.RKC).HasColumnType("char(9)");
            builder.Property(b => b.NAMEP).HasColumnType("char(45)");
            builder.Property(b => b.NAMEN).HasColumnType("char(30)");
            builder.Property(b => b.NEWNUM).HasColumnType("char(9)");
            builder.Property(b => b.NEWKS).HasColumnType("char(9)");
            builder.Property(b => b.PERMFO).HasColumnType("char(6)");
            builder.Property(b => b.SROK).HasColumnType("char(2)");
            builder.Property(b => b.AT1).HasColumnType("char(7)");
            builder.Property(b => b.AT2).HasColumnType("char(7)");
            builder.Property(b => b.TELEF).HasColumnType("char(25)");
            builder.Property(b => b.REGN).HasColumnType("char(9)");
            builder.Property(b => b.OKPO).HasColumnType("char(8)");
            builder.Property(b => b.DT_IZM).HasColumnType("datetime2");
            builder.Property(b => b.CKS).HasColumnType("char(6)");
            builder.Property(b => b.KSNP).HasColumnType("char(20)");
            builder.Property(b => b.DATE_IN).HasColumnType("datetime2");
            builder.Property(b => b.DATE_CH).HasColumnType("datetime2");
            builder.Property(b => b.VKEYDEL).HasColumnType("char(9)");
            builder.Property(b => b.DT_IZMR).HasColumnType("datetime2");

            // Make NOT NULL constraints associated with properties
            builder.Property(b => b.UER).IsRequired();
            builder.Property(b => b.RGN).IsRequired();
            builder.Property(b => b.NAMEP).IsRequired();
            builder.Property(b => b.NAMEN).IsRequired();
            builder.Property(b => b.NEWNUM).IsRequired();
            builder.Property(b => b.SROK).IsRequired();
            builder.Property(b => b.DT_IZM).IsRequired();
            builder.Property(b => b.DATE_IN).IsRequired();

            // Make the Primary key associated with property VKEY
            builder.HasKey(b => b.VKEY);
            builder.Property(b => b.VKEY).ValueGeneratedNever();

            // Make the Unique Key associated with property RGN
            builder.HasIndex(b => b.NEWNUM)
                   .IsUnique();

            // Allow to contain duplicate values
            builder.HasIndex(b => b.PZN)
                   .IsUnique(false);

            builder.HasIndex(b => b.UER)
                   .IsUnique(false);

            builder.HasIndex(b => b.RGN)
                   .IsUnique(false);

            builder.HasIndex(b => b.TNP)
                   .IsUnique(false);

            builder.HasIndex(b => b.UER)
                   .IsUnique(false);

            builder.HasIndex(b => b.VKEYDEL)
                   .IsUnique(false);

            builder.HasOne(b => b.PznEntity)
                .WithMany(p => p.BnkseekEntitys)
                .HasPrincipalKey(p => p.PZN)
                .HasForeignKey(b => b.PZN)
                .HasConstraintName("FK_BNKSEEK_PZN")
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(b => b.RegEntity)
                .WithMany(r => r.BnkseekEntitys)
                .HasPrincipalKey(r => r.RGN)
                .HasForeignKey(b => b.RGN)
                .HasConstraintName("FK_BNKSEEK_REG")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(b => b.TnpEntity)
                .WithMany(t => t.BnkseekEntitys)
                .HasPrincipalKey(t => t.TNP)
                .HasForeignKey(b => b.TNP)
                .HasConstraintName("FK_BNKSEEK_TNP")
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(b => b.UerEntity)
                .WithMany(u => u.BnkseekEntitys)
                .HasPrincipalKey(u => u.UER)
                .HasForeignKey(b => b.UER)
                .HasConstraintName("FK_BNKSEEK_UER")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
