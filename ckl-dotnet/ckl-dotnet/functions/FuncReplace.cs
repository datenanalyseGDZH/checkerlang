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
    public class FuncReplace : FuncBase
    {
        public FuncReplace() : base("replace")
        {
            info = "replace(s, a, b, start = 0)\r\n" +
                   "\r\n" +
                   "Replaces all occurences of a in the string s with b.\r\n" +
                   "The optional parameter start specifies the start index.\r\n" +
                   "\r\n" +
                   ": replace('abc', 'b', 'x') ==> 'axc'\r\n" +
                   ": replace('abcbcbca', 'b', 'x') ==> 'axcxcxca'\r\n" +
                   ": replace('abc', 'b', 'xy') ==> 'axyc'\r\n" +
                   ": replace('abcdef', 'bcd', 'xy') ==> 'axyef'\r\n" +
                   ": replace('abcabcabc', 'abc', 'xy', start = 3) ==> 'abcxyxy'\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"s", "a", "b", "start"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("s")) return ValueNull.NULL;
            var s = args.GetString("s").GetValue();
            var a = args.GetString("a").GetValue();
            var b = args.GetString("b").GetValue();
            var start = args.GetInt("start", 0L).GetValue();
            if (start >= s.Length) return args.GetString("s");
            if (start == 0)
            {
                return new ValueString(s.Replace(a, b));
            }
            var prefix = s.Substring(0, (int) start);
            return new ValueString(prefix + s.Substring((int) start).Replace(a, b));
        }
    }
    
}