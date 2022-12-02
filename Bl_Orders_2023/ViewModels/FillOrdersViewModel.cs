using CommunityToolkit.Mvvm.ComponentModel;
using Bl_Orders_2023.Core.Services;
using Bl_Orders_2023.Core.Data;
using Microsoft.EntityFrameworkCore;
using Bl_Orders_2023.Core.Models;
using System.Linq;

namespace Bl_Orders_2023.ViewModels;

public class FillOrdersViewModel : ObservableRecipient
{
    public FillOrdersViewModel()
    {
        var builder = new DbContextOptionsBuilder<BLOrdersDBContext>();
        builder.UseSqlServer(connectionString: "Data Source=ERIC-PC; Database=New_Bl_Orders;Integrated Security=true; Trust Server Certificate=true");
        var cont = new BLOrdersDBContext(builder.Options);
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

                // Update the values of the entity that failed to save from the store
                ex.Entries.Single().Reload();
            }
        }

    }
}
