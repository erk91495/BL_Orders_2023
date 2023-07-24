using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data.SQL;

internal class SqlPaymentsTable : IPaymentsTable
{
    private readonly SqlBLOrdersDBContext _db;

    public SqlPaymentsTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }

    public IEnumerable<Payment> GetPayments(DateTime startDate, DateTime endDate)
    {
        return _db.Payments.Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate).OrderBy(p => p.PaymentDate).ThenBy(p => p.Customer.CustomerName).ToList();
    }
}
