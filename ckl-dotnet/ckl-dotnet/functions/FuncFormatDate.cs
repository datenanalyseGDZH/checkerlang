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
using System.Globalization;

namespace CheckerLang
{
    public class FuncFormatDate : FuncBase
    {
        public FuncFormatDate() : base("format_date")
        {
            info = "format_date(date, fmt = 'yyyy-MM-dd HH:mm:ss')\r\n" +
                   "\r\n" +
                   "Formats the date value according to fmt and returns a string value.\r\n" +
                   "\r\n" +
                   ": format_date(date('20170102')) ==> '2017-01-02 00:00:00'\r\n" +
                   ": format_date(NULL) ==> NULL\r\n" +
                   ": format_date(date('2017010212'), fmt = 'HH') ==> '12'\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"date", "fmt"};
        }

        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("date")) return ValueNull.NULL;
            var value = args.GetDate("date");
            var date = value.AsDate().GetValue();
            var fmt = args.HasArg("fmt") ? args.GetString("fmt").GetValue() : "yyyy-MM-dd HH:mm:ss";
            return new ValueString(date.ToString(fmt, DateTimeFormatInfo.InvariantInfo));
        }
    }
}
