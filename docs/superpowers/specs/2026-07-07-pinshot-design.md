# PinShot Design

## Goal

PinShot is a tiny WinUI 3 companion for the built-in Windows 11 snipping flow. It does not capture the screen. It reads the current clipboard image when the user presses F1 and creates a polished always-on-top pinned image window.

## Product Requirements

- Global hotkey: F1.
- Manual enable or pause from the tray menu.
- If the clipboard does not contain an image, show a quiet tray balloon and do not open a window.
- Each F1 press creates a new pin from the current clipboard image.
- Pins are always-on-top by default.
- Pins support drag, mouse wheel zoom, Ctrl+wheel opacity, Ctrl+C copy, double-click close, and a right-click menu.
- No lock mode, mouse passthrough, annotation, or game detection in V1.
- Visual style must feel modern and native to Windows 11: rounded corners, subtle border, soft shadow, compact hover chrome.
- Icon style is monochrome and minimal. Use dark icon on light theme and light icon on dark theme.

## Architecture

- `PinShot.Core` contains pure logic: settings models, scale and opacity rules, hotkey enabled state, and pin state transforms.
- `PinShot.App` contains WinUI 3 UI, Win32 interop, clipboard access, tray menu, global hotkey registration, and pin windows.
- `PinShot.Core.Tests` verifies the pure behavior before UI wiring.

## Components

- `PinShotState`: stores whether F1 pinning is enabled.
- `PinTransform`: clamps scale and opacity changes.
- `AppSettings`: stores hotkey and visual defaults.
- `HotkeyService`: registers F1 using Win32 `RegisterHotKey` and dispatches events to the UI thread.
- `ClipboardImageService`: reads and writes clipboard images.
- `PinWindow`: borderless rounded WinUI 3 window that displays one image.
- `TrayService`: owns tray icon, pause/resume menu, and quit action.
- `IconService`: provides theme-aware monochrome tray icon assets.

## Testing

Automated tests cover core state and transform rules. UI behavior is verified by building the app and manually exercising F1, dragging, zooming, opacity, Ctrl+C, double-click close, and the tray pause switch.
