namespace Larp.Common;

public record Image(string ContentType, byte[] Data);

public interface IImageModifier
{
    Task<Image> GenerateWebp(byte[] data);
    Task<Image> GenerateWebpThumbnail(int maxWidth, int maxHeight, byte[] data);
}