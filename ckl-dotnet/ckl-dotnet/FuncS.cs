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
using System.IO;

namespace CheckerLang
{
    public class FuncS : FuncBase
    {
        public FuncS() : base("s")
        {
            info = "s(str, start = 0)\r\n" +
                   "\r\n" +
                   "Returns a string, where all placeholders are replaced with their\r\n" +
                   "appropriate values. Placeholder have the form '{var}' and result\r\n" +
                   "in the value of the variable var inserted at this location.\r\n" +
                   "\r\n" +
                   ": def name = 'damian'; s('hello {name}') ==> 'hello damian'\r\n" +
                   ": def foo = '{bar}'; def bar = 'baz'; s('{foo}{bar}') ==> '{bar}baz'\r\n" +
                   ": s('{PI} is cool') ==> '3.14159265358979 is cool'\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"str", "start"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("str")) return ValueNull.NULL;
            var str = args.GetString("str").GetValue();
            var start = (int) args.GetInt("start", 0).GetValue();
            if (start < 0) start = str.Length + start;
            while (true) {
                var idx1 = str.IndexOf('{', start);
                if (idx1 == -1) return new ValueString(str);
                var idx2 = str.IndexOf('}', idx1 + 1);
                if (idx2 == -1) return new ValueString(str);
                var variable = str.Substring(idx1 + 1, idx2 - idx1 - 1);
                var value = environment.Get(variable, pos).AsString().GetValue();
                str = str.Substring(0, idx1) + value + str.Substring(idx2 + 1);
                start = idx1 + value.Length;
            }
        }
    }
    
}