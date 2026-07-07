using Microsoft.Win32;

namespace PinShot.App.Services;

public static class AutoStartService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "PinShot";

    public static bool IsEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath);
            if (key is null)
            {
                return false;
            }

            var value = key.GetValue(AppName) as string;
            return !string.IsNullOrEmpty(value);
        }
        catch
        {
            return false;
        }
    }

    public static void SetEnabled(bool enabled)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
            if (key is null)
            {
                return;
            }

            if (enabled)
            {
                var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                key.SetValue(AppName, exePath);
            }
            else
            {
                if (key.GetValue(AppName) is not null)
                {
                    key.DeleteValue(AppName);
                }
            }
        }
        catch
        {
            // Silently ignore registry failures
        }
    }
}
