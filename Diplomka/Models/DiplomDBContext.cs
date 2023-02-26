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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\\MSSQLLocalDB;Database=DBDiplom;Trusted_Connection=True;");
        }
    }
}
