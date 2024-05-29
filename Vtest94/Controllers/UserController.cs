using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vtest94.Data;
using Vtest94.Interfaces;
using Vtest94.Models;

namespace Vtest94.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public UserController(IUserRepository userRepository, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager, AppDbContext context)
        {
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> PublicProfile(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public async Task<IActionResult> PrivateProfile(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId);

                var userPhotoDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "UserPhotos", userId);
                Directory.CreateDirectory(userPhotoDirectory);
                var userPhotoPath = Path.Combine(userPhotoDirectory, "profile.jpg");

                // Delete existing profile photo if it exists
                if (System.IO.File.Exists(userPhotoPath))
                {
                    System.IO.File.Delete(userPhotoPath);
                }

                using (var fileStream = new FileStream(userPhotoPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var userPhoto = await _context.UserPhotos.FirstOrDefaultAsync(up => up.UserId == userId);
                if (userPhoto == null)
                {
                    userPhoto = new UserPhoto
                    {
                        FileName = "profile.jpg",
                        FilePath = $"/UserPhotos/{userId}/profile.jpg",
                        UserId = userId
                    };
                    _context.UserPhotos.Add(userPhoto);
                }
                else
                {
                    userPhoto.FilePath = $"/UserPhotos/{userId}/profile.jpg";
                }

                await _context.SaveChangesAsync();

                user.UserPhoto = userPhoto;
                await _userManager.UpdateAsync(user);

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
}
