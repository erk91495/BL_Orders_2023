using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;

namespace BlOrders2023.Core.Data.SQL
{
    public class ShipDetailsTable : IShipDetailsTable
    {
        #region Properties
        #endregion Properties
        #region Fields
        /// <summary>
        /// The DB context for the Bl orders database
        /// </summary>
        private readonly BLOrdersDBContext _db;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Creates a new instance of the OrderTable
        /// </summary>
        /// <param name="db">The Db context for the ordres database</param>
        public ShipDetailsTable(BLOrdersDBContext db)
        {
            _db = db;
        }
        #endregion Constructors

        public async Task<IEnumerable<ShippingItem>> GetAsync()
        {
            return await _db.ShippingItems.ToListAsync();
        }

        public async Task<bool> IsDuplicateScanline(string scanline)
        {
            var item = await _db.ShippingItems.Where(item => item.Scanline == scanline).FirstOrDefaultAsync();
            return item != null;
        }
    }
}
