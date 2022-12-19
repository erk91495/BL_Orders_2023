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

        public SqlBLOrdersDatabase(DbContextOptionsBuilder<BLOrdersDBContext> dbOptionBuilder)
        {
            _dbOptions = dbOptionBuilder.Options;
            using (var db = new BLOrdersDBContext(_dbOptions))
            {
                db.Database.EnsureCreated();
            }
        }

        public IOrderTable Orders => new OrderTable(new BLOrdersDBContext(_dbOptions));

        public IWholesaleCustomerTable Customers => new WholesaleCustomerTable(new BLOrdersDBContext(_dbOptions));
    }
}
