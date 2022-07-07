namespace P2PCryptoScaner.Models
{
    public class Level
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public uint Price { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsDeactivated { get; set; }
        public string Description { get; set; }
        public List<LevelPurchase> LevelPurchases { get; set; }
    }
}
