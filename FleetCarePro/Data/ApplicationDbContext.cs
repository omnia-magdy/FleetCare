using FleetCarePro.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FleetCarePro.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ServiceCenter> ServiceCenters => Set<ServiceCenter>();
    public DbSet<VendorService> VendorServices => Set<VendorService>();
    public DbSet<ServiceRecord> ServiceRecords => Set<ServiceRecord>();
    public DbSet<ServiceLineItem> ServiceLineItems => Set<ServiceLineItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Seed Service Centers
        builder.Entity<ServiceCenter>().HasData(
            new ServiceCenter { Id = 1, Name = "Main Workshop - Cairo", IsActive = true },
            new ServiceCenter { Id = 2, Name = "Giza Auto Maintenance", IsActive = true }
        );

        // Seed Service Categories
        builder.Entity<ServiceCategory>().HasData(
            new ServiceCategory { Id = 1, CategoryName = "Oil Change" },
            new ServiceCategory { Id = 2, CategoryName = "Tire Replacement" },
            new ServiceCategory { Id = 3, CategoryName = "Brake Repair" }
        );

        builder.Entity<Vehicle>(entity =>
        {
            entity.HasIndex(v => v.VIN).IsUnique();
            entity.Property(v => v.PurchasePrice).HasPrecision(18, 2);

            entity.HasOne(v => v.AssignedDriver)
                  .WithMany(u => u.AssignedVehicles)
                  .HasForeignKey(v => v.AssignedDriverId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Join Table (ServiceCenter <-> ServiceCategory)
        builder.Entity<VendorService>(entity =>
        {
            entity.HasKey(vs => new { vs.ServiceCenterId, vs.ServiceCategoryId });

            entity.HasOne(vs => vs.ServiceCenter)
                  .WithMany(sc => sc.VendorServices)
                  .HasForeignKey(vs => vs.ServiceCenterId);

            entity.HasOne(vs => vs.ServiceCategory)
                  .WithMany(cat => cat.VendorServices)
                  .HasForeignKey(vs => vs.ServiceCategoryId);
        });

        //  ServiceRecord Configurations 
        builder.Entity<ServiceRecord>(entity =>
        {
            entity.Property(sr => sr.TotalCost).HasPrecision(18, 2);

            entity.HasOne(sr => sr.Vehicle)
                  .WithMany(v => v.ServiceRecords)
                  .HasForeignKey(sr => sr.VehicleId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sr => sr.ServiceCenter)
                  .WithMany(sc => sc.ServiceRecords)
                  .HasForeignKey(sr => sr.ServiceCenterId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sr => sr.CreatedByUser)
                  .WithMany(u => u.CreatedServiceRecords)
                  .HasForeignKey(sr => sr.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 4. ServiceLineItem Configurations (Detail)
        builder.Entity<ServiceLineItem>(entity =>
        {
            entity.Property(sli => sli.Cost).HasPrecision(18, 2);

            entity.HasOne(sli => sli.ServiceRecord)
                  .WithMany(sr => sr.ServiceLineItems)
                  .HasForeignKey(sli => sli.ServiceRecordId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sli => sli.ServiceCategory)
                  .WithMany(sc => sc.ServiceLineItems)
                  .HasForeignKey(sli => sli.ServiceCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}