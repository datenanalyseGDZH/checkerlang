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
using System.Globalization;

namespace CheckerLang
{
    public class FuncParseDate : FuncBase
    {
        public FuncParseDate() : base("parse_date")
        {
            info = "parse_date(str, fmt = 'yyyyMMdd')\r\n" +
                   "\r\n" +
                   "Parses the string str according to fmt and returns a datetime value.\r\n" +
                   "If the format does not match or if the date is invalid, the NULL\r\n" +
                   "value is returned.\r\n" +
                   "\r\n" +
                   "It is possible to pass a list of formats to the fmt parameter.\r\n" +
                   "The function sequentially tries to convert the str and if it\r\n" +
                   "works, returns the value.\r\n" +
                   "\r\n" +
                   ": parse_date('20170102') ==> '20170102'\r\n" +
                   ": parse_date('20170102', fmt = 'yyyyMMdd') ==> '20170102'\r\n" +
                   ": parse_date('2017010222', fmt = 'yyyyMMdd') ==> NULL\r\n" +
                   ": parse_date('20170102', fmt = 'yyyyMMddHH') ==> NULL\r\n" +
                   ": parse_date('20170102', fmt = ['yyyyMMdd']) ==> '20170102'\r\n" +
                   ": parse_date('201701022015', fmt = ['yyyyMMdd', 'yyyyMMddHH', 'yyyyMMddHHmm']) ==> '20170102'\r\n" +
                   ": parse_date('20170112', fmt = ['yyyy', 'yyyyMM']) ==> NULL\r\n" +
                   ": parse_date('20170144') ==> NULL\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"str", "fmt"};
        }

        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("str")) return ValueNull.NULL;
            var str = args.GetString("str").GetValue();
            var fmts = new List<string>();
            if (args.HasArg("fmt"))
            {
                if (args.Get("fmt").IsList())
                {
                    foreach (var fmt_ in args.GetList("fmt").GetValue())
                    {
                        fmts.Add(fmt_.AsString().GetValue());
                    }
                }
                else
                {
                    fmts.Add(args.GetString("fmt").GetValue());
                }
            }
            else
            {
                fmts.Add("yyyyMMdd");
            }

            foreach (var fmt in fmts)
            {
                try
                {
                    return new ValueDate(DateTime.ParseExact(str, fmt, DateTimeFormatInfo.InvariantInfo));
                }
                catch
                {
                    // continue;
                }
            }

            return ValueNull.NULL;
        }
    }
}