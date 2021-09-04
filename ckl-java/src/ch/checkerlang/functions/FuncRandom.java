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

import java.util.Arrays;
import java.util.List;
import java.util.Random;

public class FuncRandom extends FuncBase {
    public static Random random = new Random();

    public FuncRandom() {
        super("random");
        info = "random()\r\n" +
                "random(a)\r\n" +
                "random(a, b)\r\n" +
                "\r\n" +
                "Returns a random number. If no argument is provided, a decimal\r\n" +
                "value in the range [0, 1) is returned. If only a is provided, then \r\n" +
                "an int value in the range [0, a) is returned. If both a and b are\r\n" +
                "provided, then an int value in the range [a, b) is returned.\r\n" +
                "\r\n" +
                ": set_seed(1); random(5) ==> 0\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("a", "b");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.hasArg("a") && !args.hasArg("b")) {
            return new ValueInt(random.nextInt((int) args.getInt("a").getValue()));
        }

        if (args.hasArg("a") && args.hasArg("b")) {
            int a = (int) args.getInt("a").getValue();
            int b = (int) args.getInt("b").getValue();
            return new ValueInt(random.nextInt(b - a) + a);
        }

        return new ValueDecimal(random.nextDouble());
    }
}
