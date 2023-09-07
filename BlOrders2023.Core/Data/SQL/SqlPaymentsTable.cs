using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


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

    public async Task<IEnumerable<Payment>> GetPaymentsAsync(string query = null)
    {
        IEnumerable<Payment> result;
        if (!query.IsNullOrEmpty())
        {
            result = await _db.Payments.Where(p => p.PaymentID.ToString().Contains(query) || p.Customer.CustomerName.Contains(query) )
                                    .Include(p => p.PaymentMethod)
                                    .Include(p => p.Customer)
                                    .OrderByDescending(p => p.PaymentID)
                                    .ToListAsync();
        }
        else
        {
            result = await _db.Payments.OrderByDescending(p => p.PaymentID).ToListAsync();
        }
        foreach (var item in result) 
        {
            await _db.Entry(item).Reference(p => p.Customer).LoadAsync();
            await _db.Entry(item).Reference(p => p.PaymentMethod).LoadAsync();
        }
        return result;

    }
    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
    {
        return await _db.PaymentMethods.ToListAsync();
    }

    public async Task UpsertPaymentAsync(Payment payment)
    {
        _db.Payments.Update(payment);
        await _db.SaveChangesAsync();
    }
}
