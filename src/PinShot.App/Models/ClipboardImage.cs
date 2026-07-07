namespace PinShot.App.Models;

public sealed record ClipboardImage(byte[] Bytes, uint PixelWidth, uint PixelHeight);
