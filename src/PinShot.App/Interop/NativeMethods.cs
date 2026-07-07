using System.Runtime.InteropServices;

namespace PinShot.App.Interop;

internal static partial class NativeMethods
{
    internal const int WM_HOTKEY = 0x0312;
    internal const uint WM_APP = 0x8000;
    internal const uint WM_LBUTTONDBLCLK = 0x0203;
    internal const uint WM_RBUTTONUP = 0x0205;

    internal const int HWND_TOPMOST = -1;
    internal const int HWND_NOTOPMOST = -2;
    internal const int HWND_MESSAGE = -3;

    internal const uint SWP_NOMOVE = 0x0002;
    internal const uint SWP_NOSIZE = 0x0001;
    internal const uint SWP_NOACTIVATE = 0x0010;
    internal const uint SWP_NOZORDER = 0x0004;
    internal const uint SWP_ASYNCWINDOWPOS = 0x4000;

    internal const uint WM_NCLBUTTONDOWN = 0x00A1;
    internal const int HTCAPTION = 2;

    internal const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;

    internal const uint IMAGE_ICON = 1;
    internal const uint LR_LOADFROMFILE = 0x0010;
    internal const uint LR_DEFAULTSIZE = 0x0040;

    internal const uint NIF_MESSAGE = 0x00000001;
    internal const uint NIF_ICON = 0x00000002;
    internal const uint NIF_TIP = 0x00000004;
    internal const uint NIF_INFO = 0x00000010;

    internal const uint NIM_ADD = 0x00000000;
    internal const uint NIM_MODIFY = 0x00000001;
    internal const uint NIM_DELETE = 0x00000002;
    internal const uint NIM_SETVERSION = 0x00000004;

    internal const uint NOTIFYICON_VERSION_4 = 4;

    internal const uint MF_STRING = 0x00000000;
    internal const uint MF_SEPARATOR = 0x00000800;
    internal const uint TPM_RIGHTBUTTON = 0x0002;
    internal const uint TPM_RETURNCMD = 0x0100;

    internal delegate IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnregisterHotKey(IntPtr hWnd, int id);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [LibraryImport("dwmapi.dll", SetLastError = true)]
    internal static partial int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr GetModuleHandle(string? lpModuleName);

    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW", SetLastError = true)]
    internal static partial IntPtr DefWindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(IntPtr hwnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyIcon(IntPtr hIcon);

    [LibraryImport("user32.dll", EntryPoint = "LoadImageW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial IntPtr CreatePopupMenu();

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyMenu(IntPtr hMenu);

    [LibraryImport("user32.dll", EntryPoint = "AppendMenuW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AppendMenu(IntPtr hMenu, uint uFlags, nuint uIDNewItem, string? lpNewItem);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetCursorPos(out Point lpPoint);

    [LibraryImport("user32.dll")]
    internal static partial int GetDpiForWindow(IntPtr hwnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ReleaseCapture();

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetForegroundWindow(IntPtr hWnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial uint TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

    [DllImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern ushort RegisterClassEx(ref WndClassEx lpwcx);

    [DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern IntPtr CreateWindowEx(
        int dwExStyle,
        string lpClassName,
        string lpWindowName,
        int dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShellNotifyIcon(uint dwMessage, ref NotifyIconData lpData);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WndClassEx
    {
        internal uint cbSize;
        internal uint style;
        internal WndProc lpfnWndProc;
        internal int cbClsExtra;
        internal int cbWndExtra;
        internal IntPtr hInstance;
        internal IntPtr hIcon;
        internal IntPtr hCursor;
        internal IntPtr hbrBackground;
        internal string? lpszMenuName;
        internal string lpszClassName;
        internal IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct NotifyIconData
    {
        internal uint cbSize;
        internal IntPtr hWnd;
        internal uint uID;
        internal uint uFlags;
        internal uint uCallbackMessage;
        internal IntPtr hIcon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string szTip;

        internal uint dwState;
        internal uint dwStateMask;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        internal string szInfo;

        internal uint uTimeoutOrVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        internal string szInfoTitle;

        internal uint dwInfoFlags;
        internal Guid guidItem;
        internal IntPtr hBalloonIcon;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Point
    {
        internal int X;
        internal int Y;
    }
}
