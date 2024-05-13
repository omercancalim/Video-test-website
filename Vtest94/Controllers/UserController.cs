using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vtest94.Interfaces;
using Vtest94.Models;

namespace Vtest94.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Profile(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
    }
}
