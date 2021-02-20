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
import ch.checkerlang.values.ValueInt;
import ch.checkerlang.values.ValueNull;

import java.util.Arrays;
import java.util.List;

public class FuncCompare extends FuncBase {
    public FuncCompare() {
        super("compare");
        info = "compare(a, b)\r\n" +
                "\r\n" +
                "Returns -1 if a is less than b, 0 if a is equal to b, and 1 if a is greater than b.\r\n" +
                "\r\n" +
                ": compare(1, 2) ==> -1\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("a", "b");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value a = args.get("a");
        Value b = args.get("b");

        if (a.isNull() && b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isInt() && b.isInt()) {
            return new ValueInt(Long.compare(a.asInt().getValue(), b.asInt().getValue()));
        }

        if (a.isNumerical() && b.isNumerical()) {
            return new ValueInt(Double.compare(a.asDecimal().getValue(), b.asDecimal().getValue()));
        }

        if (a.isDate() && b.isDate()) {
            return new ValueInt(a.asDate().getValue().compareTo(b.asDate().getValue()));
        }

        return new ValueInt(a.asString().getValue().compareTo(b.asString().getValue()));
    }
}
