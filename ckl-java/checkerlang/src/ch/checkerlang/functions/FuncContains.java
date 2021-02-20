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
import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueBoolean;

import java.util.Arrays;
import java.util.List;

public class FuncContains extends FuncBase {
    public FuncContains() {
        super("contains");
        info = "contains(obj, part)\r\n" +
                "\r\n" +
                "Returns TRUE if the string obj contains part.\r\n" +
                "If obj is a list, set or map, TRUE is returned,\r\n" +
                "if part is contained.\r\n" +
                "\r\n" +
                ": contains('abcdef', 'abc') ==> TRUE\r\n" +
                ": contains('abcdef', 'cde') ==> TRUE\r\n" +
                ": contains('abcdef', 'def') ==> TRUE\r\n" +
                ": contains('abcdef', 'efg') ==> FALSE\r\n" +
                ": contains(NULL, 'abc') ==> FALSE\r\n" +
                ": contains([1, 2, 3], 2) ==> TRUE\r\n" +
                ": <<1, 2, 3>> !> contains(3) ==> TRUE\r\n" +
                ": <<<'a' => 1, 'b' => 2>>> !> contains('b') ==> TRUE\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("obj", "part");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("obj")) return ValueBoolean.FALSE;
        Value obj = args.get("obj");
        if (obj.isList()) {
            for (Value item : obj.asList().getValue()) {
                if (item.isEquals(args.get("part"))) return ValueBoolean.TRUE;
            }
            return ValueBoolean.FALSE;
        } else if (obj.isSet()) {
            return ValueBoolean.from(obj.asSet().getValue().contains(args.get("part")));
        } else if (obj.isMap()) {
            return ValueBoolean.from(obj.asMap().getValue().containsKey(args.get("part")));
        }
        return ValueBoolean.from(obj.asString().getValue().contains(args.getString("part").getValue()));
    }
}
