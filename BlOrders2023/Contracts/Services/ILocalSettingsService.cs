namespace BlOrders2023.Contracts.Services;

public interface ILocalSettingsService
{
    Task<T?> ReadSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);

    T? ReadSetting<T>(string key);

    void SaveSetting<T>(string key, T value);
}
