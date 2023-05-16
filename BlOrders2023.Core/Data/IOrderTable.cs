using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data
{
    public interface IOrderTable
    {
        Task<IEnumerable<Order>> GetAsync();
        Task<IEnumerable<Order>> GetAsync(int orderID);
        IEnumerable<Order> Get(int orderID);
        Task<Order> UpsertAsync(Order order);
        Task DeleteAsync(Order order);
        Order Upsert(Order order);
        void Delete(Order order);
    }
}
