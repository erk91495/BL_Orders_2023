using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data.SQL
{
    public class SqlBLOrdersDatabase : IBLDatabase
    {

        private readonly DbContextOptions<BLOrdersDBContext> _dbOptions;
        private readonly BLOrdersDBContext _dbContext;

        public SqlBLOrdersDatabase(DbContextOptions<BLOrdersDBContext> dbOptionBuilder)
        {
            _dbOptions = (DbContextOptions<BLOrdersDBContext>)dbOptionBuilder;
            _dbContext = new BLOrdersDBContext(_dbOptions);
            _dbContext.Database.EnsureCreated();

        }

        public IOrderTable Orders => new OrderTable(_dbContext);

        public IWholesaleCustomerTable Customers => new WholesaleCustomerTable(_dbContext);

        public IProductsTable Products => new ProductsTable(_dbContext);

        public IShipDetailsTable ShipDetails => new ShipDetailsTable(_dbContext);
    }
}
