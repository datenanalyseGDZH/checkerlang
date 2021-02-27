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
    public class FuncRemove : FuncBase
    {
        public FuncRemove() : base("remove")
        {
            this.info = "remove(lst, element)\r\n" +
                        "\r\n" +
                        "Removes the element from the list lst. The lst may also be a set or a map.\r\n" +
                        "Returns the changed list, but the list is changed in place.\r\n" +
                        "\r\n" +
                        ": remove([1, 2, 3, 4], 3) ==> [1, 2, 4]\r\n" +
                        ": remove(<<1, 2, 3, 4>>, 3) ==> <<1, 2, 4>>\r\n" +
                        ": remove(<<< 'a' => 1, 'b' => 2, 'c' => 3, 'd' => 4>>>, 'c') ==> <<<'a' => 1, 'b' => 2, 'd' => 4>>>\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"lst", "element"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var lst = args.Get("lst");
            var element = args.Get("element");

            if (lst.IsList()) {
                var list = lst.AsList().GetValue();
                for (var i = 0; i < list.Count; i++) {
                    if (list[i].IsEquals(element)) {
                        list.RemoveAt(i);
                        break;
                    }
                }
                return lst;
            }
            if (lst.IsSet()) {
                var set = lst.AsSet().GetValue();
                set.Remove(element);
                return lst;
            }
            if (lst.IsMap()) {
                var map = lst.AsMap().GetValue();
                map.Remove(element);
                return lst;
            }

            throw new ControlErrorException("Cannot remove from " + lst.Type(), pos);
        }
    }
}