using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.ViewModels
{
    public class CustomerDataInputControlViewModel
    {
        #region Properties
        #endregion Properties
        #region Fields
        public WholesaleCustomer Customer { get; set; } = new();
        #endregion Fields
        #region Constructors
        public CustomerDataInputControlViewModel()
        {
            Customer = new WholesaleCustomer();
        }
        #endregion Constructors
        #region Methods
        #endregion Methods
    }
}
