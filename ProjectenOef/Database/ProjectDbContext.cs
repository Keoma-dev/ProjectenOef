using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectenOef.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectenOef.Database
{
    public class ProjectDbContext : IdentityDbContext<ProjectAppUser>
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProjectStatus>().HasData(
                new ProjectStatus()
                {
                    Id = 1,
                    Status = "Toekomstig Project"
                },
                new ProjectStatus()
                {
                    Id = 2,
                    Status = "Huidig Project"
                },
                new ProjectStatus()
                {
                    Id = 3,
                    Status = "Afgewerkt Project"
                });

            builder.Entity<ProjectTag>()
                .HasKey(pt => new { pt.ProjectId, pt.TagId });

            builder.Entity<ProjectTag>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.ProjectTags)
                .HasForeignKey(pt => pt.ProjectId);

            builder.Entity<ProjectTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProjectTags)
                .HasForeignKey(pt => pt.TagId);

            builder.Entity<Tag>()
           .HasData(
           new Tag() { Id = 1, Name = "C#" },
           new Tag() { Id = 2, Name = ".NET" },
           new Tag() { Id = 3, Name = "ASP.NET" },
           new Tag() { Id = 4, Name = "HTML" },
           new Tag() { Id = 5, Name = "CSS" });

        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProjectTag> ProjectTags { get; set; }

    }
}
