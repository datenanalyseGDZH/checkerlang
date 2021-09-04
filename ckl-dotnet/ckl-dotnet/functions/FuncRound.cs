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
    public class FuncRound : FuncBase
    {
        public FuncRound() : base("round")
        {
            info = "round(x, digits = 0)\r\n" +
                   "\r\n" +
                   "Returns the decimal value x rounded to the specified number of digits.\r\n" +
                   "Default for digits is 0.\r\n" +
                   "\r\n" +
                   ": round(1.345, digits = 1) ==> 1.3\r\n";
        }

        public override List<string> GetArgNames()
        {
            return new List<string> {"x", "digits"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("x")) return ValueNull.NULL;
            var digits = 0;
            if (args.HasArg("digits"))
            {
                digits = (int) args.GetInt("digits").GetValue();
            }
            return new ValueDecimal((decimal) Math.Round((double) args.GetNumerical("x").GetValue(), digits));
        }
    }
}
