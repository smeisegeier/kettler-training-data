using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploader.Models
{
    /* EF core
     * 0) Have models classes, all of them must have Id property
     * 1) Fetch NugetPackage for Core, SqlServer and Tools (!)
     * 2) create DbSet<> for each model
     * 3) ctor: with options calling base ctor
     * 4a) give conString in appsettings.json -> 
     * 4b) inject Dependency per services.AddDbContext in startup 
     * 4c) fetch context in controller from ctor and use as _context
     * 5) in pm console: Add-Migration Init, Update-Database
     */

    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Training>()
                .HasIndex(i => i.FileName)
                .IsUnique()
                .IsClustered(false);
            modelBuilder.HasDefaultSchema("kettler");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Training> Trainings { get; set; }
        public DbSet<Record> Records { get; set; }
    }
}
