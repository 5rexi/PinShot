using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using PinShot.App.Interop;
using PinShot.App.Models;
using PinShot.App.Services;
using PinShot.Core;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using WinRT.Interop;

namespace PinShot.App.Views;

public sealed partial class PinWindow : Window
{
    private readonly ClipboardImage _image;
    private readonly AppSettings _settings;
    private bool _dragging;
    private bool _topmost = true;
    private PointInt32 _windowStart;
    private NativeMethods.Point _cursorStart;
    private double _scale;
    private double _opacity;

    public PinWindow(ClipboardImage image, AppSettings settings)
    {
        _image = image;
        _settings = settings;
        _opacity = settings.DefaultOpacity;
        _scale = 1.0;

        InitializeComponent();

        AppWindow.SetIcon("Assets/AppIcon.ico");
        ConfigureWindow();
        _ = LoadImageAsync();
    }

    private void ConfigureWindow()
    {
        var presenter = OverlappedPresenter.Create();
        presenter.SetBorderAndTitleBar(false, false);
        presenter.IsResizable = false;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        AppWindow.SetPresenter(presenter);

        var hwnd = WindowNative.GetWindowHandle(this);
        var dpi = NativeMethods.GetDpiForWindow(hwnd);
        var scaleFactor = dpi / 96.0;

        var width = (int)Math.Round(_image.PixelWidth * _scale * scaleFactor) + 2;
        var height = (int)Math.Round(_image.PixelHeight * _scale * scaleFactor) + 2;
        AppWindow.Resize(new SizeInt32(width, height));

        ApplyTopmost();
        ApplyRoundedCorners();
    }

    private async Task LoadImageAsync()
    {
        var bitmap = new BitmapImage();
        var stream = new InMemoryRandomAccessStream();
        var writer = new DataWriter(stream);
        writer.WriteBytes(_image.Bytes);
        await writer.StoreAsync();
        await writer.FlushAsync();
        stream.Seek(0);
        await bitmap.SetSourceAsync(stream);

        PinnedImage.Source = bitmap;
        Root.Focus(FocusState.Programmatic);
    }

    private void Root_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(Root);
        if (!point.Properties.IsLeftButtonPressed)
        {
            return;
        }

        _dragging = true;
        NativeMethods.GetCursorPos(out _cursorStart);
        _windowStart = AppWindow.Position;
        Root.CapturePointer(e.Pointer);
        Root.Focus(FocusState.Pointer);
    }

    private void Root_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_dragging)
        {
            return;
        }

        NativeMethods.GetCursorPos(out var cursorPos);
        var dx = cursorPos.X - _cursorStart.X;
        var dy = cursorPos.Y - _cursorStart.Y;
        var hwnd = WindowNative.GetWindowHandle(this);
        NativeMethods.SetWindowPos(
            hwnd,
            IntPtr.Zero,
            _windowStart.X + dx,
            _windowStart.Y + dy,
            0,
            0,
            NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_ASYNCWINDOWPOS);
    }

    private void Root_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        _dragging = false;
        Root.ReleasePointerCapture(e.Pointer);
    }

    private void Root_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var delta = e.GetCurrentPoint(Root).Properties.MouseWheelDelta;
        var ctrl = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
            .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        if (ctrl)
        {
            _opacity = PinTransform.AdjustOpacity(_opacity, delta);
            Root.Opacity = _opacity;
            return;
        }

        _scale = PinTransform.Zoom(_scale, delta);
        ResizeToScale();
    }

    private async void Root_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        var ctrl = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
            .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        if (ctrl && e.Key == VirtualKey.C)
        {
            await ClipboardImageService.CopyToClipboardAsync(_image);
            e.Handled = true;
        }
    }

    private void Root_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        Close();
    }

    private void Root_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        var menu = new MenuFlyout();

        var copy = new MenuFlyoutItem { Text = "Copy image" };
        copy.Click += async (_, _) => await ClipboardImageService.CopyToClipboardAsync(_image);

        var save = new MenuFlyoutItem { Text = "Save as..." };
        save.Click += async (_, _) => await SaveAsAsync();

        var reset = new MenuFlyoutItem { Text = "Reset size" };
        reset.Click += (_, _) =>
        {
            _scale = 1.0;
            ResizeToScale();
        };

        var topmost = new MenuFlyoutItem { Text = _topmost ? "Turn off always on top" : "Keep always on top" };
        topmost.Click += (_, _) =>
        {
            _topmost = !_topmost;
            ApplyTopmost();
        };

        var close = new MenuFlyoutItem { Text = "Close" };
        close.Click += (_, _) => Close();

        menu.Items.Add(copy);
        menu.Items.Add(save);
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(reset);
        menu.Items.Add(topmost);
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(close);
        menu.ShowAt(Root, e.GetPosition(Root));
    }

    private void ResizeToScale()
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        var dpi = NativeMethods.GetDpiForWindow(hwnd);
        var scaleFactor = dpi / 96.0;

        var width = Math.Max(120, (int)Math.Round(_image.PixelWidth * _scale * scaleFactor)) + 2;
        var height = Math.Max(80, (int)Math.Round(_image.PixelHeight * _scale * scaleFactor)) + 2;
        AppWindow.Resize(new SizeInt32(width, height));
    }

    private async Task SaveAsAsync()
    {
        var picker = new FileSavePicker
        {
            SuggestedFileName = $"pinshot-{DateTime.Now:yyyyMMdd-HHmmss}"
        };
        picker.FileTypeChoices.Add("PNG image", [".png"]);
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));

        StorageFile? file = await picker.PickSaveFileAsync();
        if (file is null)
        {
            return;
        }

        await FileIO.WriteBytesAsync(file, _image.Bytes);
    }

    private void ApplyTopmost()
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        NativeMethods.SetWindowPos(
            hwnd,
            new IntPtr(_topmost ? NativeMethods.HWND_TOPMOST : NativeMethods.HWND_NOTOPMOST),
            0,
            0,
            0,
            0,
            NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);
    }

    private void ApplyRoundedCorners()
    {
        var preference = 2;
        NativeMethods.DwmSetWindowAttribute(
            WindowNative.GetWindowHandle(this),
            NativeMethods.DWMWA_WINDOW_CORNER_PREFERENCE,
            ref preference,
            Marshal.SizeOf<int>());
    }
}
