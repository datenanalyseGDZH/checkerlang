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
    public class FuncIsNotEmpty : FuncBase
    {
        public FuncIsNotEmpty() : base("is_not_empty")
        {
            info = "is_not_empty(obj)\r\n" +
                   "\r\n" +
                   "Returns TRUE, if the obj is not empty.\r\n" +
                   "Lists, sets and maps are empty, if they do not contain elements.\r\n" +
                   "Strings are empty, if the contain no characters. NULL is always empty.\r\n" +
                   "\r\n" +
                   ": is_not_empty([]) ==> FALSE\r\n" +
                   ": is_not_empty(set([1, 2])) ==> TRUE\r\n" +
                   ": is_not_empty('a') ==> TRUE\r\n";
        }

        public override List<string> GetArgNames()
        {
            return new List<string> {"obj"};
        }

        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var obj = args.Get("obj");

            if (obj.IsNull())
            {
                return ValueBoolean.FALSE;
            }

            if (obj.IsNumerical())
            {
                return ValueBoolean.TRUE;
            }

            if (obj.IsString())
            {
                return ValueBoolean.From(obj.AsString().GetValue().Length > 0);
            }

            if (obj.IsList())
            {
                return ValueBoolean.From(obj.AsList().GetValue().Count != 0);
            }

            if (obj.IsSet())
            {
                return ValueBoolean.From(obj.AsSet().GetValue().Count != 0);
            }

            if (obj.IsMap())
            {
                return ValueBoolean.From(obj.AsMap().GetValue().Count != 0);
            }

            if (obj.IsObject()) return ValueBoolean.From(obj.AsObject().value.Count != 0);
            
            return ValueBoolean.TRUE;
        }
    }
}
