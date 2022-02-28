using Context.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Configuration
{
    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.ToTable("Tasks", "dbo")
                .HasKey(k => k.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id")
                .HasComment("Id");

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .HasComment("Name");

            builder.Property(x => x.Status)
                .HasColumnName("Status")
                .HasComment("Status")
                .HasConversion(
                    v => v.ToString(),
                    v => (TaskStatus)Enum.Parse(typeof(TaskStatus), v))
                .IsRequired(true);

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .HasComment("Description");

            builder.Property(x => x.Id_Project)
                .HasColumnName("Id_Project")
                .HasComment("Id_Project");

            builder.Property(x => x.Priority)
                .HasColumnName("Priority")
                .HasComment("Priority")
                .IsRequired(true);

            builder.HasOne(x => x.Project)
                .WithMany(p => p.Tasks)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(b => b.Id_Project)
                .HasConstraintName("FK_Tasks_Projects")
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
