using System;
using Context.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Context.Context.Configuration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects", "dbo")
                .HasKey(k => k.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id")
                .HasComment("Id");

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .HasComment("Name");

            builder.Property(x => x.Start)
                .HasColumnName("Start")
                .HasComment("Start");

            builder.Property(x => x.Completion)
                .HasColumnName("Completion")
                .HasComment("Completion");

            builder.Property(x => x.Status)
                .HasColumnName("Status")
                .HasComment("Status")
                .HasConversion(
                    v => v.ToString(),
                    v => (ProjectStatus)Enum.Parse(typeof(ProjectStatus), v))
                .IsRequired(true);

            builder.Property(x => x.Priority)
                .HasColumnName("Priority")
                .HasComment("Priority")
                .IsRequired(true);

        }
    }
}


