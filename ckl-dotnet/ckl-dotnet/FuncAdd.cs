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
    public class FuncAdd : FuncBase
    {
        public FuncAdd() : base("add")
        {
            info = "add(a, b)\r\n" +
                   "\r\n" +
                   "Returns the sum of a and b. For numerical values this uses the usual arithmetic.\r\n" +
                   "For lists and strings it concatenates. For sets it uses union.\r\n" +
                   "For a date and a number it adds the number of days to the date and returns the new date.\r\n" +
                   "\r\n" +
                   ": add(1, 2) ==> 3\r\n" +
                   ": add(1, 2.0) ==> 3.0\r\n" +
                   ": add([1, 2], 3) ==> [1, 2, 3]\r\n" +
                   ": add([1, 2], [3, 4]) ==> [1, 2, 3, 4]\r\n" +
                   ": add('ab', 'c') ==> 'abc'\r\n" +
                   ": add('ab', 1) ==> 'ab1'\r\n" +
                   ": format_date(add(date('2021010115'), 1), fmt='yyyyMMddHH') ==> '2021010215\r\n" +
                   ": format_date(add(date('2021010115'), 1.5), fmt='yyyyMMddHH') ==> '2021010303\r\n";
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
                return new ValueInt(a.AsInt().GetValue() + b.AsInt().GetValue());
            }
            
            if (a.IsNumerical() && b.IsNumerical())
            {
                return new ValueDecimal(a.AsDecimal().GetValue() + b.AsDecimal().GetValue());
            }
            
            if (a.IsList()) {
                if (b.IsCollection()) {
                    return new ValueList(a.AsList().GetValue()).AddItems(b.AsList().GetValue());
                }
                return new ValueList(a.AsList().GetValue()).AddItem(b);
            }

            if (a.IsDate() && b.IsNumerical())
            {
                return new ValueDate(a.AsDate().GetValue() + TimeSpan.FromDays((double) args.GetAsDecimal("b").GetValue()));
            }

            if (a.IsSet()) {
                if (b.IsCollection()) {
                    return new ValueSet(a.AsSet().GetValue()).AddItems(b.AsSet().GetValue());
                }
                return new ValueSet(a.AsSet().GetValue()).AddItem(b);
            }

            if (b.IsList()) {
                var result = new ValueList();
                result.AddItem(a);
                result.AddItems(b.AsList().GetValue());
                return result;
            }

            if (b.IsSet()) {
                var result = new ValueSet();
                result.AddItem(a);
                result.AddItems(b.AsSet().GetValue());
                return result;
            }

            if (a.IsString() && b.IsAtomic() || a.IsAtomic() && b.IsString())
            {
                return new ValueString(a.AsString().GetValue() + b.AsString().GetValue());
            }

            throw new ControlErrorException("Cannot add " + a + " and " + b, pos);
        }
    }
}
