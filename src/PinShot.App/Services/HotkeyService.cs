using Microsoft.UI.Dispatching;
using PinShot.App.Interop;

namespace PinShot.App.Services;

public sealed class HotkeyService : IDisposable
{
    private const int HotkeyId = 0x5053;
    private const uint VirtualKeyF1 = 0x70;

    private readonly DispatcherQueue _dispatcher;
    private readonly NativeMessageWindow _window;
    private bool _registered;

    public HotkeyService(DispatcherQueue dispatcher)
    {
        _dispatcher = dispatcher;
        _window = new NativeMessageWindow("PinShotHotkeyWindow", OnMessage);
    }

    public event EventHandler? HotkeyPressed;

    public bool RegisterF1()
    {
        if (_registered)
        {
            return true;
        }

        _registered = NativeMethods.RegisterHotKey(_window.Handle, HotkeyId, 0, VirtualKeyF1);
        return _registered;
    }

    public void Dispose()
    {
        if (_registered)
        {
            NativeMethods.UnregisterHotKey(_window.Handle, HotkeyId);
            _registered = false;
        }

        _window.Dispose();
    }

    private bool OnMessage(uint message, IntPtr wParam, IntPtr lParam)
    {
        if (message != NativeMethods.WM_HOTKEY || wParam.ToInt32() != HotkeyId)
        {
            return false;
        }

        _dispatcher.TryEnqueue(() => HotkeyPressed?.Invoke(this, EventArgs.Empty));
        return true;
    }
}
