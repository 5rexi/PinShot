namespace PinShot.Core;

public readonly record struct PinShotState(bool IsEnabled)
{
    public PinShotState ToggleEnabled()
    {
        return this with { IsEnabled = !IsEnabled };
    }
}
