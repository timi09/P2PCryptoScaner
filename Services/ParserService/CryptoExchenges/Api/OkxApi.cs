using System.Globalization;
using System.Text.Json;

namespace P2PCryptoScaner.Services.ParserService.CryptoExchenges.Api
{
    public class OkxApi : CryptoExchenge
    {
        public OkxApi(HttpClient client) : base(client, "Okx")
        {

        }

        private readonly Dictionary<P2PayType, string> AvailablePayTypes = new Dictionary<P2PayType, string>()
        {
            { P2PayType.Sberbank, "Sberbank" },
            { P2PayType.Tinkoff, "Tinkoff" },
            { P2PayType.Alfabank, "Alfa Bank" },
            { P2PayType.Qiwi, "QiWi"}
        };

        private string ConvertPayType(P2PayType payType)
        {
            return AvailablePayTypes[payType];
        }

        public override async Task<List<P2POrder>> GetP2POrderBook(CryptoCurrency cryptoCurrency, P2PayType payType, P2POrderType orderType)
        {
            var url = "https://www.okx.com/v3/c2c/tradingOrders/books?" +
                "t=" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() + //tick time (1656059399086)
                "&quoteCurrency=RUB" +
                "&baseCurrency=" + cryptoCurrency.ToString() +
                "&side=" + (orderType == P2POrderType.Sell ? "sell" : "buy") +
                "&paymentMethod=" + ConvertPayType(payType) +
                "&userType=all" +
                "&showTrade=false" +
                "&showFollow=false" +
                "&showAlreadyTraded=false" +
                "&isAbleFilter=false";

            var response = await _client.GetAsync(url);

            var result = await response.Content.ReadAsStringAsync();

            OkxOrderBook orderbook = JsonSerializer.Deserialize<OkxOrderBook>(result);

            List<OkxOrder> okexOrders;
            if (orderType == P2POrderType.Sell)
                okexOrders = orderbook.data.sell;
            else
                okexOrders = orderbook.data.buy;

            return GetP2POrdersData(okexOrders, ExchengeName, cryptoCurrency, payType, orderType, ConvertOkxOrder);
        }

        private P2POrder ConvertOkxOrder(OkxOrder okxOrder)
        {
            return ConvertExchangeOrder(okxOrder,
                okxOrder => okxOrder.price,
                okxOrder => okxOrder.quoteMinAmountPerOrder,
                okxOrder => okxOrder.quoteMaxAmountPerOrder,
                okxOrder => okxOrder.availableAmount
            );
        }

        private class OkxOrderBook
        {
            public int code { get; set; }
            public OkxOrders data { get; set; }
            public string detailMsg { get; set; }
            public string error_code { get; set; }
            public string error_message { get; set; }
            public string msg { get; set; }
        }

        private class OkxOrders
        {
            public List<OkxOrder> buy { get; set; }
            public List<OkxOrder> sell { get; set; }
            
        }

        private class OkxOrder
        {
            public bool alreadyTraded { get; set; } 
            public string availableAmount { get; set; } 
            public string baseCurrency { get; set; } 
            public bool black { get; set; } 
            public int cancelledOrderQuantity { get; set; } 
            public int completedOrderQuantity { get; set; } 
            public string completedRate { get; set; } 
            public string creatorType { get; set; } 
            public bool guideUpgradeKyc { get; set; } 
            public string id { get; set; } 
            public bool intention { get; set; } 
            public int maxCompletedOrderQuantity { get; set; } 
            public object maxUserCreatedDate { get; set; } 
            public string merchantId { get; set; } 
            public int minCompletedOrderQuantity { get; set; } 
            public string minCompletionRate { get; set; } 
            public int minKycLevel { get; set; } 
            public int minSellOrders { get; set; } 
            public bool mine { get; set; } 
            public string nickName { get; set; } 
            public object paymentMethods { get; set; } 
            public string price { get; set; } 
            public string publicUserId { get; set; } 
            public string quoteCurrency { get; set; } 
            public string quoteMaxAmountPerOrder { get; set; }
            public string quoteMinAmountPerOrder { get; set; } 
            public int quoteScale { get; set; } 
            public string quoteSymbol { get; set; } 
            public bool receivingAds { get; set; } 
            public bool safetyLimit { get; set; } 
            public string side { get; set; } 
            public string userType { get; set; } 

        }
    }
}
