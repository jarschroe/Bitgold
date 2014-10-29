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

namespace Bitgold
{
    public enum BgCurrency
    {
        // referred to http://www.xe.com/iso4217.php for currency codes
        AUD
    }

    public class BgApiController
    {
        public string SubmitTransaction(BgTransaction transaction)
        {
            return "error";
        }

        // TODO: value to Bitcoin (current function), or currency to Bitcoin (i.e. ret=currencyInBitcoin, args=(currency))?
        public float CurrencyToBitcoin(BgCurrency currency, float value)
        {
            // TODO: validate
            return 0.0f;
        }
    }
}
