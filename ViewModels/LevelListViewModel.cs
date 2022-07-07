using Microsoft.AspNetCore.Identity;
using P2PCryptoScaner.Models;

namespace P2PCryptoScaner.ViewModels
{
    public class LevelListViewModel
    {
        public int? UserLevelId { get; set; }
        public IList<Level> AllLevels { get; set; }
    }
}
