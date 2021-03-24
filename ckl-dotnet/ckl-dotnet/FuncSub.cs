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
    public class FuncSub : FuncBase
    {
        public FuncSub() : base("sub")
        {
            info = "sub(a, b)\r\n" +
                   "\r\n" +
                   "Returns the subtraction of b from a. For numerical values this uses usual arithmetic.\r\n" +
                   "For lists and sets, this returns lists and sets minus the element b. If a is a datetime\r\n" +
                   "value and b is datetime value, then the date difference is returned. If a is a datetime\r\n" +
                   "value and b is a numeric value, then b is interpreted as number of days and the corresponding\r\n" +
                   "datetime after subtracting these number of days is returned.\r\n" +
                   "\r\n" +
                   ": sub(1, 2) ==> -1\r\n" +
                   ": sub([1, 2, 3], 2) ==> [1, 3]\r\n" +
                   ": sub(date('20170405'), date('20170402')) ==> 3\r\n" +
                   ": sub(date('20170405'), 3.5) ==> '20170401120000'\r\n" +
                   ": sub(<<3, 1, 2>>, 2) ==> <<1, 3>>";
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
                return new ValueInt(a.AsInt().GetValue() - b.AsInt().GetValue());
            }
            
            if (a.IsNumerical() && b.IsNumerical())
            {
                return new ValueDecimal(a.AsDecimal().GetValue() - b.AsDecimal().GetValue());
            }
            
            if (a.IsDate())
            {
                if (b.IsDate())
                {
                    return new ValueInt(a.AsDate().GetValue().Subtract(b.AsDate().GetValue()).Days);
                }
                return new ValueDate(a.AsDate().GetValue() - TimeSpan.FromDays((double) args.GetAsDecimal("b").GetValue()));
            }

            if (a.IsList())
            {
                var result = new ValueList();
                foreach (var item in a.AsList().GetValue())
                {
                    var add = true;
                    foreach (var val in args.GetAsList("b").GetValue())
                    {
                        if (!item.IsEquals(val)) continue;
                        add = false;
                        break;
                    }
                    if (add) result.AddItem(item);
                }

                return result;
            }

            if (a.IsSet())
            {
                var result = new ValueSet();
                var minus = new SortedSet<Value>(new ComparerValue());
                if (b.IsSet())
                {
                    minus = new SortedSet<Value>(b.AsSet().GetValue(), new ComparerValue());
                }
                else if (b.IsList())
                {
                    foreach (var element in b.AsList().GetValue())
                    {
                        minus.Add(element);
                    }
                }
                else
                {
                    minus.Add(b);
                }
                foreach (var element in a.AsSet().GetValue())
                {
                    if (!minus.Contains(element)) result.AddItem(element);
                }
                return result;
            }

            throw new ControlErrorException("Cannot subtract " + b + " from " + a, pos);
        }
    }
}
