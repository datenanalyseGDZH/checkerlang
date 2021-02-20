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
using System.Collections.Generic;

namespace CheckerLang
{
    public class FuncMul : FuncBase
    {
        public FuncMul() : base("mul")
        {
            info = "mul(a, b)\r\n" +
                   "\r\n" +
                   "Returns the product of a and b. For numerical values this uses the usual arithmetic.\r\n" +
                   "If a is a string and b is an int, then the string a is repeated b times. If a is a\r\n" +
                   "list and b is an int, then the list is repeated b times.\r\n" +
                   "\r\n" +
                   ": mul(2, 3) ==> 6\r\n" +
                   ": mul('2', 3) ==> '222'\r\n" +
                   ": mul([1, 2], 3) ==> [1, 2, 1, 2, 1, 2]\r\n";
        }

        public override List<string> GetArgNames()
        {
            return new List<string> {"a", "b"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var a = args.Get("a");
            var b = args.Get("b");

            if (a.IsNull() || b.IsNull())
            {
                return ValueNull.NULL;
            }
            
            if (a.IsInt() && b.IsInt())
            {
                return new ValueInt(a.AsInt().GetValue() * b.AsInt().GetValue());
            }
            
            if (a.IsNumerical() && b.IsNumerical())
            {
                return new ValueDecimal(a.AsDecimal().GetValue() * b.AsDecimal().GetValue());
            }
            
            if (a.IsString() && b.IsInt())
            {
                var s = a.AsString().GetValue();
                var r = "";
                for (var i = 0; i < b.AsInt().GetValue(); i++)
                {
                    r += s;
                }
                return new ValueString(r);
            }
            
            if (a.IsList() && b.IsInt())
            {
                var result = new List<Value>();
                for (var i = 0; i < b.AsInt().GetValue(); i++)
                {
                    result.AddRange(a.AsList().GetValue());
                }

                return new ValueList(result);
            }
            
            throw new ControlErrorException("Cannot multiply " + a + " by " + b, pos);
        }
    }
}
