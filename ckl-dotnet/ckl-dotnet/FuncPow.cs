/*  Copyright (c) 2021 Damian Brunold, Gesundheitsdirektion Kanton Zürich

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Collections.Generic;

namespace CheckerLang
{
    public class FuncPow : FuncBase
    {
        public FuncPow() : base("pow")
        {
            info = "pow(x, y)\r\n" +
                   "\r\n" +
                   "Returns the power x ^ y.\r\n" +
                   "\r\n" +
                   ": pow(2, 3) ==> 8\r\n" +
                   ": pow(2.5, 2) ==> 6.25\r\n" +
                   ": pow(4, 2) ==> 16\r\n" +
                   ": pow(4.0, 2.0) ==> 16.0\r\n" +
                   ": round(pow(2, 1.5), digits = 3) ==> 2.828\r\n";
        }

        public override List<string> GetArgNames()
        {
            return new List<string> {"x", "y"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("x")) return ValueNull.NULL;
            if (args.IsNull("y")) return ValueNull.NULL;
            if (args.Get("y").IsInt())
            {
                if (args.Get("x").IsInt())
                {
                    var x = args.GetInt("x").GetValue();
                    var y = args.GetInt("y").GetValue();
                    long result = 1;
                    for (int i = 0; i < y; i++)
                    {
                        result *= x;
                    }
                    return new ValueInt(result);
                }
                else
                {
                    var x = args.GetDecimal("x").GetValue();
                    var y = args.GetInt("y").GetValue();
                    decimal result = 1;
                    for (int i = 0; i < y; i++)
                    {
                        result *= x;
                    }
                    return new ValueDecimal(result);
                }
            }
            else
            {
                return new ValueDecimal((decimal) Math.Pow((double) args.GetNumerical("x").GetValue(), (double) args.GetNumerical("y").GetValue()));
            }
        }
    }
}
