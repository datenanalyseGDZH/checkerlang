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

public class FuncIsEmpty extends FuncBase {
    public FuncIsEmpty() {
        super("is_empty");
        info = "is_empty(obj)\r\n" +
                "\r\n" +
                "Returns TRUE, if the obj is empty.\r\n" +
                "Lists, sets and maps are empty, if they do not contain elements.\r\n" +
                "Strings are empty, if the contain no characters. NULL is always empty.\r\n" +
                "Other objects are converted to a string and if the result is empty, TRUE is returned.\r\n" +
                "\r\n" +
                ": is_empty([]) ==> TRUE\r\n" +
                ": is_empty(set([1, 2])) ==> FALSE\r\n" +
                ": is_empty('') ==> TRUE\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("obj");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value obj = args.get("obj");

        if (obj.isNull()) {
            return ValueBoolean.TRUE;
        }

        if (obj.isNumerical())
        {
            return ValueBoolean.FALSE;
        }

        if (obj.isString()) {
            return ValueBoolean.from(obj.asString().getValue().equals(""));
        }

        if (obj.isList()) {
            return ValueBoolean.from(obj.asList().getValue().size() == 0);
        }

        if (obj.isSet()) {
            return ValueBoolean.from(obj.asSet().getValue().size() == 0);
        }

        if (obj.isMap()) {
            return ValueBoolean.from(obj.asMap().getValue().size() == 0);
        }

        return ValueBoolean.FALSE;
    }
}
