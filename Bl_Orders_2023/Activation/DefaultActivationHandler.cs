using Bl_Orders_2023.Contracts.Services;
using Bl_Orders_2023.ViewModels;

using Microsoft.UI.Xaml;

namespace Bl_Orders_2023.Activation;

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
        _navigationService.NavigateTo(typeof(FillOrdersViewModel).FullName!, args.Arguments);

        await Task.CompletedTask;
    }
}
