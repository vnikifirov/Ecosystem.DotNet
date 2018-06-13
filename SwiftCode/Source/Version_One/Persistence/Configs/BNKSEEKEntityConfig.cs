using System;
using bank_identification_code.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bank_identification_code.Persistence.Configs
{
    public class BNKSEEKEntityConfig : IEntityTypeConfiguration<BNKSEEKEntity>
    {
        public void Configure(EntityTypeBuilder<BNKSEEKEntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore( b => b.PZNEntity);
            builder.Ignore(b => b.REGEntity);
            builder.Ignore(b => b.TNPEntity);
            builder.Ignore(b => b.UEREntity);

            // Override Table Name
            builder.ToTable("BNKSEEK");

            // Make the default column as a string type. Also, they're keys
            builder.Property<string>(b => b.VKEY).HasColumnType("char(8)");
            builder.Property<string>(b => b.REAL).HasColumnType("char(4)");
            builder.Property<string>(b => b.PZN).HasColumnType("char(2)");
            builder.Property<string>(b => b.UER).HasColumnType("char(1)");
            builder.Property<string>(b => b.RGN).HasColumnType("char(2)");
            builder.Property<string>(b => b.TNP).HasColumnType("char(1)");
            builder.Property<string>(b => b.VKEYDEL).HasColumnType("char(8)");

            // Specify a column names
            builder.Property<string>(b => b.IND).HasColumnType("char(6)");
            builder.Property<string>(b => b.NNP).HasColumnType("char(25)");
            builder.Property<string>(b => b.ADR).HasColumnType("char(30)");
            builder.Property<string>(b => b.RKC).HasColumnType("char(9)");
            builder.Property<string>(b => b.NAMEP).HasColumnType("char(45)");
            builder.Property<string>(b => b.NAMEN).HasColumnType("char(30)");
            builder.Property<string>(b => b.NEWNUM).HasColumnType("char(9)");
            builder.Property<string>(b => b.NEWKS).HasColumnType("char(9)");
            builder.Property<string>(b => b.PERMFO).HasColumnType("char(6)");
            builder.Property<string>(b => b.SROK).HasColumnType("char(2)");
            builder.Property<string>(b => b.AT1).HasColumnType("char(7)");
            builder.Property<string>(b => b.AT2).HasColumnType("char(7)");
            builder.Property<string>(b => b.TELEF).HasColumnType("char(25)");
            builder.Property<string>(b => b.REGN).HasColumnType("char(9)");
            builder.Property<string>(b => b.OKPO).HasColumnType("char(8)");
            builder.Property<DateTime>(b => b.DT_IZM).HasColumnType("datetime2");
            builder.Property<string>(b => b.CKS).HasColumnType("char(6)");
            builder.Property<string>(b => b.KSNP).HasColumnType("char(20)");
            builder.Property<DateTime>(b => b.DATE_IN).HasColumnType("datetime2");
            builder.Property<DateTime>(b => b.DATE_CH).HasColumnType("datetime2");
            builder.Property<string>(b => b.VKEYDEL).HasColumnType("char(9)");
            builder.Property<DateTime>(b => b.DT_IZMR).HasColumnType("datetime2");

            // Make NOT NULL constraints associated with properties
            builder.Property<string>(b => b.UER).IsRequired();
            builder.Property<string>(b => b.RGN).IsRequired();
            builder.Property<string>(b => b.NAMEP).IsRequired();
            builder.Property<string>(b => b.NAMEN).IsRequired();
            builder.Property<string>(b => b.NEWNUM).IsRequired();
            builder.Property<string>(b => b.SROK).IsRequired();
            builder.Property<DateTime>(b => b.DT_IZM).IsRequired();
            builder.Property<DateTime>(b => b.DATE_IN).IsRequired();

            // Make the Primary key associated with property VKEY
            builder.HasKey( b => b.VKEY);
            builder.Property( b => b.VKEY).ValueGeneratedNever();

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

            builder.HasOne<PZNEntity>( b =>  b.PZNEntity)
                .WithMany( p => p.BNKSEEKEntitys)
                .HasPrincipalKey( p => p.PZN )
                .HasForeignKey( b => b.PZN )
                .HasConstraintName( "FK_BNKSEEK_PZN" )
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne<REGEntity>( b =>  b.REGEntity)
                .WithMany( r => r.BNKSEEKEntitys)
                .HasPrincipalKey( r => r.RGN )
                .HasForeignKey( b => b.RGN )
                .HasConstraintName( "FK_BNKSEEK_REG" )
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne<TNPEntity>( b =>  b.TNPEntity)
                .WithMany( t => t.BNKSEEKEntitys)
                .HasPrincipalKey( t => t.TNP )
                .HasForeignKey( b => b.TNP )
                .HasConstraintName( "FK_BNKSEEK_TNP" )
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne<UEREntity>( b =>  b.UEREntity)
                .WithMany( u => u.BNKSEEKEntitys)
                .HasPrincipalKey( u => u.UER )
                .HasForeignKey( b => b.UER )
                .HasConstraintName( "FK_BNKSEEK_UER" )
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // builder.HasOne<UEREntity>( b =>  b.UEREntity)
            //     .WithOne( u => u.BNKSEEKEntity)
            //     .HasPrincipalKey<UEREntity>( u => u.UER )
            //     .HasForeignKey<BNKSEEKEntity>( b => b.UER )
            //     .HasConstraintName( "FK_BNKSEEK_UER" )
            //     .IsRequired()
            //     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}