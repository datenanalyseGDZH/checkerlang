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
using System.Linq;
using System.Xml.Linq;

namespace CheckerLang
{
    public class FuncSorted : FuncBase
    {
        public FuncSorted() : base("sorted")
        {
            info = "sorted(lst, cmp=compare, key=identity)\r\n" +
                   "\r\n" +
                   "Returns a sorted copy of the list. This is sorted according to the\r\n" +
                   "value returned by the key function for each element of the list.\r\n" +
                   "The values are compared using the compare function cmp.\r\n" +
                   "\r\n" +
                   ": sorted([3, 2, 1]) ==> [1, 2, 3]\r\n" +
                   ": sorted([6, 2, 5, 3, 1, 4]) ==> [1, 2, 3, 4, 5, 6]\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"lst", "cmp", "key"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var env = environment.NewEnv();
            var lst = args.GetAsList("lst");
            var cmp = args.HasArg("cmp") ? args.GetFunc("cmp") : environment.Get("compare", pos).AsFunc();
            var key = args.HasArg("key") ? args.GetFunc("key") : environment.Get("identity", pos).AsFunc();
            var result = new List<Value>();
            result.AddRange(lst.GetValue());
            for (var i = 1; i < result.Count; i++)
            {
                var v = key.Execute(new Args(key.GetArgNames()[0], result[i], pos), env, pos);
                for (var j = i - 1; j >= 0; j--)
                {
                    var v2 = key.Execute(new Args(key.GetArgNames()[0], result[j], pos), env, pos);
                    var cmpargs = new Args(cmp.GetArgNames()[0], cmp.GetArgNames()[1], v, v2, pos);
                    var comparison = (int) cmp.Execute(cmpargs, env, pos).AsInt().GetValue();
                    if (comparison < 0)
                    {
                        var temp = result[j + 1];
                        result[j + 1] = result[j];
                        result[j] = temp;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return new ValueList(result);
        }
    }
}