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
import ch.checkerlang.values.ValueNull;
import ch.checkerlang.values.ValueString;

import java.util.Arrays;
import java.util.List;

public class FuncS extends FuncBase {
    public FuncS() {
        super("s");
        info = "s(str, start = 0)\r\n" +
                "\r\n" +
                "Returns a string, where all placeholders are replaced with their\r\n" +
                "appropriate values. Placeholder have the form '{var}' and result\r\n" +
                "in the value of the variable var inserted at this location.\r\n" +
                "\r\n" +
                ": def name = 'damian'; s('hello {name}') ==> 'hello damian'\r\n" +
                ": def foo = '{bar}'; def bar = 'baz'; s('{foo}{bar}') ==> '{bar}baz'\r\n" +
                ": s('{PI} is cool') ==> '3.141592653589793 is cool'\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("str", "start");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("str")) return ValueNull.NULL;
        String str = args.getString("str").getValue();
        int start = (int) args.getInt("start", 0).getValue();
        if (start < 0) start = str.length() + start;
        while (true) {
            int idx1 = str.indexOf('{', start);
            if (idx1 == -1) return new ValueString(str);
            int idx2 = str.indexOf('}', idx1 + 1);
            if (idx2 == -1) return new ValueString(str);
            String var = str.substring(idx1 + 1, idx2);
            String value = environment.get(var, pos).asString().getValue();
            str = str.substring(0, idx1) + value + str.substring(idx2 + 1);
            start = idx1 + value.length();
        }
    }
}
