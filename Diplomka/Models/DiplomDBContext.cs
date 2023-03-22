﻿using System;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DBDiplom;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authentications>()
                .HasOne(a => a.Users)
                .WithOne(u => u.Authentications)
                .HasForeignKey<Users>(u => u.AuthenticationId);
            modelBuilder.Entity<Roles>()
                .HasOne(b => b.Users)
                .WithOne(d => d.Roles)
                .HasForeignKey<Users>(d => d.RoleId);
            modelBuilder.Entity<EducationPrCode>()
                .HasOne(p => p.EducationProgram)
                .WithOne(c => c.EducationPrCode)
                .HasForeignKey<EducationProgram>(c => c.EducationCodeId);
            modelBuilder.Entity<EducationProgram>()
                 .HasMany(ep => ep.Subjects)
                 .WithOne(s => s.EducationProgram);
        }
    }
}
