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

import java.util.Arrays;
import java.util.List;

public class FuncCompare extends FuncBase {
    public FuncCompare() {
        super("compare");
        info = "compare(a, b)\r\n" +
                "\r\n" +
                "Returns an int less than 0 if a is less than b,\r\n" +
                "0 if a is equal to b, and an int greater than 0\r\n" +
                "if a is greater than b.\r\n" +
                "\r\n" +
                ": compare(1, 2) < 0 ==> TRUE\r\n" +
                ": compare(3, 1) > 0 ==> TRUE\r\n" +
                ": compare(1, 1) == 0 ==> TRUE\r\n" +
                ": compare('1', 2) < 0 ==> TRUE\r\n" +
                ": compare('2', 1) < 0 ==> TRUE\r\n" +
                ": compare(100, '100') > 0 ==> TRUE\r\n" +
                ": compare(NULL, 1) > 0 ==> TRUE\r\n" +
                ": compare(NULL, NULL) == 0 ==> TRUE\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("a", "b");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value a = args.get("a");
        Value b = args.get("b");
        return new ValueInt(a.compareTo(b));
    }
}
