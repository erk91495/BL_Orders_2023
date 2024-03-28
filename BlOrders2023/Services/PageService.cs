using BlOrders2023.Contracts.Services;
using BlOrders2023.ViewModels;
using BlOrders2023.Views;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Controls;

namespace BlOrders2023.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<OrdersPageViewModel, OrdersPage>();
        Configure<ReportsPageViewModel, ReportsPage>();
        Configure<OrderDetailsPageViewModel, OrderDetailsPage>();
        Configure<ProductsPageViewModel, ProductsPage>();
        Configure<FillOrdersPageViewModel, FillOrdersPage>();
        Configure<WholesaleCustomersViewModel, WholesaleCustomersPage>();
        Configure<AllocatorPageViewModel, AllocatorPage>();
        Configure<PaymentsPageViewModel, PaymentsPage>();
        Configure<SettingsViewModel, SettingsPage>();
        Configure<InventoryPageViewModel, InventoryPage>();
        Configure<LiveInventoryPageViewModel, LiveInventoryPage>();
        Configure<InventoryAdjustmentsPageViewModel, InventoryAdjustmentsPage>();
        Configure<InventoryAuditLogPageViewModel, InventoryAuditLogPage>();
        Configure<InventoryReconciliationPageViewModel, InventoryReconciliationPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.Any(p => p.Value == type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
