using CommunityToolkit.Mvvm.ComponentModel;
using BlOrders2023.Core.Services;
using Microsoft.EntityFrameworkCore;
using BlOrders2023.Models;
using System.Linq;
using BlOrders2023.Core.Data.SQL;
using BlOrders2023.Core.Data;

namespace BlOrders2023.ViewModels;

public class FillOrdersViewModel : ObservableRecipient
{
    public FillOrdersViewModel()
    {
        var builder = new DbContextOptionsBuilder<BLOrdersDBContext>();
        builder.UseSqlServer(connectionString: "Data Source=ERIC-PC; Database=New_Bl_Orders;Integrated Security=true; Trust Server Certificate=true",
            opts => opts.CommandTimeout(300));
        var cont = new BLOrdersDBContext(builder.Options);

        WholesaleCustomerTable table = new WholesaleCustomerTable(cont);
        
        var ord = cont.Orders;
        var i = ord.First<Order>();

        var task = Task.Run(async () => await table.GetAsync());
        task.Wait(35000);
        var result = task.Result;


        if (cont != null)
        {
            var item = cont.Inventory.Find(keyValues: 41);
            if (item != null)
            {
                item.QuantityOnHand++;
            }
            try
            {
                cont.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                //TODO: Notify user on fail
                // Update the values of the entity that failed to save from the store
                //ex.Entries.Single().Reload();
            }
        }

    }
}
