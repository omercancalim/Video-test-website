﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nest;
using System.Security.Claims;
using Vtest94.Data;
using Vtest94.Interfaces;
using Vtest94.Models;
using Vtest94.Services;
using Vtest94.ViewModels;

namespace Vtest94.Controllers
{
    public class VideoController : Controller
    {
        private readonly IUploadVideoRepository _videoRepository;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly VideoSearchService _videoSearchService;
        public VideoController(IUploadVideoRepository videoRepository, 
            UserManager<User> userManager, 
            AppDbContext appDbContext,
            VideoSearchService videoSearchService)
        {
            _videoRepository = videoRepository;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _videoSearchService = videoSearchService;
        }

        [HttpGet] // Actually [HttpGet] is set by default we dont have to wirte it explicitly
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var videos = await _videoRepository.GetAllVideosAsync(searchString);
            return View(videos);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchVideo(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var videos = await _videoSearchService.SearchVideosAsync(searchString);

            return View(videos);
        }

        // Display the form for uploading a new video
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = await _appDbContext.Categories.OrderBy(c => c.Name).ToListAsync();
            var model = new CreateVideoViewModel
            {
                Categories = new SelectList(categories, "Id", "Name")  // Populate the SelectList for categories
            };
            return View(model);
        }

        // Process the form submission for a new video
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(200_000_000)]
        [Authorize]
        public async Task<IActionResult> Create(CreateVideoViewModel videoViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage)
                                   .ToList();
                return BadRequest(new { Errors = errors });
                //return View(videoViewModel);
            }

            var user = await _userManager.GetUserAsync(User);  // Get the authenticated user
            if (user == null)
            {
                ModelState.AddModelError("", "User not found. Please login again.");
                return View(videoViewModel);
            }

            if (videoViewModel.VideoFile != null && videoViewModel.VideoFile.Length > 0)
            {
                videoViewModel.Video.UserId = user.Id;  // Set the UserId to the logged user's Id
                await _videoRepository.AddVideoAsync(videoViewModel.Video, videoViewModel.VideoFile, videoViewModel.ThumbnailFrameTime, videoViewModel.SelectedCategoryId.Value);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("VideoFile", "Please select a video file to upload.");
                return View(videoViewModel);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CategoryVideos(int categoryId, string categoryName)
        {
            var videos = await _videoRepository.GetVideosByCategoryIdAsync(categoryId);

            // ViewBag.CategoryName is set to the categoryName parameter directly
            ViewBag.CategoryName = categoryName ?? "Category";

            return View(videos);
        }

        // New method for fetching videos dynamically
        [HttpGet]
        [AllowAnonymous]
        [Route("api/videos")]
        public async Task<IActionResult> GetVideos(string searchString = null, int page = 1, int pageSize = 4)
        {
            if(searchString == "latest")
            {
                var latestVideos = await _videoRepository.GetAllLatestVideosAsync();
                var totalVideos = latestVideos.Count();
                var paginatedVideos = latestVideos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(new { TotalCount = totalVideos, Videos = paginatedVideos });
            }
            else
            {
                var videos = await _videoRepository.GetAllVideosAsync(searchString);
                var totalVideos = videos.Count();
                var paginatedVideos = videos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(new { TotalCount = totalVideos, Videos = paginatedVideos });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/videos/game")]
        public async Task<IActionResult> GetGameVideos(int page = 1, int pageSize = 4)
        {
            var sportVideos = await _videoRepository.GetVideosByCategoryIdAsync(3);
            var totalVideos = sportVideos.Count();
            var paginatedVideos = sportVideos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(new { TotalCount = totalVideos, Videos = paginatedVideos });
        }

        [AllowAnonymous]
        public async Task<IActionResult> SelectedVideo(int videoId)
        {
            var selectedVideo = await _videoRepository.GetVideoAndUserByIdAsync(videoId);

            if (selectedVideo == null)
            {
                return NotFound();
            }

            return View(selectedVideo);
        }

        [HttpPost]
        public async Task<IActionResult> IncrementViewCount(int videoId)
        {
            var video = await _appDbContext.Videos.Include(v => v.VideoStats).FirstOrDefaultAsync(v => v.Id == videoId);
            if (video == null)
            {
                return NotFound();
            }

            video.VideoStats.ViewCount++;
            await _appDbContext.SaveChangesAsync();

            return Ok(new { success = true });

            //return Ok(new { success = false, message = "View already counted" });
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int videoId)
        {
            using (var transaction = await _appDbContext.Database.BeginTransactionAsync())
            {
                var videoStats = _appDbContext.VideoStats.SingleOrDefault(v => v.VideoId == videoId);

                try
                {
                    var userId = GetUserId();
                    var existingAction = _appDbContext.UserLikes.SingleOrDefault(ua => ua.VideoId == videoId && ua.UserId == userId && ua.IsLike);

                    var existingDislikeAction = _appDbContext.UserLikes.SingleOrDefault(ua => ua.VideoId == videoId && ua.UserId == userId && !ua.IsLike);

                    if (existingAction != null)
                    {
                        _appDbContext.UserLikes.Remove(existingAction);
                    }
                    else
                    {
                        if (existingDislikeAction != null)
                        {
                            _appDbContext.UserLikes.Remove(existingDislikeAction);
                        }
                        _appDbContext.UserLikes.Add(new UserLikes { UserId = userId, VideoId = videoId, IsLike = true });
                    }

                    _appDbContext.SaveChanges();

                    //throw new Exception("Simulated error to test transaction rollback.");
                    //await transaction.RollbackAsync();

                    if (videoStats != null)
                    {
                        videoStats.LikeCount = _appDbContext.UserLikes.Count(ul => ul.VideoId == videoId && ul.IsLike);
                        videoStats.DislikeCount = _appDbContext.UserLikes.Count(ul => ul.VideoId == videoId && !ul.IsLike);
                        _appDbContext.SaveChanges();
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    //return Json(new { likeCount = videoStats.LikeCount, dislikeCount = videoStats.DislikeCount });
                    return StatusCode(500, new { message = "Internal server error", likeCount = videoStats?.LikeCount ?? 0, dislikeCount = videoStats?.DislikeCount ?? 0 });
                }

                return Json(new { likeCount = videoStats?.LikeCount ?? 0, dislikeCount = videoStats?.DislikeCount ?? 0 });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ToggleDislike(int videoId)
        {
            using (var transaction = await _appDbContext.Database.BeginTransactionAsync())
            {
                var videoStats = _appDbContext.VideoStats.SingleOrDefault(v => v.VideoId == videoId);

                try
                {
                    var userId = GetUserId();
                    var existingAction = _appDbContext.UserLikes.SingleOrDefault(ua => ua.VideoId == videoId && ua.UserId == userId && !ua.IsLike);

                    var existingLikeAction = _appDbContext.UserLikes.SingleOrDefault(ua => ua.VideoId == videoId && ua.UserId == userId && ua.IsLike);

                    if (existingAction != null)
                    {
                        _appDbContext.UserLikes.Remove(existingAction);
                    }
                    else
                    {
                        if (existingLikeAction != null)
                        {
                            _appDbContext.UserLikes.Remove(existingLikeAction);
                        }
                        _appDbContext.UserLikes.Add(new UserLikes { UserId = userId, VideoId = videoId, IsLike = false });
                    }

                    _appDbContext.SaveChanges();

                    //throw new Exception("Simulated error to test transaction rollback.");

                    if (videoStats != null)
                    {
                        videoStats.LikeCount = _appDbContext.UserLikes.Count(ul => ul.VideoId == videoId && ul.IsLike);
                        videoStats.DislikeCount = _appDbContext.UserLikes.Count(ul => ul.VideoId == videoId && !ul.IsLike);
                        _appDbContext.SaveChanges();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    //return Json(new { likeCount = videoStats.LikeCount, dislikeCount = videoStats.DislikeCount });
                    return StatusCode(500, new { message = "Internal server error", likeCount = videoStats?.LikeCount ?? 0, dislikeCount = videoStats?.DislikeCount ?? 0 });
                }

                return Json(new { likeCount = videoStats?.LikeCount ?? 0, dislikeCount = videoStats?.DislikeCount ?? 0 });
            }
        }
    }
}
