// The MIT License (MIT)
// 
// Copyright (c) 2014 Jared Schroeder (jarschroe)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace Bitgold
{
    public enum BgCurrency
    {
        // referred to http://www.xe.com/iso4217.php for currency codes
        AUD
    }

    public class BgApiController
    {
        static string BlockchainBaseUrl = "https://blockchain.info/";

        public string SubmitTransaction(BgTransaction transaction)
        {
            // convert value to Bitcoin
            float bitcoinValue = CurrencyToBitcoin(BgCurrency.AUD, transaction.Value);
            // convert value to Satoshi
            float bitcoinSatoshi = bitcoinValue * 100000000;

            // try transaction
            // API transaction information at https://blockchain.info/api/blockchain_wallet_api
            string requestUrl = BlockchainBaseUrl + "merchant/" + transaction.Player.PrivateKey + "/payment?" + "to=" + transaction.Player.Address + "&amount=" + bitcoinSatoshi.ToString() + "&from=" + transaction.Developer.Address;
            WebRequest request = WebRequest.Create(requestUrl);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();

            // TODO: handle errors
            return result;
        }

        // TODO: value to Bitcoin (current function), or currency to Bitcoin (i.e. ret=currencyInBitcoin, args=(currency))?
        public float CurrencyToBitcoin(BgCurrency currency, float value)
        {
            // TODO: validate
            // WebRequest code referenced from http://msdn.microsoft.com/en-us/library/456dfw4f(v=vs.110).aspx
            // request the currency conversion
            // API currency conversion information at https://blockchain.info/api/exchange_rates_api
            WebRequest request = WebRequest.Create(BlockchainBaseUrl + "tobtc?currency=" + currency.ToString() + "&value=" + value.ToString());
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            // get the response
            float result = float.Parse(reader.ReadToEnd());
            return result;
        }

        public BgTransaction TestJSON(BgTransaction transaction)
        {
            string json = JsonConvert.SerializeObject(transaction, Formatting.Indented);
            BgTransaction fromJson = JsonConvert.DeserializeObject<BgTransaction>(json);
            return fromJson;
        }
    }
}
