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
    public class FuncDiv : FuncBase
    {
        public FuncDiv() : base("div")
        {
            info = "div(a, b)\r\n" +
                   "\r\n" +
                   "Returns the value of a divided by b. If both values are ints,\r\n" +
                   "then the result is also an int. Otherwise, it is a decimal.\r\n" +
                   "\r\n" +
                   ": div(6, 2) ==> 3\r\n";
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
                var divisor = b.AsInt().GetValue();
                if (divisor == 0)
                {
                    if (environment.IsDefined("DIV_0_VALUE") &&
                        environment.Get("DIV_0_VALUE", pos) != ValueNull.NULL)
                    {
                        return environment.Get("DIV_0_VALUE", pos);
                    }
                    throw new ControlDivideByZeroException("divide by zero", pos);
                }
                return new ValueInt(a.AsInt().GetValue() / divisor);
            }
            
            if (a.IsNumerical() && b.IsNumerical())
            {
                var divisor = b.AsDecimal().GetValue();
                if (divisor == 0)
                {
                    if (environment.IsDefined("DIV_0_VALUE") &&
                        environment.Get("DIV_0_VALUE", pos) != ValueNull.NULL)
                    {
                        return environment.Get("DIV_0_VALUE", pos);
                    }
                    throw new ControlDivideByZeroException("divide by zero", pos);
                }
                return new ValueDecimal(a.AsDecimal().GetValue() / divisor);
            }
            
            throw new ControlErrorException(new ValueString("ERROR"),"Cannot divide values", pos);
        }
    }
}
