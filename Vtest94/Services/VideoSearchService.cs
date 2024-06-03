using Nest;
using Vtest94.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Vtest94.Services
{
    public class VideoSearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<VideoSearchService> _logger;

        public VideoSearchService(IElasticClient elasticClient, ILogger<VideoSearchService> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        public async Task<List<Video>> SearchVideosAsync(string searchString)
        {
            var searchResponse = await _elasticClient.SearchAsync<Video>(s => s
            .Source(src => src
                .Includes(i => i
                    .Fields(
                        f => f.Id,
                        f => f.Title,
                        f => f.Description,
                        f => f.FileName,
                        f => f.ThumbnailName,
                        f => f.Duration,
                        f => f.UploadedDate,
                        f => f.UserId,
                        f => f.CategoryId,
                        f => f.VideoStatsId
                    )
                )
            )
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field("title")
                        .Field("description"))
                    .Query(searchString)
                )
            )
        );

            if (!searchResponse.IsValid)
            {
                // Handle errors
                var debugInfo = searchResponse.DebugInformation;
                var error = searchResponse.ServerError.Error;
            }

            _logger.LogInformation($"Search query: {searchString}, Hits: {searchResponse.Hits.Count}"); // Log search query and results
            return searchResponse.Documents.ToList();
        }
    }
}
