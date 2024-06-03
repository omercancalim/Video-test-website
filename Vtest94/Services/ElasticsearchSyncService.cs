using Microsoft.EntityFrameworkCore;
using Nest;
using Vtest94.Data;

namespace Vtest94.Services
{
    public class ElasticsearchSyncService
    {
        private readonly IElasticClient _elasticClient;
        private readonly AppDbContext _context;
        private readonly ILogger<ElasticsearchSyncService> _logger; // Add logger

        public ElasticsearchSyncService(IElasticClient elasticClient, AppDbContext context, ILogger<ElasticsearchSyncService> logger)
        {
            _elasticClient = elasticClient;
            _context = context;
            _logger = logger;
        }

        public async Task SyncVideosAsync()
        {
            var videos = await _context.Videos.ToListAsync();
            foreach (var video in videos)
            {
                var esDocument = new
                {
                    id = video.Id,
                    title = video.Title,
                    description = video.Description,
                    fileName = video.FileName,
                    thumbnailName = video.ThumbnailName,
                    duration = video.Duration.ToString(),  // Store as string
                    uploadedDate = video.UploadedDate,
                    userId = video.UserId,
                    categoryId = video.CategoryId,
                    videoStatsId = video.VideoStatsId
                };
                var response = await _elasticClient.IndexDocumentAsync(esDocument);
                _logger.LogInformation($"Indexed video: {video.Title}, Success: {response.IsValid}"); // Log indexing status

                //await _elasticClient.IndexDocumentAsync(video);
            }
        }
    }
}
