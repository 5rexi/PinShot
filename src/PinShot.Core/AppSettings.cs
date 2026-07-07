namespace PinShot.Core;

public sealed record AppSettings(
    string Hotkey,
    int DefaultPinWidth,
    double DefaultOpacity,
    bool StartEnabled)
{
    public static AppSettings Default { get; } = new("F1", 520, 1.0, true);
}
