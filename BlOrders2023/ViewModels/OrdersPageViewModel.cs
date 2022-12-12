using CommunityToolkit.Mvvm.ComponentModel;
using BlOrders2023.Core.Services;
using Microsoft.EntityFrameworkCore;
using BlOrders2023.Models;
using System.Linq;
using BlOrders2023.Core.Data.SQL;
using BlOrders2023.Core.Data;
using System.Collections.ObjectModel;

namespace BlOrders2023.ViewModels;

public class OrdersPageViewModel : ObservableRecipient
{
    public List<Order> Orders { get; set; } = new List<Order>();
    public OrdersPageViewModel()
    {
        var builder = new DbContextOptionsBuilder<BLOrdersDBContext>();
        builder.UseSqlServer(connectionString: "Data Source=.; Database=New_Bl_Orders;Integrated Security=true; Trust Server Certificate=true",
            opts => opts.CommandTimeout(300));
        var cont = new BLOrdersDBContext(builder.Options);

        OrderTable table = new OrderTable(cont);

        var ord = cont.Orders;
        var i = ord.First<Order>();

        var task = Task.Run(async () => await table.GetAsync());
        task.Wait(35000);
        var result = task.Result;
        if(result != null)
            Orders = (List<Order>)result;

        //if (cont != null)
        //{
        //    var item = cont.Inventory.Find(keyValues: 41);
        //    if (item != null)
        //    {
        //        item.QuantityOnHand++;
        //    }
        //    try
        //    {
        //        cont.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {

        //        //TODO: Notify user on fail
        //        // Update the values of the entity that failed to save from the store
        //        //ex.Entries.Single().Reload();
        //    }
        //}

    }
}
