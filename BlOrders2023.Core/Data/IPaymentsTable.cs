using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data
{
    public interface IPaymentsTable
    {
        public IEnumerable<Payment> GetPayments(DateTime startDate, DateTime endDate);
    }
}
