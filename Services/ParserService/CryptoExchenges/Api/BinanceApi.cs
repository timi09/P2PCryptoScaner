using System.Globalization;
using System.Text;
using System.Text.Json;

namespace P2PCryptoScaner.Services.ParserService.CryptoExchenges.Api
{
    public class BinanceApi : CryptoExchenge
    {
        public BinanceApi(HttpClient client) : base(client, "Binance")
        {

        }

        private readonly Dictionary<P2PayType, string> AvailablePayTypes = new Dictionary<P2PayType, string>()
        {
            { P2PayType.Sberbank, "RosBank" },
            { P2PayType.Tinkoff, "Tinkoff" },
            { P2PayType.Alfabank, "ABank" },
            { P2PayType.Qiwi, "QIWI"}
        };

        private string ConvertPayType(P2PayType payType)
        {
            return AvailablePayTypes[payType];
        }

        public override async Task<List<P2POrder>> GetP2POrderBook(CryptoCurrency cryptoCurrency, P2PayType payType, P2POrderType orderType)
        {
            var data = new StringContent("{\"page\":1," +
                                         "\"rows\":10," +
                                         "\"payTypes\":[\"" + ConvertPayType(payType) + "\"]," +
                                         "\"publisherType\":null," +
                                         "\"asset\":\"" + cryptoCurrency.ToString() + "\"," +
                                         "\"tradeType\":\"" + (orderType == P2POrderType.Sell ? "BUY" : "SELL") + "\"," +
                                         "\"fiat\":\"RUB\"}",
            Encoding.UTF8, "application/json");

            var url = "https://p2p.binance.com/bapi/c2c/v2/friendly/c2c/adv/search";

            var response = await _client.PostAsync(url, data);

            var result = await response.Content.ReadAsStringAsync();

            BinanceOrderBook orderbook = JsonSerializer.Deserialize<BinanceOrderBook>(result);

            return GetP2POrdersData(orderbook.data, ExchengeName, cryptoCurrency, payType, orderType, ConvertBinanceOrder);
        }

        private P2POrder ConvertBinanceOrder(BinanceOrder binanceOrder)
        {
            return ConvertExchangeOrder(binanceOrder,
                binanceOrder => binanceOrder.adv.price,
                binanceOrder => binanceOrder.adv.minSingleTransAmount,
                binanceOrder => binanceOrder.adv.dynamicMaxSingleTransAmount,
                binanceOrder => binanceOrder.adv.tradableQuantity
            );

        }

        private class BinanceOrderBook
        {
            public string code { get; set; }
            public List<BinanceOrder> data { get; set; }
            public string message { get; set; }
            public string messageDetail { get; set; }
            public bool success { get; set; }
            public int total { get; set; }
        }

        private class BinanceOrder
        {
            public BinanceAdvert adv { get; set; }
            public object advertiser { get; set; }
        }

        private class BinanceAdvert
        {
            public string advNo { get; set; } 
            public string asset { get; set; } 
            public int assetScale { get; set; } 
            public string classify { get; set; } 
            public string commissionRate { get; set; } 
            public string dynamicMaxSingleTransAmount { get; set; } 
            public string dynamicMaxSingleTransQuantity { get; set; } 
            public int fiatScale { get; set; } 
            public string fiatSymbol { get; set; } 
            public string fiatUnit { get; set; } 
            public string initAmount { get; set; } 
            public bool isTradable { get; set; } 
            public string maxSingleTransAmount { get; set; } 
            public string maxSingleTransQuantity { get; set; } 
            public string minSingleTransAmount { get; set; } 
            public string minSingleTransQuantity { get; set; } 
            public object payTimeLimit { get; set; } 
            public string price { get; set; } 
            public int priceScale { get; set; } 
            public string surplusAmount { get; set; } 
            public string tradableQuantity { get; set; } 
            public object tradeMethods { get; set; }
            public string tradeType { get; set; } 
        }

    }

}
