using Microsoft.Win32;

namespace PinShot.App.Services;

internal static class IconService
{
    public static string GetThemeAwareIconPath()
    {
        return IsSystemLightTheme()
            ? Path.Combine(AppContext.BaseDirectory, "Assets", "PinShotDark.ico")
            : Path.Combine(AppContext.BaseDirectory, "Assets", "PinShotLight.ico");
    }

    public static bool IsSystemLightTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        return key?.GetValue("AppsUseLightTheme") is not int value || value != 0;
    }
}
