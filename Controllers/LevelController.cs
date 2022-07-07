using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P2PCryptoScaner.Data;
using P2PCryptoScaner.Models;
using P2PCryptoScaner.Services.LevelService;
using P2PCryptoScaner.ViewModels;

namespace P2PCryptoScaner.Controllers
{

    public class LevelController : Controller
    {
        private readonly ILogger<LevelController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly LevelManager _levelManager;

        public LevelController(ILogger<LevelController> logger, UserManager<User> userManager, LevelManager levelManager)
        {
            _logger = logger;
            _userManager = userManager;
            _levelManager = levelManager;
        }

        // GET: LevelController
        [AllowAnonymous]
        public async  Task <ActionResult> Index()
        {
            string userId = _userManager.GetUserId(this.User);
            User? user = await _userManager.FindByIdAsync(userId);

            LevelListViewModel model = new LevelListViewModel();

            if (user is not null)
            {
                model.UserLevelId = user.LevelId;
            }
            model.AllLevels = _levelManager.GetAllLevels();

            return View(model);
        }

        // GET: LevelController/Buy/5
        [Authorize(Roles = "user")]
        public ActionResult Buy(int id)
        {
            Level? level = _levelManager.GetLevelById(id);
            if (level == null)
                return RedirectToAction(nameof(Index));
            return View(level);
        }

        // POST: LevelController/Buy/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "user")]
        public async Task<ActionResult> Buy(int id, IFormCollection collection)
        {
            Level? level = _levelManager.GetLevelById(id);

            string userId = _userManager.GetUserId(this.User);
            User user = await _userManager.FindByIdAsync(userId);

            if (level is null || user is null)
                return RedirectToAction(nameof(Index));
            
            try
            {
                _levelManager.BuyLevelForUser(level, user);
                
                _logger.LogInformation("User bought level.");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LevelController/Create
        [Authorize(Roles = "moderator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: LevelController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "moderator")]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                Level level = LevelFromForm(collection);

                _levelManager.AddLevel(level);

                _logger.LogInformation("Moderator created a new level.");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LevelController/Edit/5
        [Authorize(Roles = "moderator")]
        public ActionResult Edit(int id)
        {
            Level? level = _levelManager.GetLevelById(id);
            if (level == null)
                return RedirectToAction(nameof(Index));
            return View(level);
        }

        // POST: LevelController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "moderator")]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            Level? level = _levelManager.GetLevelById(id);
            if (level == null)
                return RedirectToAction(nameof(Index));

            try
            {
                Level editLevel = LevelFromForm(collection);
                
                _levelManager.UpdateLevel(level, editLevel);

                _logger.LogInformation("Moderator edited level.");

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View(level);
            }

        }

        // GET: LevelController/Delete/5
        [Authorize(Roles = "moderator")]
        public ActionResult Delete(int id)
        {
            Level? level = _levelManager.GetLevelById(id);
            if (level == null)
                return RedirectToAction(nameof(Index));
            return View(level);
        }

        // POST: LevelController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "moderator")]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            Level? level = _levelManager.GetLevelById(id);
            if (level == null)
                return RedirectToAction(nameof(Index));

            try
            {
                _levelManager.RemoveLevel(level);
                
                _logger.LogInformation("Moderator deleted level.");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private Level LevelFromForm(IFormCollection form) 
        {
            return new Level()
            {
                Name = form["Name"],
                Price = Convert.ToUInt32(form["Price"]),
                Duration = TimeSpan.FromDays(Convert.ToUInt32(form["Duration"])),
                IsDeactivated = Convert.ToBoolean(form["IsDeactivated"][0]),
                Description = form["Description"]
            };
        }
    }
}
