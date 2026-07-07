using PinShot.Core;

namespace PinShot.Core.Tests;

public sealed class PinShotCoreTests
{
    [Fact]
    public void ToggleEnabled_switches_between_enabled_and_paused()
    {
        var state = new PinShotState(true);

        var paused = state.ToggleEnabled();
        var resumed = paused.ToggleEnabled();

        Assert.False(paused.IsEnabled);
        Assert.True(resumed.IsEnabled);
    }

    [Theory]
    [InlineData(1.0, 120, 1.08)]
    [InlineData(1.0, -120, 0.92)]
    [InlineData(0.16, -120, 0.15)]
    [InlineData(5.96, 120, 6.0)]
    public void Zoom_applies_wheel_steps_and_clamps_to_supported_range(
        double currentScale,
        int wheelDelta,
        double expectedScale)
    {
        var actual = PinTransform.Zoom(currentScale, wheelDelta);

        Assert.Equal(expectedScale, actual, precision: 2);
    }

    [Theory]
    [InlineData(1.0, -120, 0.95)]
    [InlineData(0.95, 120, 1.0)]
    [InlineData(0.21, -120, 0.2)]
    [InlineData(1.0, 120, 1.0)]
    public void AdjustOpacity_applies_control_wheel_steps_and_clamps_to_supported_range(
        double currentOpacity,
        int wheelDelta,
        double expectedOpacity)
    {
        var actual = PinTransform.AdjustOpacity(currentOpacity, wheelDelta);

        Assert.Equal(expectedOpacity, actual, precision: 2);
    }

    [Fact]
    public void Default_settings_match_v1_product_choices()
    {
        var settings = AppSettings.Default;

        Assert.Equal("F1", settings.Hotkey);
        Assert.Equal(520, settings.DefaultPinWidth);
        Assert.Equal(1.0, settings.DefaultOpacity);
        Assert.True(settings.StartEnabled);
    }
}
