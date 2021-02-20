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
    public class FuncDiv0 : FuncBase
    {
        public FuncDiv0() : base("div0")
        {
            info = "div0(a, b, div_0_value = MAXINT)\r\n" +
                   "\r\n" +
                   "If b is not zero, the result of a / b is returned.\r\n" +
                   "If b is zero, the value div_0_value is returned.\r\n" +
                   "\r\n" +
                   ": div0(12, 3) ==> 4\r\n" +
                   ": div0(12, 5) ==> 2\r\n" +
                   ": div0(12.0, 5) ==> 2.40\r\n" +
                   ": div0(12.5, 2) ==> 6.25\r\n" +
                   ": div0(12, 0) ==> MAXINT\r\n" +
                   ": div0(12, 0, 0) ==> 0\r\n" +
                   ": div0(12, 0.0, 0) ==> 0\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"a", "b", "div_0_value"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var a = args.Get("a");
            var b = args.Get("b");

            if (a.IsNull() || b.IsNull())
            {
                return ValueNull.NULL;
            }
            
            Value div_0_value;
            if (args.HasArg("div_0_value")) div_0_value = args.Get("div_0_value");
            else div_0_value = new ValueInt(long.MaxValue);

            if (a.IsInt() && b.IsInt())
            {
                var divisor = b.AsInt().GetValue();
                if (divisor == 0)
                {
                    return div_0_value;
                }
                return new ValueInt(a.AsInt().GetValue() / divisor);
            }
            
            if (a.IsNumerical() && b.IsNumerical())
            {
                var divisor = b.AsDecimal().GetValue();
                if (divisor == 0)
                {
                    return div_0_value;
                }
                return new ValueDecimal(a.AsDecimal().GetValue() / divisor);
            }
            
            throw new ControlErrorException("Cannot div0 " + a + " by " + b, pos);
        }
    }
    
}
