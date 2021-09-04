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
import ch.checkerlang.values.ValueList;

import java.util.Arrays;
import java.util.List;

public class FuncRange extends FuncBase {
    public FuncRange() {
        super("range");
        info = "range(a)\r\n" +
                "range(a, b)\r\n" +
                "range(a, b, step)\r\n" +
                "\r\n" +
                "Returns a list containing int values in the range. If only a is\r\n" +
                "provided, the range is [0, a). If both a and b are provided, the\r\n" +
                "range is [a, b). If step is given, then only every step element\r\n" +
                "is included in the list.\r\n" +
                "\r\n" +
                ": range(4) ==> [0, 1, 2, 3]\r\n" +
                ": range(3, 6) ==> [3, 4, 5]\r\n" +
                ": range(10, step = 3) ==> [0, 3, 6, 9]\r\n" +
                ": range(10, 0, step = -2) ==> [10, 8, 6, 4, 2]\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("a", "b", "step");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        int start = 0;
        int end = 0;
        int step = 1;
        if (args.hasArg("a") && !args.hasArg("b")) {
            end = (int) args.getInt("a").getValue();
        } else if (args.hasArg("a") && args.hasArg("b")) {
            start = (int) args.getInt("a").getValue();
            end = (int) args.getInt("b").getValue();
        }
        if (args.hasArg("step")) {
            step = (int) args.getInt("step").getValue();
        }

        ValueList result = new ValueList();
        int i = start;
        if (step > 0) {
            while (i < end) {
                result.addItem(new ValueInt(i));
                i += step;
            }
        } else if (step < 0) {
            while (i > end) {
                result.addItem(new ValueInt(i));
                i += step;
            }
        }
        return result;
    }
}
