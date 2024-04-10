using Vtest94.Interfaces;
using Xabe.FFmpeg;

namespace Vtest94.Utilities
{
    public class VideoProcessing : IVideoProcessing
    {
        public async Task<string> ExtractThumbnailAsync(string videoPath, string outputPath, TimeSpan timeFrame)
        {
            try
            {
                // Ensure the output directory exists
                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                // Generate the thumbnail
                IConversion conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(videoPath, outputPath, timeFrame);
                await conversion.Start();

                return outputPath;
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or throw)
                Console.WriteLine($"An error occurred while extracting a thumbnail: {ex.Message}");
                return null;
            }
        }
    }
}
