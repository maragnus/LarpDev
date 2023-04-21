using SkiaSharp;

namespace Larp.Landing.Server.Services;

public class ImageModifier : IImageModifier
{
    public Task<Image> GenerateWebp(byte[] data)
    {
        using var image = SKImage.FromEncodedData(data);
        using var bitmap = SKBitmap.FromImage(image);
        var thumbData = bitmap.Encode(SKEncodedImageFormat.Webp, 80);
        return Task.FromResult(new Image("image/webp", thumbData.ToArray()));
    }
    
    public Task<Image> GenerateWebpThumbnail(int maxWidth, int maxHeight, byte[] data)
    {
        using var originalImage = SKImage.FromEncodedData(data);
        using var originalBitmap = SKBitmap.FromImage(originalImage);
        var size = CalculateResizedDimensions(originalBitmap.Width, originalBitmap.Height, maxWidth, maxHeight);
        var thumbBitmap = originalBitmap.Resize(size, SKFilterQuality.High);
        var thumbData = thumbBitmap.Encode(SKEncodedImageFormat.Webp, 80);
        return Task.FromResult(new Image("image/webp", thumbData.ToArray()));
    }

    private static SKSizeI CalculateResizedDimensions(int imageWidth, int imageHeight, int desiredWidth,
        int desiredHeight)
    {
        var widthScale = (double)desiredWidth / imageWidth;
        var heightScale = (double)desiredHeight / imageHeight;
        var scale = widthScale < heightScale ? widthScale : heightScale;
        return new SKSizeI((int)(scale * imageWidth), (int)(scale * imageHeight));
    }
}