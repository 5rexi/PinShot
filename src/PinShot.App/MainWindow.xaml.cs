using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PinShot.App.Services;
using PinShot.Core;
using Windows.Graphics;

namespace PinShot.App;

public sealed partial class MainWindow : Window
{
    private readonly Action _toggleEnabled;
    private bool _isUpdating;

    public event EventHandler? WindowClosed;

    public MainWindow(Action toggleEnabled)
    {
        _toggleEnabled = toggleEnabled;
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");
        AppWindow.Resize(new SizeInt32(560, 400));

        Closed += (_, _) => WindowClosed?.Invoke(this, EventArgs.Empty);

        _isUpdating = true;
        AutoStartSwitch.IsOn = AutoStartService.IsEnabled();
        _isUpdating = false;
    }

    public void SetState(PinShotState state)
    {
        _isUpdating = true;
        EnabledSwitch.IsOn = state.IsEnabled;
        StatusText.Text = state.IsEnabled
            ? "Ready. F1 will pin the clipboard image."
            : "Paused. F1 will be ignored until re-enabled.";
        _isUpdating = false;
    }

    private void EnabledSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        _toggleEnabled();
    }

    private void AutoStartSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        AutoStartService.SetEnabled(AutoStartSwitch.IsOn);
    }
}
