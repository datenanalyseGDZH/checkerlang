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
    public class FuncDeleteAt : FuncBase
    {
        public FuncDeleteAt() : base("delete_at")
        {
            this.info = "delete_at(lst, index)\r\n" +
                        "\r\n" +
                        "Removes the element at the given index from the list lst.\r\n" +
                        "The list is changed in place. Returns the removed element or\r\n" +
                        "NULL, if no element was removed\r\n" +
                        "\r\n" +
                        ": delete_at(['a', 'b', 'c', 'd'], 2) ==> 'c'\r\n" +
                        ": delete_at(['a', 'b', 'c', 'd'], -3) ==> 'b'\r\n" +
                        ": def lst = ['a', 'b', 'c', 'd']; delete_at(lst, 2); lst ==> ['a', 'b', 'd']\r\n" +
                        ": delete_at(['a', 'b', 'c', 'd'], 4) ==> NULL\r\n";
        }

        public override List<string> GetArgNames()
        {
            return new List<string> {"lst", "index"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var lst = args.Get("lst");
            var index = (int) args.GetInt("index").GetValue();

            if (lst.IsList()) {
                var list = lst.AsList().GetValue();
                if (index < 0) index = list.Count + index;
                if (index >= list.Count) return ValueNull.NULL;
                var result = list[index];
                list.RemoveAt(index);
                return result;
            }

            throw new ControlErrorException(new ValueString("ERROR"),"Cannot delete from " + lst.Type(), pos);
        }
    }
}
