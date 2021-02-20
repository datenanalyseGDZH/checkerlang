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
package ch.checkerlang.functions;

import ch.checkerlang.Args;
import ch.checkerlang.ControlErrorException;
import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;

import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.Set;

public class FuncRemove extends FuncBase {
    public FuncRemove() {
        super("remove");
        this.info = "remove(lst, element)\r\n" +
                "\r\n" +
                "Removes the element from the list lst. The lst may also be a set or a map.\r\n" +
                "Returns the changed list, but the list is changed in place.\r\n" +
                "\r\n" +
                ": remove([1, 2, 3, 4], 3) ==> [1, 2, 4]\r\n" +
                ": remove(<<1, 2, 3, 4>>, 3) ==> <<1, 2, 4>>\r\n" +
                ": remove(<<< 'a' => 1, 'b' => 2, 'c' => 3, 'd' => 4>>>, 'c') ==> <<<'a' => 1, 'b' => 2, 'd' => 4>>>\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("lst", "element");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value lst = args.get("lst");
        Value element = args.get("element");

        if (lst.isList()) {
            List<Value> list = lst.asList().getValue();
            for (int i = 0; i < list.size(); i++) {
                if (list.get(i).isEquals(element)) {
                    list.remove(i);
                    break;
                }
            }
            return lst;
        }
        if (lst.isSet()) {
            Set<Value> set = lst.asSet().getValue();
            set.remove(element);
            return lst;
        }
        if (lst.isMap()) {
            Map<Value, Value> map = lst.asMap().getValue();
            map.remove(element);
            return lst;
        }

        throw new ControlErrorException("Cannot remove from " + lst, pos);
    }
}
