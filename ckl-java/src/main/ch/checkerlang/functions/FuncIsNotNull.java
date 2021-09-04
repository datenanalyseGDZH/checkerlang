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

public class FuncIsNotNull extends FuncBase {
    public FuncIsNotNull() {
        super("is_not_null");
        info = "is_not_null(obj)\r\n" +
                "\r\n" +
                "Returns TRUE, if the obj is not NULL.\r\n" +
                "\r\n" +
                ": is_not_null('') ==> TRUE\r\n" +
                ": is_not_null(1) ==> TRUE\r\n" +
                ": is_not_null(NULL) ==> FALSE\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("obj");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        return ValueBoolean.from(!args.get("obj").isNull());
    }
}
