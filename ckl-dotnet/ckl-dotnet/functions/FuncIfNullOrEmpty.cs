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
    public class FuncIfNullOrEmpty : FuncBase
    {
        public FuncIfNullOrEmpty() : base("if_null_or_empty")
        {
            info = "if_null_or_empty(a, b)\r\n" +
                   "\r\n" +
                   "Returns b if a is null or an empty string otherwise returns a.\r\n" +
                   "\r\n" +
                   ": if_null_or_empty(1, 2) ==> 1\r\n" +
                   ": if_null_or_empty(NULL, 2) ==> 2\r\n" +
                   ": if_null_or_empty('', 2) ==> 2\r\n";
        }

        public override List<string> GetArgNames()
        {
            return new List<string> {"a", "b"};
        }

        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var a = args.Get("a");

            if (a.IsNull())
            {
                return args.Get("b");
            }

            if (a.IsString() && a.AsString().GetValue().Length == 0)
            {
                return args.Get("b");
            }

            return a;
        }
    }
}
