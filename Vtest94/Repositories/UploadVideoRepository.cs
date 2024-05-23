using Microsoft.EntityFrameworkCore;
using Vtest94.Data;
using Vtest94.Interfaces;
using Vtest94.Models;
using Xabe.FFmpeg;

namespace Vtest94.Repositories
{
    public class UploadVideoRepository : IUploadVideoRepository
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IVideoProcessing _videoProcessing;
        public UploadVideoRepository(AppDbContext context, IWebHostEnvironment hostingEnvironment, IVideoProcessing videoProcessing)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _videoProcessing = videoProcessing;
        }
        public async Task<Video> AddVideoAsync(Video video, IFormFile videoFile, int thumbnailFrameTime, int categoryId)
        {
            if (videoFile != null && videoFile.Length > 0)
            {
                var (fileName, filePath) = await SaveFileAsync(videoFile);
                video.FileName = fileName;
                video.UploadedDate = DateTime.UtcNow;

                var mediaInfo = await FFmpeg.GetMediaInfo(filePath);
                video.Duration = mediaInfo.Duration;

                // New implementation using VideoProcessing
                var thumbnailsDirectoryPath = Path.Combine(_hostingEnvironment.WebRootPath, "thumbnails");
                var thumbnailFileName = $"thumbnail_{Guid.NewGuid()}.jpg";
                var thumbnailFilePath = Path.Combine(thumbnailsDirectoryPath, thumbnailFileName);
                var thumbnailPath = await _videoProcessing.ExtractThumbnailAsync(filePath, thumbnailFilePath, TimeSpan.FromSeconds(thumbnailFrameTime));

                video.ThumbnailName = thumbnailFileName;

                video.CategoryId = categoryId;
                // Assuming other properties like Title and Description are already set on the Video object
                _context.Videos.Add(video);
                await _context.SaveChangesAsync();
            }

            return video;
        }

        private async Task<(string FileName, string FilePath)> SaveFileAsync(IFormFile file)
        {
            var uploadsDirectoryPath = Path.Combine(_hostingEnvironment.WebRootPath, "videos");
            if (!Directory.Exists(uploadsDirectoryPath))
            {
                Directory.CreateDirectory(uploadsDirectoryPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsDirectoryPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return (uniqueFileName, filePath); // Return the file name to be saved in the database
        }

        public async Task<IEnumerable<Video>> GetAllVideosAsync(string searchString = null)
        {
            IQueryable<Video> query = _context.Videos;

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(v => EF.Functions.Like(v.Title, $"%{searchString}%"));

            }

            return await query.ToListAsync();

            // return await _context.Videos.ToListAsync();
        }

        public async Task<IEnumerable<Video>> GetAllLatestVideosAsync()
        {
            IQueryable<Video> query = _context.Videos;

            query = query.OrderByDescending(v => v.UploadedDate);

            return await query.ToListAsync();
        }

        public async Task<Video> GetVideoByIdAsync(int id)
        {
            return await _context.Videos.FindAsync(id);
        }

        public async Task<Video> GetVideoAndUserByIdAsync(int id)
        {
            return await _context.Videos
                                  .Include(v => v.User)
                                  .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId)
        {
            return await _context.Videos.Where(v => v.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Video>> GetVideosByCategoryIdAsync(int categoryId)
        {
            return await _context.Videos.Where(v => v.CategoryId == categoryId).ToListAsync();
        }
    }
}
