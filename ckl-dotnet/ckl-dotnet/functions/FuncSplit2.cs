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
    public class FuncSplit2 : FuncBase
    {
        public FuncSplit2() : base("split2")
        {
            info = "split2(str, sep1, sep2)\r\n" +
                   "\r\n" +
                   "Performs a two-stage split of the string data.\r\n" +
                   "This results in a list of list of strings.\r\n" +
                   "\r\n" +
                   ": split2('a:b:c|d:e:f', escape_pattern('|'), escape_pattern(':')) ==> [['a', 'b', 'c'], ['d', 'e', 'f']]\r\n" +
                   ": split2('', '\\|', ':') ==> []\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"str", "sep1", "sep2"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("str")) return ValueNull.NULL;
            var value = args.GetString("str").GetValue();
            var sep1 = args.GetAsString("sep1").GetValue();
            var sep2 = args.GetAsString("sep2").GetValue();
            var result = FuncSplit.SplitValue(value, sep1);
            var list = result.GetValue();
            for (var i = 0; i < list.Count; i++)
            {
                list[i] = FuncSplit.SplitValue(list[i].AsString().GetValue(), sep2);
            }
            return result;
        }
    }
}
