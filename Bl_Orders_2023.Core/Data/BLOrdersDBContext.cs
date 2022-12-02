using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bl_Orders_2023.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Bl_Orders_2023.Core.Data;
public class BLOrdersDBContext : DbContext
{
    public BLOrdersDBContext (DbContextOptions<BLOrdersDBContext> options) : base(options)
    {

    }
    public DbSet<InventoryItem> Inventory
    {
        get; set;
    }
}
