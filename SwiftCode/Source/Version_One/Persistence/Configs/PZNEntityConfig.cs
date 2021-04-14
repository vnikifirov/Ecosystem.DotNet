using System;
using System.ComponentModel.DataAnnotations.Schema;
using bank_identification_code.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bank_identification_code.Persistence.Configs
{
    public class PZNEntityConfig : IEntityTypeConfiguration<PZNEntity>
    {
        public void Configure(EntityTypeBuilder<PZNEntity> builder)
        {
            // Prevent creating relationships
            builder.Ignore( p => p.BNKSEEKEntitys);

            // Override Table Name
            builder.ToTable("PZN");

            // Make the default column type string
            builder.Property<string>(p => p.VKEY).HasColumnType("char(8)");
            builder.Property<string>(p => p.PZN).HasColumnType("char(2)");
            builder.Property<string>(p => p.IMY);
            builder.Property<string>(p => p.NAME);
            builder.Property<DateTime>(p => p.CB_DATE).HasColumnType("datetime2");
            builder.Property<DateTime>(p => p.CE_DATE).HasColumnType("datetime2");

            // Make the Primary key associated with property VKEY
            builder.HasKey( p => p.VKEY);
            builder.Property( p => p.VKEY).ValueGeneratedNever();

            // Make the Unique Key associated with property PZN
            builder.HasIndex(p => p.PZN).IsUnique();
        }
    }
}