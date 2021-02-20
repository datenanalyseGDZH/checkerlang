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
    public class FuncMod : FuncBase
    {
        public FuncMod() : base("mod")
        {
            info = "mod(a, b)\r\n" +
                   "\r\n" +
                   "Returns the modulus of a modulo b.\r\n" +
                   "\r\n" +
                   ": mod(7, 2) ==> 1\r\n";
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
                return new ValueInt(a.AsInt().GetValue() % b.AsInt().GetValue());
            }
            
            if (a.IsNumerical() && b.IsNumerical())
            {
                return new ValueDecimal(a.AsDecimal().GetValue() % b.AsDecimal().GetValue());
            }
            
            throw new ControlErrorException("Cannot calculate modulus of " + a + " by " + b, pos);
        }
    }
}
