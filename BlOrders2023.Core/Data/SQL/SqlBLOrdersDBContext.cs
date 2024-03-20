using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BlOrders2023.Core.Data.SQL;
public class SqlBLOrdersDBContext : DbContext
{
    public SqlBLOrdersDBContext(DbContextOptions<SqlBLOrdersDBContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .Property(e => e.OrderStatus)
            .HasConversion(
                v => (int)v,
                v => (OrderStatus)v);
        modelBuilder.Entity<Order>()
            .Property(e => e.Shipping)
            .HasConversion(
                v => (byte)v,
                v => (ShippingType)v);
        modelBuilder.Entity<Order>()
            .Property(e => e.Frozen)
            .HasConversion(
            v => (bool)v ? (short)-1 : (short)0,
            v => (bool)(v == -1)
            );
        modelBuilder.Entity<WholesaleCustomer>()
            .Property(e => e.AllocationType)
            .HasConversion(
            v => v == CustomerAllocationType.Grocer,
            v => v ? CustomerAllocationType.Grocer : CustomerAllocationType.Gift
            );
        modelBuilder.Entity<AllocationGroup>()
            .Property(e => e.ProductIDs)
            .HasConversion(
            v => string.Join(',',v.Select(x => x.ToString())),
            v => v.Split(",", StringSplitOptions.TrimEntries).Select(int.Parse).ToList()
            );
        modelBuilder.Entity<Order>().ToTable(tb => tb.HasTrigger("tgrOrdersLastUpdateDate"));
    }

    public DbSet<InventoryAdjustmentItem> InventoryAdjustments { get; set; }
    public DbSet<WholesaleCustomer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<CustomerClass> CustomerClasses { get; set; }
    public DbSet<ShippingItem> ShippingItems { get; set; }
    public DbSet<OrderTotalsItem> OrderTotalsItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<AllocationGroup> AllocationGroups { get; set; }
    public DbSet<CompanyInfo> CompanyInfo { get; set; }
    public DbSet<Box> Boxes { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<WholesaleCustomerNote> CustomerNotes { get; set; }
    public DbSet<LiveInventoryItem> LiveInventoryItems { get; set; }
    public DbSet<LotCode> LotCodes { get; set; }
    public DbSet<InventoryTotalItem> InventoryTotalItems { get; set; }
    public DbSet<InventoryAuditItem> InventoryAuditItems { get; set; }
}
