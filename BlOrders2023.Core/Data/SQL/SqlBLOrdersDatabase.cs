using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlOrders2023.Core.Data.SQL;

public class SqlBLOrdersDatabase : IBLDatabase
{

    private readonly DbContextOptions<SqlBLOrdersDBContext> _dbOptions;
    private readonly SqlBLOrdersDBContext _dbContext;

    public SqlBLOrdersDatabase(DbContextOptions<SqlBLOrdersDBContext> dbOptionBuilder)
    {
        _dbOptions = (DbContextOptions<SqlBLOrdersDBContext>)dbOptionBuilder;
        _dbContext = new SqlBLOrdersDBContext(_dbOptions);
    }

    public IOrderTable Orders => new SqlOrderTable(_dbContext);

    public IWholesaleCustomerTable Customers => new SqlWholesaleCustomerTable(_dbContext);

    public IProductsTable Products => new SqlProductsTable(_dbContext);

    public IShipDetailsTable ShipDetails => new SqlShipDetailsTable(_dbContext);

    public IPaymentsTable Payments => new SqlPaymentsTable(_dbContext);

    public ICustomerClassesTable CustomerClasses => new SqlCustomerClassesTable(_dbContext);

    public IInventoryTable Inventory => new SqlInventoryTable(_dbContext);

    public IAllocationTable Allocation => new SqlAllocationTable(_dbContext);

    public IBoxTable Boxes => new SqlBoxTable(_dbContext);
    public IProductCategoriesTable ProductCategories => new SqlProductCategoriesTable(_dbContext);
    public IReportsTable Reports => new SqlReportsTable(_dbContext);
    public IDiscountTable Discounts => new SqlDiscountsTable(_dbContext);
    public Version dbVersion 
    {
        get
        {
            using var command = _dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT TOP (1) [Version_Major], [Version_Minor],[Version_Build] FROM [dbo].[tbl_DBVersion]";
            command.CommandType = CommandType.Text;

            _dbContext.Database.OpenConnection();

            using var result = command.ExecuteReader();
            if (result.Read())
            {
                return new Version(int.Parse(result[0].ToString()), int.Parse(result[1].ToString()), int.Parse(result[2].ToString()));
            }
            return new Version(0, 0, 0);
        }
    }
    public DbConnection DbConnection => _dbContext.Database.GetDbConnection();
    public CompanyInfo CompanyInfo => _dbContext.CompanyInfo.AsNoTracking().ToList().First();

    
}
