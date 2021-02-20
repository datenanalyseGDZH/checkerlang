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
import ch.checkerlang.values.ValueInt;

import java.util.Arrays;
import java.util.List;

public class FuncLength extends FuncBase {
    public FuncLength() {
        super("length");
        info = "length(obj)\r\n" +
                "\r\n" +
                "Returns the length of obj. This only works for strings, lists, sets and maps.\r\n" +
                "\r\n" +
                ": length('123') ==> 3\r\n" +
                ": length([1, 2, 3]) ==> 3\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("obj");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value arg = args.get("obj");
        if (arg.isString()) return new ValueInt(arg.asString().getValue().length());
        if (arg.isList()) return new ValueInt(arg.asList().getValue().size());
        if (arg.isSet()) return new ValueInt(arg.asSet().getValue().size());
        if (arg.isMap()) return new ValueInt(arg.asMap().getValue().size());
        throw new ControlErrorException("Cannot determine length of " + arg, pos);
    }
}
