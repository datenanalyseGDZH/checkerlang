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
import ch.checkerlang.values.*;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class FuncMul extends FuncBase {
    public FuncMul() {
        super("mul");
        info = "mul(a, b)\r\n" +
                "\r\n" +
                "Returns the product of a and b. For numerical values this uses the usual arithmetic.\r\n" +
                "If a is a string and b is an int, then the string a is repeated b times. If a is a\r\n" +
                "list and b is an int, then the list is repeated b times.\r\n" +
                "\r\n" +
                ": mul(2, 3) ==> 6\r\n" +
                ": mul('2', 3) ==> '222'\r\n" +
                ": mul([1, 2], 3) ==> [1, 2, 1, 2, 1, 2]\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("a", "b");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value a = args.get("a");
        Value b = args.get("b");

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isString() && b.isInt()) {
            String s = a.asString().getValue();
            StringBuilder r = new StringBuilder();
            for (int i = 0; i < b.asInt().getValue(); i++) {
                r.append(s);
            }
            return new ValueString(r.toString());
        }

        if (a.isList() && b.isInt()) {
            List<Value> result = new ArrayList<>();
            for (int i = 0; i < b.asInt().getValue(); i++) {
                result.addAll(a.asList().getValue());
            }

            return new ValueList(result);
        }

        if (a.isInt() && b.isInt()) {
            return new ValueInt(a.asInt().getValue() * b.asInt().getValue());
        }

        if (a.isNumerical() && b.isNumerical()) {
            return new ValueDecimal(a.asDecimal().getValue() * b.asDecimal().getValue());
        }

        throw new ControlErrorException("Cannot multiply " + a + " by " + b, pos);
    }
}
