using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PinShot.App.Models;
using PinShot.App.Services;
using PinShot.App.Views;
using PinShot.Core;
using Windows.Graphics;

namespace PinShot.App;

public partial class App : Application
{
    private readonly AppSettings _settings = AppSettings.Default;
    private readonly List<PinWindow> _pins = [];
    private ClipboardImageService? _clipboard;
    private HotkeyService? _hotkey;
    private PinShotState _state = new(AppSettings.Default.StartEnabled);
    private TrayService? _tray;
    private MainWindow? _window;
    private Window? _keeperWindow;

    public App()
    {
        UnhandledException += OnUnhandledException;
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            _clipboard = new ClipboardImageService();

            _window = new MainWindow(ToggleEnabled);
            _window.SetState(_state);
            _window.WindowClosed += (_, _) => _window = null;
            _window.Activate();

            // 创建不可见守护窗口，确保主窗口关闭后应用继续运行
            _keeperWindow = new Window();
            _keeperWindow.AppWindow.Move(new PointInt32(-32000, -32000));
            _keeperWindow.AppWindow.Resize(new SizeInt32(1, 1));
            var presenter = OverlappedPresenter.Create();
            presenter.SetBorderAndTitleBar(false, false);
            _keeperWindow.AppWindow.SetPresenter(presenter);

            DispatcherQueue dispatcher = DispatcherQueue.GetForCurrentThread();
            _tray = new TrayService(dispatcher, () => _state, ToggleEnabled, ShowSettings, Quit);

            _hotkey = new HotkeyService(dispatcher);
            _hotkey.HotkeyPressed += OnHotkeyPressed;

            if (!_hotkey.RegisterF1())
            {
                _tray.ShowMessage("PinShot", "F1 is already registered by another app.");
            }
        }
        catch (Exception exception)
        {
            AppLog.Write(exception, "Launch failed");
            throw;
        }
    }

    private void ToggleEnabled()
    {
        _state = _state.ToggleEnabled();
        _tray?.RefreshState();
        _window?.SetState(_state);
    }

    private void ShowSettings()
    {
        if (_window is null)
        {
            _window = new MainWindow(ToggleEnabled);
            _window.SetState(_state);
            _window.WindowClosed += (_, _) => _window = null;
            _window.Activate();
        }
        else
        {
            _window.Activate();
        }
    }

    private async void OnHotkeyPressed(object? sender, EventArgs e)
    {
        if (!_state.IsEnabled)
        {
            return;
        }

        ClipboardImage? image = await _clipboard!.TryGetImageAsync();
        if (image is null)
        {
            _tray?.ShowMessage("PinShot", "Clipboard does not contain an image.");
            return;
        }

        var pin = new PinWindow(image, _settings);
        pin.Closed += (_, _) => _pins.Remove(pin);
        _pins.Add(pin);
        pin.Activate();
    }

    private void Quit()
    {
        _hotkey?.Dispose();
        _tray?.Dispose();

        foreach (var pin in _pins.ToArray())
        {
            pin.Close();
        }

        _keeperWindow?.Close();
        _window?.Close();
        Environment.Exit(0);
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        AppLog.Write(e.Exception, "Unhandled XAML exception");
    }
}
