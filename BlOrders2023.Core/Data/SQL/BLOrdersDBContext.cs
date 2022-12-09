using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;

namespace BlOrders2023.Core.Data.SQL;
public class BLOrdersDBContext : DbContext
{
    public BLOrdersDBContext(DbContextOptions<BLOrdersDBContext> options) : base(options)
    { }
    public DbSet<InventoryItem> Inventory { get; set; }
    public DbSet<WholesaleCustomer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}
