using Windows.System;

namespace BlOrders2023.Helpers;

public static class Helpers
{
    public static async Task SendEmailAsync(string email)
    {
        await Launcher.LaunchUriAsync(new Uri(String.Format("mailto:{0}", email)));
    }
}