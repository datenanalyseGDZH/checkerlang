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

import ch.checkerlang.*;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueDecimal;
import ch.checkerlang.values.ValueInt;
import ch.checkerlang.values.ValueNull;

import java.util.Arrays;
import java.util.List;

public class FuncDiv extends FuncBase {
    public FuncDiv() {
        super("div");
        info = "div(a, b)\r\n" +
                "\r\n" +
                "Returns the value of a divided by b. If both values are ints,\r\n" +
                "then the result is also an int. Otherwise, it is a decimal.\r\n" +
                "\r\n" +
                ": div(6, 2) ==> 3\r\n";
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

        if (a.isInt() && b.isInt()) {
            long divisor = b.asInt().getValue();
            if (divisor == 0) {
                if (environment.isDefined("DIV_0_VALUE") &&
                        environment.get("DIV_0_VALUE", pos) != ValueNull.NULL) {
                    return environment.get("DIV_0_VALUE", pos);
                }
                throw new ControlDivideByZeroException("divide by zero", pos);
            }
            return new ValueInt(a.asInt().getValue() / divisor);
        }

        if (a.isNumerical() && b.isNumerical()) {
            double divisor = b.asDecimal().getValue();
            if (divisor == 0.0) {
                if (environment.isDefined("DIV_0_VALUE") &&
                        environment.get("DIV_0_VALUE", pos) != ValueNull.NULL) {
                    return environment.get("DIV_0_VALUE", pos);
                }
                throw new ControlDivideByZeroException("divide by zero", pos);
            }
            return new ValueDecimal(a.asDecimal().getValue() / divisor);
        }

        throw new ControlErrorException("Cannot divide values", pos);
    }
}
