using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using P2PCryptoScaner.Data;
using P2PCryptoScaner.Models;
using System.Security.Claims;

namespace P2PCryptoScaner.Services.LevelService
{
    public class LevelManager
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public LevelManager(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public List<Level> GetAllLevels() 
        {
            return _applicationDbContext.Levels.ToList();
        }

        public Level? GetLevelById(int id)
        {
            return _applicationDbContext.Levels.Find(id);
        }

        public Level? GetLevelByName(string name)
        {
            return _applicationDbContext.Levels.FirstOrDefault(l => l.Name == name);
        }

        public LevelPurchase MakeLevelPurchase(Level level)
        {
            return new LevelPurchase()
            {
                Level = level,
                LevelId = level.Id,
                LevelPrice = level.Price,
                LevelDuration = level.Duration,
                LevelEndTime = DateTime.Now.Add(level.Duration),
                PurchaseTime = DateTime.Now
            };
        }

        public void BuyLevelForUser(Level level, User user)
        {
            LevelPurchase levelPurchase = MakeLevelPurchase(level);

            user.LevelId = levelPurchase.LevelId;
            user.PurchaseHistory.Add(levelPurchase);

            _applicationDbContext.Users.Update(user);
            _applicationDbContext.SaveChanges();
        }

        public Level? GetUserLevel(User user) 
        {
            User? dbUser = (User?)_applicationDbContext.Users.Include(u => ((User)u).Level)
                    .FirstOrDefault(u => u.Id == user.Id);
            if (dbUser is null || dbUser.Level is null)
                return null;

            LevelPurchase? lastLevelPurchase = _applicationDbContext.LevelPurchases
               .Where(lp => lp.UserId == user.Id)
               .Include(lp => lp.Level)
               .OrderByDescending(lp => lp.PurchaseTime)
               .FirstOrDefault();

            if (lastLevelPurchase is null || lastLevelPurchase.Level.IsDeactivated)
                return null;

            if (lastLevelPurchase.LevelEndTime < DateTime.Now)
            {
                dbUser.LevelId = null;
                _applicationDbContext.Users.Update(dbUser);
                _applicationDbContext.SaveChanges();
                return null;
            }

            return lastLevelPurchase.Level;
        }

        public void AddLevel(Level level) 
        {
            _applicationDbContext.Levels.Add(level);
            _applicationDbContext.SaveChanges();
        }

        public void UpdateLevel(Level level, Level editLevel)
        {
            level.Name = editLevel.Name;
            level.Price = editLevel.Price;
            level.Duration = editLevel.Duration;
            level.IsDeactivated = editLevel.IsDeactivated;
            level.Description = editLevel.Description;

            _applicationDbContext.Levels.Update(level);
            _applicationDbContext.SaveChanges();
        }

        public void RemoveLevel(Level level)
        {
            _applicationDbContext.Levels.Remove(level);
            _applicationDbContext.SaveChanges();
        }
    }
}
