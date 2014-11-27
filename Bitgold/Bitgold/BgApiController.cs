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

using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace Bitgold
{
    public class BgApiResult
    {
        public enum ResultType
        {
            SUCCESS,
            ERROR
        }

        public ResultType Type { get; set; }
        public string Message { get; set; }
        public string TransactionHash { get; set; }
        public string Notice { get; set; }
    };

    public class BgApiController
    {
        static string BlockchainBaseUrl = "https://blockchain.info/";
        static string WebContentType = "application/json, charset=utf-8";
        static string TransactionNote = "Mediated by Bitgold";

        // Blockchain API response information keys referenced from https://blockchain.info/api/blockchain_wallet_api
        static string BlockchainSuccessKey = "message"; // Blockchain.info returns a 'message' result if the transaction is successful
        static string BlockchainErrorKey = "error";
        static string BlockchainTransactionHashKey = "tx_hash";
        static string BlockchainNoticeKey = "notice";

        static float MinTransactionValue = 0.49f;
        static float MaxTransactionValue = 9.99f;

        // error codes
        static float InvalidCurrencyError = 0.0f;
        static float InvalidValueError = -1.0f;
        // Bitgold API error results
        static BgApiResult[] ErrorResults = new BgApiResult[]
        {
            new BgApiResult{ Type=BgApiResult.ResultType.ERROR, Message="The player has an currency! Please view the README file for Bitgold's supported currencies." },
            new BgApiResult{ Type=BgApiResult.ResultType.ERROR, Message="You have provided an invalid transaction value! Please view the README file for Bitgold's supported transaction values." }
        };

        public BgApiResult SubmitTransaction(BgTransaction transaction)
        {
            // convert value to Bitcoin
            float bitcoinValue = ValueToBitcoin(transaction.Player.Currency, transaction.Value);

            // error-check the conversion result
            if (bitcoinValue == InvalidCurrencyError || bitcoinValue == InvalidValueError)
            {
                return ErrorResults[(int)Math.Abs(bitcoinValue)];
            }

            // convert value to Satoshi
            float bitcoinSatoshi = bitcoinValue * 100000000;

            // try transaction
            // API transaction information at https://blockchain.info/api/blockchain_wallet_api
            string requestUrl = BlockchainBaseUrl + "merchant/" + transaction.Player.PrivateKey + "/payment?" + "to=" + transaction.Developer.Address + "&amount=" + bitcoinSatoshi.ToString() + "&from=" + transaction.Player.Address + "&note=" + TransactionNote;
            WebRequest request = WebRequest.Create(requestUrl);
            request.ContentType = WebContentType;
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            // retrieve the transaction result from the JSON response
            JObject jsonResponse = JObject.Parse(reader.ReadToEnd());
            BgApiResult result = new BgApiResult();

            // find the type of response, and then set the response message accordingly
            if ((string)jsonResponse[BlockchainSuccessKey] != null)
            {
                result.Type = BgApiResult.ResultType.SUCCESS;
                result.Message = (string)jsonResponse[BlockchainSuccessKey];
                result.TransactionHash = (string)jsonResponse[BlockchainTransactionHashKey];
                result.Notice = (string)jsonResponse[BlockchainNoticeKey];
                
            }
            else if ((string)jsonResponse[BlockchainErrorKey] != null)
            {
                result.Type = BgApiResult.ResultType.ERROR;
                result.Message = (string)jsonResponse[BlockchainErrorKey];
                // TODO: insufficient funds error may return required and available funds in Satoshi - so handle this information in some way
            }
            else
            {
                // unrecognised response
                result = null;
            }

            return result;
        }

        float ValueToBitcoin(BgCurrency currency, float value)
        {
            // validate the currency argument
            if ((int)currency >= 0 && (int)currency < (int)BgCurrency.CURRENCY_COUNT)
            {
                // validate the value argument
                if (value >= MinTransactionValue && value <= MaxTransactionValue)
                {
                    // request the currency conversion
                    // API currency conversion information at https://blockchain.info/api/exchange_rates_api
                    string requestUrl = BlockchainBaseUrl + "tobtc?currency=" + currency.ToString() + "&value=" + value.ToString();
                    // WebRequest code referenced from http://msdn.microsoft.com/en-us/library/456dfw4f(v=vs.110).aspx
                    WebRequest request = WebRequest.Create(requestUrl);
                    request.ContentType = WebContentType;
                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    // get the response
                    float result = float.Parse(reader.ReadToEnd());
                    // TODO: handle currency conversion error response
                    return result;
                }
                else
                {
                    return InvalidValueError;
                }
            }
            else
            {
                return InvalidCurrencyError;
            }
        }
    }
}
