using BlOrders2023.Contracts.Services;
using BlOrders2023.Helpers;
using BlOrders2023.Services;
using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;

namespace BlOrders2023.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;
    

    public DefaultActivationHandler(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        // None of the ActivationHandlers has handled the activation.
        return _navigationService.Frame?.Content == null;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        var settings = App.GetService<ILocalSettingsService>();
        if (!settings.ReadSetting<bool>(LocalSettingsKeys.DBSettingsSet))
        {
            _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!, args.Arguments);
        }
        else
        {
            _navigationService.NavigateTo(typeof(OrdersPageViewModel).FullName!, args.Arguments);
        }
        

        await Task.CompletedTask;
    }
}
