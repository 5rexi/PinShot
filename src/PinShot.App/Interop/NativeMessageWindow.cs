using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace PinShot.App.Interop;

internal sealed class NativeMessageWindow : IDisposable
{
    private static readonly ConcurrentDictionary<IntPtr, NativeMessageWindow> Windows = new();
    private static readonly NativeMethods.WndProc SharedWndProc = WndProc;
    private static int _classRegistered;

    private readonly Func<uint, IntPtr, IntPtr, bool> _messageHandler;
    private bool _disposed;

    public NativeMessageWindow(string name, Func<uint, IntPtr, IntPtr, bool> messageHandler)
    {
        _messageHandler = messageHandler;
        RegisterClass();

        Handle = NativeMethods.CreateWindowEx(
            0,
            ClassName,
            name,
            0,
            0,
            0,
            0,
            0,
            new IntPtr(NativeMethods.HWND_MESSAGE),
            IntPtr.Zero,
            NativeMethods.GetModuleHandle(null),
            IntPtr.Zero);

        if (Handle == IntPtr.Zero)
        {
            throw new InvalidOperationException($"Could not create native message window. Win32 error: {Marshal.GetLastWin32Error()}");
        }

        Windows[Handle] = this;
    }

    public IntPtr Handle { get; }

    private static string ClassName => "PinShotNativeMessageWindow";

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Windows.TryRemove(Handle, out _);
        NativeMethods.DestroyWindow(Handle);
    }

    private static void RegisterClass()
    {
        if (Interlocked.Exchange(ref _classRegistered, 1) == 1)
        {
            return;
        }

        var wndClass = new NativeMethods.WndClassEx
        {
            cbSize = (uint)Marshal.SizeOf<NativeMethods.WndClassEx>(),
            lpfnWndProc = SharedWndProc,
            hInstance = NativeMethods.GetModuleHandle(null),
            lpszClassName = ClassName
        };

        var atom = NativeMethods.RegisterClassEx(ref wndClass);
        if (atom == 0)
        {
            throw new InvalidOperationException($"Could not register native window class. Win32 error: {Marshal.GetLastWin32Error()}");
        }
    }

    private static IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (Windows.TryGetValue(hwnd, out var window) && window._messageHandler(msg, wParam, lParam))
        {
            return IntPtr.Zero;
        }

        return NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
    }
}
