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
    public class FuncZip : FuncBase
    {
        public FuncZip() : base("zip")
        {
            info = "zip(a, b)\r\n" +
                   "\r\n" +
                   "Returns a list where each element is a list of two items.\r\n" +
                   "The first of the two items is taken from the first list,\r\n" +
                   "the second from the second list. The resulting list has\r\n" +
                   "the same length as the shorter of the two input lists.\r\n" +
                   "\r\n" +
                   ": zip([1, 2, 3], [4, 5, 6, 7]) ==> [[1, 4], [2, 5], [3, 6]]\r\n";
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
                var result = new List<Value>();
                for (var i = 0; i < Math.Min(lista.Count, listb.Count); i++)
                {
                    var pair = new List<Value>();
                    pair.Add(lista[i]);
                    pair.Add(listb[i]);
                    result.Add(new ValueList(pair));
                }

                return new ValueList(result);
            }

            throw new ControlErrorException(new ValueString("ERROR"),"Cannot zip " + a + " and " + b, pos);
        }
    }
}
