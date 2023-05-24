using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Diplomka.Models;

namespace Diplomka.Models
{
    public class DiplomDBContext : DbContext
    {
        public DbSet<Authentications> Authentications { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Subjects> Subjects { get; set; }
        public DbSet<Universities> Universities { get; set; }
        public DbSet<EducationPrCode> EducationPrCode { get; set; }
        public DbSet<EducationProgram> EducationProgram { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<ENTMINPredictions> ENTMINPredictions { get; set; }
        public DbSet<ENTAVGPredictions> ENTAVGPredictions { get; set; }
        public DbSet<Resume> Resumes { get; set; }

        public DbSet<HREmployees> HREmployees { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=DBDiplom; Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authentications>()
                .HasOne(a => a.Users)
                .WithOne(u => u.Authentications)
                .HasForeignKey<Users>(u => u.AuthenticationId);

            modelBuilder.Entity<Roles>()
                .HasOne(r => r.Users)
                .WithOne(u => u.Roles)
                .HasForeignKey<Users>(r => r.RoleId);

            modelBuilder.Entity<EducationPrCode>()
                .HasOne(epc => epc.EducationProgram)
                .WithOne(ep => ep.EducationPrCode)
                .HasForeignKey<EducationProgram>(ep => ep.EducationCodeId);

            modelBuilder.Entity<EducationProgram>()
                .HasMany(ep => ep.Subjects)
                .WithOne(s => s.EducationProgram);

            modelBuilder.Entity<Users>()
                .HasOne(u => u.HREmployees)
                .WithOne(hr => hr.Users)
                .HasForeignKey<HREmployees>(hr => hr.UserId);
        }
    }
}
