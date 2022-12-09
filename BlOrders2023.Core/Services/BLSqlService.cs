using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data.SQL;
using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;
namespace BlOrders2023.Core.Services;
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
