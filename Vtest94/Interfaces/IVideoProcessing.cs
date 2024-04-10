namespace Vtest94.Interfaces
{
    public interface IVideoProcessing
    {
        Task<string> ExtractThumbnailAsync(string videoPath, string outputPath, TimeSpan timeFrame);
    }
}
