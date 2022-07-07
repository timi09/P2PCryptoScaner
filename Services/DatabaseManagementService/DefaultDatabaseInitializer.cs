using Microsoft.AspNetCore.Identity;
using P2PCryptoScaner.Models;
using P2PCryptoScaner.Services.LevelService;

namespace P2PCryptoScaner.Services.Initializer
{
    public class DefaultDatabaseInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly LevelManager _levelManager;
        public DefaultDatabaseInitializer(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, LevelManager levelManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _levelManager = levelManager;
        }

        public async Task InitializeAsync()
        {
            string adminEmail = "admin@gmail.com";
            string password = "_Aa123456";

            //initialize default roles
            if (await _roleManager.FindByNameAsync("admin") is null)
            {
                await _roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await _roleManager.FindByNameAsync("moderator") is null)
            {
                await _roleManager.CreateAsync(new IdentityRole("moderator"));
            }
            if (await _roleManager.FindByNameAsync("user") is null)
            {
                await _roleManager.CreateAsync(new IdentityRole("user"));
            }

            //initialize default admin
            if (await _userManager.FindByNameAsync(adminEmail) is null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail, EmailConfirmed = true };
                IdentityResult result = await _userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(admin, new string[]{"admin", "moderator", "user" });
                }
            }

            //initialize default levels
            if (_levelManager.GetLevelByName("Silver") is null)
            {
                _levelManager.AddLevel(new Level() 
                {
                    Name = "Silver",
                    Price = 100,
                    Duration = TimeSpan.FromDays(30),
                    IsDeactivated = false,
                    Description = "Standard acces level.\n"
                                +"- Available exchenges: Binance\n"
                                +"- Available payments: Sberbank, Tinkoff, Alfa Bank, Qiwi\n"
                                +"- Available cryptocurrency: USDT"

                });
            }

            if (_levelManager.GetLevelByName("Gold") is null)
            {
                _levelManager.AddLevel(new Level()
                {
                    Name = "Gold",
                    Price = 200,
                    Duration = TimeSpan.FromDays(30),
                    IsDeactivated = false,
                    Description = "Extended access level. Added more cryptocurrencies.\n"
                                + "- Available exchanges: Binance\n"
                                + "- Available payments: Sberbank, Tinkoff, Alfa Bank, Qiwi\n"
                                + "- Available cryptocurrency: USDT, BTC, ETH"

                });
            }

            if (_levelManager.GetLevelByName("Diamond") is null)
            {
                _levelManager.AddLevel(new Level()
                {
                    Name = "Diamond",
                    Price = 300,
                    Duration = TimeSpan.FromDays(30),
                    IsDeactivated = false,
                    Description = "Max access level. Added more cryptocurrencies. Added new crypto exchenges.\n"
                                + "- Available exchanges: Binance, Huobi, Okx\n"
                                + "- Available payments: Sberbank, Tinkoff, Alfa Bank, Qiwi\n"
                                + "- Available cryptocurrency: USDT, BTC, ETH"

                });
            }
        }
    }
}
