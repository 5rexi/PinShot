# PinShot

PinShot is a small WinUI 3 companion app for pinning clipboard images on screen.

## Run

The debug build output is here:

```powershell
C:\Users\hanasaku\Desktop\PinShot\src\PinShot.App\bin\x64\Debug\net8.0-windows10.0.26100.0\win-x64\PinShot.App.exe
```

To rebuild from source with the portable SDK used for this project:

```powershell
$dotnet = 'C:\Users\hanasaku\Documents\Codex\2026-07-06\wo-xi\work\.dotnet-sdk\dotnet.exe'
& $dotnet restore C:\Users\hanasaku\Desktop\PinShot\PinShot.sln -r win-x64
& $dotnet build C:\Users\hanasaku\Desktop\PinShot\PinShot.sln --no-restore
```

## Controls

- `F1`: create a pinned image from the current clipboard image.
- Drag a pinned image window to move it.
- Mouse wheel over a pinned image to resize it.
- `Ctrl + mouse wheel` over a pinned image to adjust opacity.
- `Ctrl + C` while a pinned image is focused to copy that pinned image back to the clipboard.
- Double-click a pinned image to close it.
- Right-click a pinned image for copy, save, and close actions.
- Use the main window switch or tray menu to pause or enable F1 pinning.

## Notes

- Pin windows are topmost by default.
- The tray icon switches between black and white icon files based on the Windows app theme.
- The app is configured as an unpackaged, self-contained WinUI 3 app so it can run without separately installing the Windows App Runtime.
