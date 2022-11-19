namespace Bl_Orders_2023.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
