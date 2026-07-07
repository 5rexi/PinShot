using PinShot.App.Models;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace PinShot.App.Services;

public sealed class ClipboardImageService
{
    public async Task<ClipboardImage?> TryGetImageAsync()
    {
        var content = Clipboard.GetContent();
        if (!content.Contains(StandardDataFormats.Bitmap))
        {
            return null;
        }

        var bitmapReference = await content.GetBitmapAsync();
        await using var stream = (await bitmapReference.OpenReadAsync()).AsStreamForRead();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        var bytes = memory.ToArray();

        var randomAccessStream = new InMemoryRandomAccessStream();
        var writer = new DataWriter(randomAccessStream);
        writer.WriteBytes(bytes);
        await writer.StoreAsync();
        await writer.FlushAsync();
        randomAccessStream.Seek(0);

        var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
        return new ClipboardImage(bytes, decoder.PixelWidth, decoder.PixelHeight);
    }

    public static async Task CopyToClipboardAsync(ClipboardImage image)
    {
        var stream = new InMemoryRandomAccessStream();
        var writer = new DataWriter(stream);
        writer.WriteBytes(image.Bytes);
        await writer.StoreAsync();
        await writer.FlushAsync();
        stream.Seek(0);

        var package = new DataPackage();
        package.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
        Clipboard.SetContent(package);
    }
}
