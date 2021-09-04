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
import ch.checkerlang.values.*;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class FuncReplace extends FuncBase {
    public FuncReplace() {
        super("replace");
        info = "replace(s, a, b, start = 0)\r\n" +
                "\r\n" +
                "Replaces all occurences of a in the string s with b.\r\n" +
                "The optional parameter start specifies the start index.\r\n" +
                "\r\n" +
                ": replace('abc', 'b', 'x') ==> 'axc'\r\n" +
                ": replace('abcbcbca', 'b', 'x') ==> 'axcxcxca'\r\n" +
                ": replace('abc', 'b', 'xy') ==> 'axyc'\r\n" +
                ": replace('abcdef', 'bcd', 'xy') ==> 'axyef'\r\n" +
                ": replace('abcabcabc', 'abc', 'xy', start = 3) ==> 'abcxyxy'\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("s", "a", "b", "start");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("s")) return ValueNull.NULL;
        String s = args.getString("s").getValue();
        String a = args.getString("a").getValue();
        String b = args.getString("b").getValue();
        int start = (int) args.getInt("start", 0L).getValue();
        if (start >= s.length()) return args.getString("s");
        if (start == 0) {
            return new ValueString(s.replace(a, b));
        }
        String prefix = s.substring(0, (int) start);
        return new ValueString(prefix + s.substring((int) start).replace(a, b));
    }
}
