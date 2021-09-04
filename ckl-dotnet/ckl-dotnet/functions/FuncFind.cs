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
    public class FuncFind : FuncBase
    {
        public FuncFind() : base("find")
        {
            info = "find(obj, part, key = identity, start = 0)\r\n" +
                   "\r\n" +
                   "Returns the index of the first occurence of part in obj.\r\n" +
                   "If part is not contained in obj, then -1 is returned. Start specifies\r\n" +
                   "the search start index. It defaults to 0.\r\n" +
                   "Obj can be a string or a list. In case of a string, part can be any\r\n" +
                   "substring, in case of a list, a single element.\r\n" +
                   "In case of lists, the elements can be accessed using the\r\n" +
                   "key function.\r\n" +
                   "\r\n" +
                   ": find('abcdefg', 'cde') ==> 2\r\n" +
                   ": find('abc|def|ghi', '|', start = 4) ==> 7\r\n" +
                   ": find('abcxyabc', 'abc', start = 5) ==> 5\r\n" +
                   ": find([1, 2, 3, 4], 3) ==> 2\r\n" +
                   ": find(['abc', 'def'], 'e', key = fn(x) x[1]) ==> 1\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"obj", "part", "key", "start"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("obj")) return ValueNull.NULL;
            var obj = args.Get("obj");
            var start = (int) args.GetInt("start", 0).GetValue();
            var key = args.HasArg("key") ? args.GetFunc("key") : null;
            if (obj.IsString()) {
                var part = args.GetString("part").GetValue();
                return new ValueInt(obj.AsString().GetValue().IndexOf(part, start, StringComparison.Ordinal));
            } else if (obj.IsList()) {
                var env = environment;
                if (key != null) env = environment.NewEnv();
                var item = args.Get("part");
                var list = obj.AsList().GetValue();
                for (int idx = start; idx < list.Count; idx++) {
                    var elem = list[idx];
                    if (key != null) elem = key.Execute(new Args(key.GetArgNames()[0], elem, pos), env, pos);
                    if (elem.IsEquals(item)) return new ValueInt(idx);
                }
                return new ValueInt(-1);
            }
            throw new ControlErrorException(new ValueString("ERROR"),"Find only works with strings and lists", pos);
        }
    }
    
}