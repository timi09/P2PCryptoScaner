namespace P2PCryptoScaner.Services.ParserService.CryptoExchenges
{
    public enum P2PayType { Sberbank, Tinkoff, Alfabank, Qiwi };
    public enum P2POrderType { Sell, Buy };

    public class P2POrder
    {
        public string ExchengeName { get; set; }
        public CryptoCurrency CryptoCurrency { get; set; }
        public P2PayType PayType { get; set; }
        public P2POrderType Type { get; set; }
        public double Price { get; set; }
        public double MinAmount { get; set; }
        public double MaxAmount { get; set; }
        public double MaxQuantity { get; set; }
    }
}
