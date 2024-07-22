using System.Runtime.Versioning;
using Microsoft.Win32;

namespace HardWareMonitorService.Service;

[SupportedOSPlatform("windows")]
public class RegistryService
{
    private const string key = "userPcId";
    
    
    public string? CreateRegistry(string userPcId)
    {
        var currentUserKey = Registry.LocalMachine;
        var appName = AppDomain.CurrentDomain.FriendlyName;
        var software = currentUserKey.OpenSubKey("SOFTWARE", true);
        var helloKey= software.CreateSubKey(appName,true);
        helloKey.SetValue("userPcId", userPcId);
        helloKey.Close();
        software.Close();
        return GetUserPcId();
    }

    public string? GetUserPcId()
    {
        var currentUserKey = Registry.LocalMachine;
        var registryKey = currentUserKey.OpenSubKey("SOFTWARE").OpenSubKey(AppDomain.CurrentDomain.FriendlyName);
        return registryKey?.GetValue(key)?.ToString();
    }
}