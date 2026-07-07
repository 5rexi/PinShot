using Microsoft.UI.Dispatching;
using PinShot.App.Interop;
using PinShot.Core;
using System.Runtime.InteropServices;

namespace PinShot.App.Services;

public sealed class TrayService : IDisposable
{
    private const uint TrayIconId = 1;
    private const uint CallbackMessage = NativeMethods.WM_APP + 42;
    private const uint CommandToggle = 1001;
    private const uint CommandSettings = 1002;
    private const uint CommandQuit = 1003;

    private readonly DispatcherQueue _dispatcher;
    private readonly Action _exit;
    private readonly Func<PinShotState> _getState;
    private readonly NativeMessageWindow _window;
    private readonly Action _showSettings;
    private readonly Action _toggleEnabled;
    private bool _disposed;
    private IntPtr _iconHandle;

    public TrayService(
        DispatcherQueue dispatcher,
        Func<PinShotState> getState,
        Action toggleEnabled,
        Action showSettings,
        Action exit)
    {
        _dispatcher = dispatcher;
        _getState = getState;
        _toggleEnabled = toggleEnabled;
        _showSettings = showSettings;
        _exit = exit;
        _window = new NativeMessageWindow("PinShotTrayWindow", OnMessage);

        AddIcon();
        RefreshState();
    }

    public void RefreshState()
    {
        if (_disposed)
        {
            return;
        }

        UpdateIcon(_getState().IsEnabled ? "PinShot - F1 enabled" : "PinShot - paused");
    }

    public void ShowMessage(string title, string message)
    {
        var data = CreateData(NativeMethods.NIF_INFO);
        data.szInfoTitle = Limit(title, 63);
        data.szInfo = Limit(message, 255);
        NativeMethods.ShellNotifyIcon(NativeMethods.NIM_MODIFY, ref data);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        var data = CreateData(0);
        NativeMethods.ShellNotifyIcon(NativeMethods.NIM_DELETE, ref data);

        if (_iconHandle != IntPtr.Zero)
        {
            NativeMethods.DestroyIcon(_iconHandle);
            _iconHandle = IntPtr.Zero;
        }

        _window.Dispose();
    }

    private void AddIcon()
    {
        _iconHandle = LoadTrayIcon();
        var data = CreateData(NativeMethods.NIF_MESSAGE | NativeMethods.NIF_ICON | NativeMethods.NIF_TIP);
        data.hIcon = _iconHandle;
        data.szTip = "PinShot";

        NativeMethods.ShellNotifyIcon(NativeMethods.NIM_ADD, ref data);
        data.uTimeoutOrVersion = NativeMethods.NOTIFYICON_VERSION_4;
        NativeMethods.ShellNotifyIcon(NativeMethods.NIM_SETVERSION, ref data);
    }

    private void UpdateIcon(string tooltip)
    {
        var newIcon = LoadTrayIcon();
        var oldIcon = _iconHandle;
        _iconHandle = newIcon;

        var data = CreateData(NativeMethods.NIF_ICON | NativeMethods.NIF_TIP);
        data.hIcon = _iconHandle;
        data.szTip = Limit(tooltip, 127);
        NativeMethods.ShellNotifyIcon(NativeMethods.NIM_MODIFY, ref data);

        if (oldIcon != IntPtr.Zero)
        {
            NativeMethods.DestroyIcon(oldIcon);
        }
    }

    private bool OnMessage(uint message, IntPtr wParam, IntPtr lParam)
    {
        if (message != CallbackMessage)
        {
            return false;
        }

        var trayMessage = (uint)lParam.ToInt64();
        if (trayMessage == NativeMethods.WM_LBUTTONDBLCLK)
        {
            _dispatcher.TryEnqueue(() => _showSettings());
            return true;
        }

        if (trayMessage == NativeMethods.WM_RBUTTONUP)
        {
            ShowContextMenu();
            return true;
        }

        return true;
    }

    private void ShowContextMenu()
    {
        var menu = NativeMethods.CreatePopupMenu();
        if (menu == IntPtr.Zero)
        {
            return;
        }

        try
        {
            var toggleText = _getState().IsEnabled ? "Pause F1 pinning" : "Enable F1 pinning";
            NativeMethods.AppendMenu(menu, NativeMethods.MF_STRING, CommandToggle, toggleText);
            NativeMethods.AppendMenu(menu, NativeMethods.MF_STRING, CommandSettings, "Settings");
            NativeMethods.AppendMenu(menu, NativeMethods.MF_SEPARATOR, 0, null);
            NativeMethods.AppendMenu(menu, NativeMethods.MF_STRING, CommandQuit, "Quit PinShot");

            if (!NativeMethods.GetCursorPos(out var point))
            {
                return;
            }

            NativeMethods.SetForegroundWindow(_window.Handle);
            var command = NativeMethods.TrackPopupMenu(
                menu,
                NativeMethods.TPM_RETURNCMD | NativeMethods.TPM_RIGHTBUTTON,
                point.X,
                point.Y,
                0,
                _window.Handle,
                IntPtr.Zero);

            DispatchCommand(command);
        }
        finally
        {
            NativeMethods.DestroyMenu(menu);
        }
    }

    private void DispatchCommand(uint command)
    {
        switch (command)
        {
            case CommandToggle:
                _dispatcher.TryEnqueue(() => _toggleEnabled());
                break;
            case CommandSettings:
                _dispatcher.TryEnqueue(() => _showSettings());
                break;
            case CommandQuit:
                _dispatcher.TryEnqueue(() => _exit());
                break;
        }
    }

    private NativeMethods.NotifyIconData CreateData(uint flags)
    {
        return new NativeMethods.NotifyIconData
        {
            cbSize = (uint)Marshal.SizeOf<NativeMethods.NotifyIconData>(),
            hWnd = _window.Handle,
            uID = TrayIconId,
            uFlags = flags,
            uCallbackMessage = CallbackMessage
        };
    }

    private static IntPtr LoadTrayIcon()
    {
        var path = IconService.GetThemeAwareIconPath();
        if (!File.Exists(path))
        {
            return IntPtr.Zero;
        }

        return NativeMethods.LoadImage(
            IntPtr.Zero,
            path,
            NativeMethods.IMAGE_ICON,
            0,
            0,
            NativeMethods.LR_LOADFROMFILE | NativeMethods.LR_DEFAULTSIZE);
    }

    private static string Limit(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
