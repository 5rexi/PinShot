namespace PinShot.Core;

public static class PinTransform
{
    public const double MinimumScale = 0.15;
    public const double MaximumScale = 6.0;
    public const double MinimumOpacity = 0.2;
    public const double MaximumOpacity = 1.0;

    public static double Zoom(double currentScale, int wheelDelta)
    {
        var direction = Math.Sign(wheelDelta);
        var next = currentScale + direction * 0.08;
        return Math.Round(Math.Clamp(next, MinimumScale, MaximumScale), 2);
    }

    public static double AdjustOpacity(double currentOpacity, int wheelDelta)
    {
        var direction = Math.Sign(wheelDelta);
        var next = currentOpacity + direction * 0.05;
        return Math.Round(Math.Clamp(next, MinimumOpacity, MaximumOpacity), 2);
    }
}
