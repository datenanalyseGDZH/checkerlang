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
import ch.checkerlang.values.ValueDecimal;
import ch.checkerlang.values.ValueInt;
import ch.checkerlang.values.ValueNull;

import java.util.Arrays;
import java.util.List;

public class FuncPow extends FuncBase {
    public FuncPow() {
        super("pow");
        info = "pow(x, y)\r\n" +
                "\r\n" +
                "Returns the power x ^ y.\r\n" +
                "\r\n" +
                ": pow(2, 3) ==> 8\r\n" +
                ": pow(2.5, 2) ==> 6.25\r\n" +
                ": pow(4, 2) ==> 16\r\n" +
                ": pow(4.0, 2.0) ==> 16.0\r\n" +
                ": round(pow(2, 1.5), digits = 3) ==> 2.828\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("x", "y");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        if (args.isNull("y")) return ValueNull.NULL;
        if (args.get("y").isInt()) {
            if (args.get("x").isInt()) {
                long x = args.getInt("x").getValue();
                long y = args.getInt("y").getValue();
                long result = 1;
                for (int i = 0; i < y; i++) {
                    result *= x;
                }
                return new ValueInt(result);
            } else {
                double x = args.getDecimal("x").getValue();
                long y = args.getInt("y").getValue();
                double result = 1.0;
                for (int i = 0; i < y; i++) {
                    result *= x;
                }
                return new ValueDecimal(result);
            }
        } else {
            return new ValueDecimal(Math.pow(args.getNumerical("x").getValue(), args.getNumerical("y").getValue()));
        }
    }
}
