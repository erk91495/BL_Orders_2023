using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public Version dbVersion 
        {
            get
            {
                using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT TOP (1) [Version_Major], [Version_Minor],[Version_Build] FROM [dbo].[tbl_DBVersion]";
                    command.CommandType = CommandType.Text;

                    _dbContext.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        if (result.Read())
                        {
                            return new Version(int.Parse(result[0].ToString()), int.Parse(result[1].ToString()), int.Parse(result[2].ToString()));
                        }
                        return new Version(0,0,0);

                    }
                }
                }
        }
    }
}
