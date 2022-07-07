using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2PCryptoScaner.Data;
using P2PCryptoScaner.Models;
using P2PCryptoScaner.Services.LevelService;
using P2PCryptoScaner.Services.ParserService;
using P2PCryptoScaner.Services.ParserService.CryptoExchenges;
using System.Text.Json;

namespace P2PCryptoScaner.Controllers
{
    [Route("api/p2p")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly LevelManager _levelManager;
        private readonly Parser _parser;

        public ApiController(UserManager<User> userManager, LevelManager levelManager, Parser parser)
        {
            _userManager = userManager;
            _levelManager = levelManager;
            _parser = parser;
        }

        // GET: api/p2p
        [HttpGet]
        public string Get()
        {
            string userId = _userManager.GetUserId(this.User);
            Level? userLevel = _levelManager.GetUserLevel(new User { Id = userId });

            if (userLevel is null)
                return null;

            if (userLevel.Name == "Diamond") 
            {
                return JsonSerializer.Serialize(_parser.OrderTree);
            }
            else if(userLevel.Name == "Gold")
            {
                return "{\"Binance\":" + JsonSerializer.Serialize(_parser.OrderTree["Binance"]) + "}";
            }
            else // silver 
            {
                return "{\"Binance\":{\"USDT\":" + JsonSerializer.Serialize(_parser.OrderTree["Binance"][CryptoCurrency.USDT]) + "}}";
            }

        }

        // GET api/<P2PController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<P2PController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<P2PController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<P2PController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
