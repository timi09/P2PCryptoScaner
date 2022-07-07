using Microsoft.AspNetCore.Identity;

namespace P2PCryptoScaner.Models
{
    public class User : IdentityUser
    {
        public DateTime RegistrationDate { get; set; }
        public int? LevelId { get; set; }
        public Level? Level { get; set; }
        public List<LevelPurchase> PurchaseHistory { get; set; } = new List<LevelPurchase>();
        
    }
}
