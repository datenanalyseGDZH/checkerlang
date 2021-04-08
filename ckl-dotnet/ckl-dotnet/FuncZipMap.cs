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
    public class FuncZipMap : FuncBase
    {
        public FuncZipMap() : base("zip_map")
        {
            info = "zip_map(a, b)\r\n" +
                   "\r\n" +
                   "Returns a map where the key of each entry is taken from a,\r\n" +
                   "and where the value of each entry is taken from b, where\r\n" +
                   "a and b are lists of identical length.\r\n" +
                   "\r\n" +
                   ": zip_map(['a', 'b', 'c'], [1, 2, 3]) ==> <<<'a' => 1, 'b' => 2, 'c' => 3>>>\r\n";
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

            if (a.IsList() && b.IsList())
            {
                var lista = a.AsList().GetValue();
                var listb = b.AsList().GetValue();
                var result = new ValueMap();
                for (var i = 0; i < Math.Min(lista.Count, listb.Count); i++)
                {
                    result.AddItem(lista[i], listb[i]);
                }

                return result;
            }

            throw new ControlErrorException(new ValueString("ERROR"),"Cannot zip_map " + a + " and " + b, pos);
        }
    }
}
