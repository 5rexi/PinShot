# PinShot Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a WinUI 3 clipboard-image pinning utility triggered by F1.

**Architecture:** Keep UI-independent behavior in `PinShot.Core`, with WinUI 3 and Win32 interop in `PinShot.App`. Tests exercise pure state transitions and transform clamping.

**Tech Stack:** C# 12, .NET 8, WinUI 3, Windows App SDK, xUnit.

## Global Constraints

- Project root: `C:\Users\hanasaku\Desktop\PinShot`.
- Global hotkey is F1 by default.
- V1 has only manual pause, no game detection.
- Pinned images are always-on-top by default.
- Double-click closes the pin.
- Ctrl+C copies the pin image.
- Visual design must match modern Windows 11 styling.

---

### Task 1: Core Logic

**Files:**
- Create: `src/PinShot.Core/PinShotState.cs`
- Create: `src/PinShot.Core/PinTransform.cs`
- Create: `src/PinShot.Core/AppSettings.cs`
- Modify: `tests/PinShot.Core.Tests/UnitTest1.cs`

**Interfaces:**
- Produces: `PinShotState.ToggleEnabled()`, `PinTransform.Zoom(double, int)`, `PinTransform.AdjustOpacity(double, int)`, `AppSettings.Default`.

- [ ] Write tests for pause toggling, zoom clamping, opacity clamping, and default settings.
- [ ] Run tests and verify they fail because the types do not exist.
- [ ] Implement the minimal core classes.
- [ ] Run tests and verify they pass.

### Task 2: WinUI Shell and Tray

**Files:**
- Modify: `src/PinShot.App/PinShot.App.csproj`
- Modify: `src/PinShot.App/App.xaml.cs`
- Create: `src/PinShot.App/Services/TrayService.cs`
- Create: `src/PinShot.App/Services/HotkeyService.cs`
- Create: `src/PinShot.App/Interop/NativeMethods.cs`

**Interfaces:**
- Consumes: `PinShotState`.
- Produces: tray pause/resume menu and F1 event callback.

- [ ] Add Windows Forms support for tray icon hosting.
- [ ] Implement a Win32 hotkey message window.
- [ ] Wire tray pause/resume to `PinShotState`.
- [ ] Build the app.

### Task 3: Clipboard and Pin Window

**Files:**
- Create: `src/PinShot.App/Models/ClipboardImage.cs`
- Create: `src/PinShot.App/Services/ClipboardImageService.cs`
- Create: `src/PinShot.App/Views/PinWindow.xaml`
- Create: `src/PinShot.App/Views/PinWindow.xaml.cs`
- Modify: `src/PinShot.App/App.xaml.cs`

**Interfaces:**
- Consumes: `PinTransform`.
- Produces: one always-on-top image window per F1 press.

- [ ] Read bitmap bytes from the clipboard.
- [ ] Render the bytes in a borderless rounded WinUI window.
- [ ] Implement drag, zoom, Ctrl+wheel opacity, double-click close, Ctrl+C copy, and right-click menu.
- [ ] Build and manually test interactions.

### Task 4: Polish and Verification

**Files:**
- Create: `src/PinShot.App/Assets/PinShotLight.ico`
- Create: `src/PinShot.App/Assets/PinShotDark.ico`
- Create: `README.md`

**Interfaces:**
- Consumes: tray and pin window services.
- Produces: documented run instructions and theme-aware tray icon assets.

- [ ] Generate minimalist monochrome icons.
- [ ] Switch tray icon based on app theme.
- [ ] Run `dotnet test`.
- [ ] Run `dotnet build`.
- [ ] Document how to run with the portable SDK path.

## Self-Review

- Spec coverage: F1, manual pause, multi-pin, drag, zoom, opacity, double-click close, Ctrl+C, right-click menu, WinUI 3, and Windows 11 visual style are covered.
- Placeholder scan: no TBD or TODO markers.
- Type consistency: core type names match the implementation tasks.
