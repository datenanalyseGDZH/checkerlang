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
    public class FuncCompare : FuncBase
    {
        public FuncCompare() : base("compare")
        {
            info = "compare(a, b)\r\n" +
                   "\r\n" +
                   "Returns -1 if a is less than b, 0 if a is equal to b, and 1 if a is greater than b.\r\n" +
                   "\r\n" +
                   ": compare(1, 2) ==> -1\r\n";
        }

        public override List<string> GetArgNames()
        {
            return new List<string> {"a", "b"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var a = args.Get("a");
            var b = args.Get("b");

            if (a.IsNull() && b.IsNull())
            {
                return ValueNull.NULL;
            }
            
            if (a.IsInt() && b.IsInt())
            {
                return new ValueInt(a.AsInt().GetValue().CompareTo(b.AsInt().GetValue()));
            }

            if (a.IsNumerical() && b.IsNumerical())
            {
                return new ValueInt(a.AsDecimal().GetValue().CompareTo(b.AsDecimal().GetValue()));
            }
            
            if (a.IsDate() && b.IsDate())
            {
                return new ValueInt(a.AsDate().GetValue().CompareTo(b.AsDate().GetValue()));
            }

            return new ValueInt(string.Compare(a.AsString().GetValue(), b.AsString().GetValue(), StringComparison.Ordinal));
        }
    }
}
