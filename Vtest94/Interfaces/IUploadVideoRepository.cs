using Vtest94.Models;

namespace Vtest94.Interfaces
{
    public interface IUploadVideoRepository
    {
        Task<Video> AddVideoAsync(Video video, IFormFile videoFile, int thumbnailFrameTime, int categoryId);
        Task<IEnumerable<Video>> GetAllVideosAsync(string searchString = null);
        Task<Video> GetVideoAndUserByIdAsync(int id);
        Task<IEnumerable<Video>> GetAllLatestVideosAsync();
        Task<Video> GetVideoByIdAsync(int id);
        Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId);
        Task<IEnumerable<Video>> GetVideosByCategoryIdAsync(int categoryId);
    }
}
