using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;

public interface IBLDatabase
{
    public IOrderTable Orders { get; }
    public IWholesaleCustomerTable Customers { get; }
    public IProductsTable Products { get; }
    public IShipDetailsTable ShipDetails { get; }
    public IPaymentsTable Payments { get; }
    public ICustomerClassesTable CustomerClasses { get; }
    public IInventoryTable Inventory { get; }
    public IAllocationTable Allocation { get; }
    public Version dbVersion { get; }
    public DbConnection DbConnection{ get; }
    public CompanyInfo CompanyInfo { get; }
}
