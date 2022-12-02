using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bl_Orders_2023.Core.Data;
using Bl_Orders_2023.Core.Models;
using Microsoft.EntityFrameworkCore;
namespace Bl_Orders_2023.Core.Services;
public class BLSqlService
{
    private readonly BLOrdersDBContext _context;

    public BLSqlService(BLOrdersDBContext context)
    {
        _context = context;
    }

    public DbSet<InventoryItem> GetInventoryItems()
    {
        return _context.Inventory;
    }
}
