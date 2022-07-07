namespace P2PCryptoScaner.Models
{
    public class LevelPurchase
    {
        public int Id { get; set; }
        public uint LevelPrice { get; set; }
        public TimeSpan LevelDuration { get; set; }
        public DateTime PurchaseTime { get; set; }
        public DateTime LevelEndTime { get; set; }

        public int LevelId { get; set; }
        public Level Level { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
