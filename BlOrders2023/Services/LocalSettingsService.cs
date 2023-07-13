using BlOrders2023.Contracts.Services;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
using Microsoft.Extensions.Options;

using Windows.ApplicationModel;
using Windows.Storage;

namespace BlOrders2023.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string _defaultApplicationDataFolder = "BlOrders2023/ApplicationData";
    private const string _defaultLocalSettingsFile = "LocalSettings.json";

    private readonly IFileService _fileService;
    private readonly LocalSettingsOptions _options;

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;
    private readonly string _localsettingsFile;

    private IDictionary<string, object> _settings;

    private bool _isInitialized;

    public LocalSettingsService(IFileService fileService, IOptions<LocalSettingsOptions> options)
    {
        _fileService = fileService;
        _options = options.Value;

        _applicationDataFolder = Path.Combine(_localApplicationData, _options.ApplicationDataFolder ?? _defaultApplicationDataFolder);
        _localsettingsFile = _options.LocalSettingsFile ?? _defaultLocalSettingsFile;

        _settings = new Dictionary<string, object>();
        bool firstRun;
        if (RuntimeHelper.IsMSIX)
        {
            firstRun = !ApplicationData.Current.LocalSettings.Values.TryGetValue(LocalSettingsKeys.FirstRun, out _);
        }
        else
        {
            var settings = _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile) ?? new Dictionary<string, object>();
            firstRun = !settings.TryGetValue(LocalSettingsKeys.FirstRun, out _);
        }
        if (firstRun) 
        {
            WriteDefaultLocalSettings();
        }
    }

    //private async Task WriteDefaultLocalSettingsAsync()
    //{
    //    await Task.WhenAll(
    //    SaveSettingAsync(LocalSettingsKeys.FirstRun, false),
    //    SaveSettingAsync(LocalSettingsKeys.DatabaseServer, "ERIC-PC"),
    //    SaveSettingAsync(LocalSettingsKeys.DatabaseName, "New_Bl_Orders")
    //    );
    //}


    //TODO: CHANGE BEFORE RELEASE
    private void WriteDefaultLocalSettings()
    {
        SaveSetting(LocalSettingsKeys.FirstRun, false);
        SaveSetting(LocalSettingsKeys.DatabaseServer, "ERIC-PC");
        SaveSetting(LocalSettingsKeys.DatabaseName, "New_Bl_Orders");
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _settings = await Task.Run(() => _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile)) ?? new Dictionary<string, object>();

            _isInitialized = true;
        }
    }

    private void Initialize()
    {
        if (!_isInitialized)
        {
            _settings = _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile) ?? new Dictionary<string, object>();

            _isInitialized = true;
        }
    }

    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }
        else
        {
            await InitializeAsync();

            if (_settings != null && _settings.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value);
        }
        else
        {
            await InitializeAsync();

            _settings[key] = await Json.StringifyAsync(value);

            await Task.Run(() => _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings));
        }
    }

    private void SaveSetting<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = Json.Stringify(value);
        }
        else
        {
            Initialize();

            _settings[key] = Json.Stringify(value);

            _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings);
        }
    }
}
