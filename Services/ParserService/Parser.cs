using Microsoft.EntityFrameworkCore;
using P2PCryptoScaner.Services.ParserService.CryptoExchenges;
using P2PCryptoScaner.Services.ParserService.CryptoExchenges.Api;
using System.Collections.Concurrent;

namespace P2PCryptoScaner.Services.ParserService
{
    public class Parser
    {
        private readonly List<CryptoExchenge> _cryptoExchenges;
        private readonly List<CryptoCurrency> _cryptoCurrencies;
        private readonly List<P2PayType> _payTypes;
        private readonly Dictionary<string, Thread> _threadPool = new Dictionary<string, Thread>();
        private  CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public readonly Dictionary<string, Dictionary<CryptoCurrency, Dictionary<P2PayType, Dictionary<P2POrderType, List<P2POrder>>>>> OrderTree = new Dictionary<string, Dictionary<CryptoCurrency, Dictionary<P2PayType, Dictionary<P2POrderType, List<P2POrder>>>>>();

        public Parser() 
        {
            HttpClient httpClient = new HttpClient();
            
            //initialize parameters for parsing
            _cryptoExchenges = new List<CryptoExchenge>
            {
                new BinanceApi(httpClient),
                new HuobiApi(httpClient),
                new OkxApi(httpClient)
            };

            _cryptoCurrencies = Enum.GetValues(typeof(CryptoCurrency))
                .Cast<CryptoCurrency>()
                .ToList();

            _payTypes = Enum.GetValues(typeof(P2PayType))
                .Cast<P2PayType>()
                .ToList();

            InitializeOrderThree();

            //start parse when service will be start
            StartParse();
        }

        
        public void StartParse() 
        {
            InitializeThreads();
            StartThreads();
        }
        
        public void StopParse()
        {
            AbortThreads();
        }

        private async void ParseExchangeData(CryptoExchenge exchenge, CancellationTokenSource cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Thread {0} is aborted", Thread.CurrentThread);

                    return;
                }

                foreach (var cryptoCurrency in _cryptoCurrencies)
                {
                    foreach (var payType in _payTypes)
                    {
                        var sellOrders = await exchenge.GetP2POrderBook(cryptoCurrency, payType, P2POrderType.Sell);
                        Thread.Sleep(10000);
                        var buyOrders = await exchenge.GetP2POrderBook(cryptoCurrency, payType, P2POrderType.Buy);
                        Thread.Sleep(10000);

                        OrderTree[exchenge.ExchengeName][cryptoCurrency][payType][P2POrderType.Sell] = sellOrders.GetRange(0, sellOrders.Count > 3? 3 : sellOrders.Count);
                        OrderTree[exchenge.ExchengeName][cryptoCurrency][payType][P2POrderType.Buy] = buyOrders.GetRange(0, buyOrders.Count > 3 ? 3 : buyOrders.Count);
                    }
                }
            }
        }
        
        private void StartThreads()
        {
            foreach (var thread in _threadPool.Values)
            {
                thread.Start();
            }
        }

        private void InitializeThreads()
        {
            AbortThreads();

            foreach (var cryptoExchenge in _cryptoExchenges)
            {
                _threadPool[cryptoExchenge.ExchengeName] = new Thread(() => ParseExchangeData(cryptoExchenge, _cancellationToken));
            }
        }

        private void AbortThreads()
        {
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            
            _cancellationToken = new CancellationTokenSource();
        }

        private void InitializeOrderThree()
        {
            foreach (var cryptoExchenge in _cryptoExchenges)
            {
                OrderTree.Add(cryptoExchenge.ExchengeName, new Dictionary<CryptoCurrency, Dictionary<P2PayType, Dictionary<P2POrderType, List<P2POrder>>>>());

                foreach (var cryptoCurrency in _cryptoCurrencies)
                {
                    OrderTree[cryptoExchenge.ExchengeName].Add(cryptoCurrency, new Dictionary<P2PayType, Dictionary<P2POrderType, List<P2POrder>>>());
                    foreach (var payType in _payTypes)
                    {
                        OrderTree[cryptoExchenge.ExchengeName][cryptoCurrency].Add(payType, new Dictionary<P2POrderType, List<P2POrder>>());

                        OrderTree[cryptoExchenge.ExchengeName][cryptoCurrency][payType].Add(P2POrderType.Sell, new List<P2POrder>());
                        OrderTree[cryptoExchenge.ExchengeName][cryptoCurrency][payType].Add(P2POrderType.Buy, new List<P2POrder>());
                    }
                }
            }
        }
    }
}
