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
import ch.checkerlang.values.ValueNull;

import java.util.Arrays;
import java.util.List;

public class FuncOrd extends FuncBase {
    public FuncOrd() {
        super("ord");
        info = "ord(ch)\r\n" +
                "\r\n" +
                "Returns the code point integer of the character ch.\r\n" +
                "\r\n" +
                ": ord('a') ==> 97\r\n" +
                ": ord(' ') ==> 32\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("ch");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("ch")) return ValueNull.NULL;
        return new ValueInt(Character.codePointAt(args.getString("ch").getValue(), 0));
    }
}
