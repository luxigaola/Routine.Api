﻿using Microsoft.EntityFrameworkCore;
using Routine.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Data
{
    public class RoutineDbContext:DbContext
    {
        public RoutineDbContext(DbContextOptions<RoutineDbContext> options)
            :base(options)
        {

        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .Property(x => x.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Company>()
                .Property(x => x.Introduction).IsRequired().HasMaxLength(500);

            modelBuilder.Entity<Employee>()
                .Property(x => x.EmployeeNo).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Employee>()
                .Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>()
                .Property(x => x.LastName).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Company)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>().HasData(
                new Company 
                {
                    Id=Guid.Parse("3a496508-384f-2fb2-5d1e-4e19328df49a"),
                    Name="Microsoft",
                    Introduction="Great Company"
                },
                new Company 
                {
                    Id=Guid.Parse("c195d4f5-d469-c2b7-97bf-97cc8ff34b5f"),
                    Name="Google",
                    Introduction="Don't be evil"
                },
                new Company
                {
                    Id=Guid.Parse("09802ddf-f462-d23f-88b5-b6991e7cd6c9"),
                    Name="Alipapa",
                    Introduction="Fubao Company"
                });
        }
    }
}