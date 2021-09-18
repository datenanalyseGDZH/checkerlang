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

import java.util.Arrays;
import java.util.List;

public class FuncBitShiftRight extends FuncBase {
    public FuncBitShiftRight() {
        super("bit_shift_right");
        info = "bit_shift_right(a, n)\r\n" +
                "\r\n" +
                "Performs bitwise shift of 32bit value a by n bits to the right.\r\n" +
                ": bit_shift_right(4, 1) ==> 2\r\n" +
                ": bit_shift_right(4, 3) ==> 0\r\n" +
                ": bit_shift_right(4, 2) ==> 1\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("a", "n");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        long a = args.getInt("a").getValue();
        int n = (int) args.getInt("n").getValue();
        return new ValueInt((a >>> n) & 4294967295L);
    }
}
