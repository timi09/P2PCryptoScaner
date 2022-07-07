using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P2PCryptoScaner.Data;
using P2PCryptoScaner.Models;
using P2PCryptoScaner.Services.LevelService;
using System.Diagnostics;

namespace P2PCryptoScaner.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly LevelManager _levelManager;

        public HomeController(UserManager<User> userManager, LevelManager levelManager)
        {
            _userManager = userManager;
            _levelManager = levelManager;
        }

        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(this.User);
            Level? userLevel = _levelManager.GetUserLevel(new User { Id = userId });
            return View(userLevel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}