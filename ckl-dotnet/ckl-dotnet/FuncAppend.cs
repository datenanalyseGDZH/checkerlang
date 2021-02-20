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
    public class FuncAppend : FuncBase
    {
        public FuncAppend() : base("append")
        {
            info = "append(lst, element)\r\n" +
                   "\r\n" +
                   "Appends the element to the list lst. The lst may also be a set.\r\n" +
                   "Returns the changed list.\r\n" +
                   "\r\n" +
                   ": append([1, 2], 3) ==> [1, 2, 3]\r\n" +
                   ": append(set([1, 2]), 3) ==> set([1, 2, 3])\r\n";
        }

        public override List<string> GetArgNames()
        {
            return new List<string> {"lst", "element"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var lst = args.Get("lst");
            var element = args.Get("element");

            if (lst.IsList())
            {
                lst.AsList().GetValue().Add(element);
                return lst;
            }

            if (lst.IsSet())
            {
                lst.AsSet().GetValue().Add(element);
                return lst;
            }
            
            throw new ControlErrorException("Cannot append to " + lst, pos);
        }
    }
}
