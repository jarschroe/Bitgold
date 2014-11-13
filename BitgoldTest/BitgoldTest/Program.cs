﻿// The MIT License (MIT)
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

using Bitgold;
using System;
using System.IO;

namespace BitgoldTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // create parties
            string devAddress = "18R3k1bCPKmD6oNtE5rBq2pwut8i2d8SEB";
            BgDeveloper developer = new BgDeveloper(devAddress);
            string key = new StreamReader("H:/New Text Document.txt").ReadToEnd();
            string playerAddress = "16q6tj1wCAYbdVg7yWDngtnYmCeAUBAR2Q";
            BgPlayer player = new BgPlayer(playerAddress, key);
            // dummy transaction ($0.99)
            float value = 0.99f;
            BgTransaction transaction = new BgTransaction(developer, player, value);
            // do transaction (currently does nothing)
            BgApiController api = new BgApiController();
            BgApiResult result = api.SubmitTransaction(transaction);
            Console.Write("Result: " + result.Type.ToString() + ": " + result.Message);
            Console.Read();
        }
    }
}
