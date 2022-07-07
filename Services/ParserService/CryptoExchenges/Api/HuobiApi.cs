using System.Globalization;
using System.Text;
using System.Text.Json;

namespace P2PCryptoScaner.Services.ParserService.CryptoExchenges.Api
{
    public class HuobiApi : CryptoExchenge
    {
        public HuobiApi(HttpClient client) : base(client, "Huobi")
        {

        }

        private readonly Dictionary<P2PayType, string> AvailablePayTypes = new Dictionary<P2PayType, string>()
        {
            { P2PayType.Sberbank, "29" },
            { P2PayType.Tinkoff, "28" },
            { P2PayType.Alfabank, "25" },
            { P2PayType.Qiwi, "9"}
        };

        private readonly Dictionary<CryptoCurrency, string> AvailableCryptoCurrency = new Dictionary<CryptoCurrency, string>()
        {
            { CryptoCurrency.USDT, "2" },
            { CryptoCurrency.BTC, "1" },
            { CryptoCurrency.ETH, "3" }
        };

        private string ConvertPayType(P2PayType payType)
        {
            return AvailablePayTypes[payType];
        }

        private string ConvertCryptoCurrency(CryptoCurrency cryptoCurrency)
        {
            return AvailableCryptoCurrency[cryptoCurrency];
        }

        public override async Task<List<P2POrder>> GetP2POrderBook(CryptoCurrency cryptoCurrency, P2PayType payType, P2POrderType orderType)
        {
            var url = "https://otc-api.bitderiv.com/v1/data/trade-market?" +
                "coinId=" + ConvertCryptoCurrency(cryptoCurrency) +
                "&currency=11" + //currency 11 = fiat RUB
                "&tradeType=" + (orderType == P2POrderType.Sell ? "sell" : "buy") +
                "&currPage=1" +
                "&payMethod=" + ConvertPayType(payType) +
                "&acceptOrder=" + (orderType == P2POrderType.Sell ? "0" : "-1") +
                "&country=" +
                "&blockType=general" +
                "&online=1" +
                "&range=0" +
                "&amount=" +
                "&onlyTradable=false";

            var response = await _client.GetAsync(url);

            var result = await response.Content.ReadAsStringAsync();

            HuobiOrderBook orderbook = JsonSerializer.Deserialize<HuobiOrderBook>(result);

            return GetP2POrdersData(orderbook.data, ExchengeName, cryptoCurrency, payType, orderType, ConvertHuobiOrder);
        }

        private P2POrder ConvertHuobiOrder(HuobiOrder huobiOrder)
        {
            return ConvertExchangeOrder(huobiOrder,
                huobiOrder => huobiOrder.price,
                huobiOrder => huobiOrder.minTradeLimit,
                huobiOrder => huobiOrder.maxTradeLimit,
                huobiOrder => huobiOrder.tradeCount
            );
        }

        private class HuobiOrderBook
        {
            public int code { get; set; }
            public List<HuobiOrder> data { get; set; }
            public int currPage { get; set; }
            public string message { get; set; }
            public int pageSize { get; set; }
            public bool success { get; set; }
            public int totalCount { get; set; }
            public int totalPage { get; set; }
        }

        private class HuobiOrder
        {
            public int blockType { get; set; }
            public int coinId { get; set; }
            public int currency { get; set; } 
            public long gmtSort { get; set; } 
            public int id { get; set; } 
            public bool isCopyBlock { get; set; } 
            public bool isFollowed { get; set; } 
            public bool isOnline { get; set; } 
            public string maxTradeLimit { get; set; } 
            public int merchantLevel { get; set; } 
            public object merchantTags { get; set; } 
            public string minTradeLimit { get; set; } 
            public string orderCompleteRate { get; set; } 
            public string payMethod { get; set; } 
            public object payMethods { get; set; } 
            public string payName { get; set; } 
            public int payTerm { get; set; } 
            public string price { get; set; } 
            public string seaViewRoom { get; set; } 
            public string takerAcceptAmount { get; set; } 
            public int takerAcceptOrder { get; set; } 
            public int takerLimit { get; set; } 
            public int thumbUp { get; set; } 
            public string tradeCount { get; set; } 
            public int tradeMonthTimes { get; set; } 
            public int tradeType { get; set; } 
            public long uid { get; set; } 
            public string userName { get; set; } 
        }
    }
}
