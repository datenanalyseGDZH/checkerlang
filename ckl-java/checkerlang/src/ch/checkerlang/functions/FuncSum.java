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
import ch.checkerlang.values.ValueDecimal;
import ch.checkerlang.values.ValueInt;
import ch.checkerlang.values.ValueNull;

import java.util.*;

public class FuncSum extends FuncBase {
    public FuncSum() {
        super("sum");
        info = "sum(list, ignore = [])\r\n" +
                "\r\n" +
                "Returns the sum of a list of numbers. Values contained in the optional list ignore\r\n" +
                "are counted as 0.\r\n" +
                "\r\n" +
                ": sum([1, 2, 3]) ==> 6\r\n" +
                ": sum([1, 2.5, 3]) ==> 6.5\r\n" +
                ": sum([1, 2.5, 1.5, 3]) ==> 8.0\r\n" +
                ": sum([1.0, 2.0, 3.0]) ==> 6.0\r\n" +
                ": sum([1.0, 2, -3.0]) ==> 0.0\r\n" +
                ": sum([1, 2, -3]) ==> 0\r\n" +
                ": sum([1, '1', 1], ignore = ['1']) ==> 2\r\n" +
                ": sum(range(101)) ==> 5050\r\n" +
                ": sum([]) ==> 0\r\n" +
                ": sum([NULL], ignore = [NULL]) ==> 0\r\n" +
                ": sum([1, NULL, 3], ignore = [NULL]) ==> 4\r\n" +
                ": sum([1, NULL, '', 3], ignore = [NULL, '']) ==> 4\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("list", "ignore");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("list")) return ValueNull.NULL;

        List<Value> list = args.getList("list").getValue();
        List<Value> ignore = new ArrayList<>();

        if (args.hasArg("ignore"))
        {
            ignore.addAll(args.getList("ignore").getValue());
        }

        double resultdecimal = (double) 0;
        long resultint = 0L;
        boolean decimalrequired = false;

        for (Value value : list) {
            boolean skipvalue = false;
            for (Value ignoreval : ignore) {
                if (ignoreval.isEquals(value)) {
                    skipvalue = true;
                    break;
                }
            }
            if (skipvalue) continue;

            if (decimalrequired) {
                resultdecimal += value.asDecimal().getValue();
            } else if (value.isInt()) {
                resultint += value.asInt().getValue();
            } else if (value.isDecimal()) {
                if (!decimalrequired) {
                    decimalrequired = true;
                    resultdecimal = resultint;
                }

                resultdecimal += value.asDecimal().getValue();
            } else {
                throw new ControlErrorException("Cannot sum " + value, pos);
            }
        }

        if (decimalrequired) {
            return new ValueDecimal(resultdecimal);
        }
        return new ValueInt(resultint);    }
}
