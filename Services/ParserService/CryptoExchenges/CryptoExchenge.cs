using System.Globalization;

namespace P2PCryptoScaner.Services.ParserService.CryptoExchenges
{
    public enum CryptoCurrency { USDT, BTC, ETH };

    public abstract class CryptoExchenge
    {
        private protected HttpClient _client;
        public readonly string ExchengeName;
        public CryptoExchenge(HttpClient client, string exchengeName)
        {
            _client = client;
            ExchengeName = exchengeName;
        }

        public abstract Task<List<P2POrder>> GetP2POrderBook(CryptoCurrency cryptoCurrency, P2PayType payType, P2POrderType orderType);

        private protected delegate P2POrder ConvertOrderAction<ExchangeOrderType>(ExchangeOrderType exchangeOrder);

        private protected delegate object GetOrderProperty<ExchangeOrderType>(ExchangeOrderType exchangeOrder);

        private protected List<P2POrder> GetP2POrdersData<ExchangeOrderType>(List<ExchangeOrderType> orderList, string exchangeName, CryptoCurrency cryptoCurrency, P2PayType payType, P2POrderType orderType, ConvertOrderAction<ExchangeOrderType> convertAction)
        {
            List<P2POrder> p2pOrders = new List<P2POrder>();
            foreach (var exchangeOrder in orderList)
            {
                P2POrder order = convertAction(exchangeOrder);
                order.ExchengeName = exchangeName;
                order.CryptoCurrency = cryptoCurrency;
                order.PayType = payType;
                order.Type = orderType;
                p2pOrders.Add(order);
            }

            return p2pOrders;
        }

        private protected P2POrder ConvertExchangeOrder<ExchangeOrderType>(ExchangeOrderType exchangeOrder, GetOrderProperty<ExchangeOrderType> getPrice, GetOrderProperty<ExchangeOrderType> getMinAmount, GetOrderProperty<ExchangeOrderType> getMaxAmount, GetOrderProperty<ExchangeOrderType> getMaxQuantity)
        {
            P2POrder order = new P2POrder();
            order.Price = Convert.ToDouble(getPrice(exchangeOrder), CultureInfo.InvariantCulture);
            order.MinAmount = Convert.ToDouble(getMinAmount(exchangeOrder), CultureInfo.InvariantCulture);
            order.MaxAmount = Convert.ToDouble(getMaxAmount(exchangeOrder), CultureInfo.InvariantCulture);
            order.MaxQuantity = Convert.ToDouble(getMaxQuantity(exchangeOrder), CultureInfo.InvariantCulture);

            return order;
        }

    }
}
