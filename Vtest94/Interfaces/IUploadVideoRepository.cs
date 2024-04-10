﻿using Vtest94.Models;

namespace Vtest94.Interfaces
{
    public interface IUploadVideoRepository
    {
        Task<Video> AddVideoAsync(Video video, IFormFile videoFile, int thumbnailFrameTime);
        Task<IEnumerable<Video>> GetAllVideosAsync(string searchString = null);
        Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId);
        Task<Video> GetVideoByIdAsync(int id);
    }
}