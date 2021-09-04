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
import ch.checkerlang.Parser;
import ch.checkerlang.SourcePos;
import ch.checkerlang.nodes.Node;
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
                "The placeholder can also be expressions and their result will\r\n" +
                "be inserted instead of the placeholder.\r\n" +
                "\r\n" +
                "There are formatting suffixes to the placeholder, which allow\r\n" +
                "some control over the formatting. They formatting spec starts after\r\n" +
                "a # character and consists of align/fill, width and precision fields.\r\n" +
                "For example #06.2 will format the decimal to a width of six characters\r\n" +
                "and uses two digits after the decimal point. If the number is less than\r\n" +
                "six characters wide, then it is prefixed with zeroes until the width\r\n" +
                "is reached, e.g. '001.23'. Please refer to the examples below.\r\n" +
                "\r\n" +
                ": def name = 'damian'; s('hello {name}') ==> 'hello damian'\r\n" +
                ": def foo = '{bar}'; def bar = 'baz'; s('{foo}{bar}') ==> '{bar}baz'\r\n" +
                ": def a = 'abc'; s('a = {a#5}') ==> 'a =   abc'\r\n" +
                ": def a = 'abc'; s('a = {a#-5}') ==> 'a = abc  '\r\n" +
                ": def n = 12; s('n = {n#5}') ==> 'n =    12'\r\n" +
                ": def n = 12; s('n = {n#-5}') ==> 'n = 12   '\r\n" +
                ": def n = 12; s('n = {n#05}') ==> 'n = 00012'\r\n" +
                ": def n = 1.2345678; s('n = {n#.2}') ==> 'n = 1.23'\r\n" +
                ": def n = 1.2345678; s('n = {n#06.2}') ==> 'n = 001.23'\r\n" +
                ": s('2x3 = {2*3}') ==> '2x3 = 6'\r\n" +
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
            int width = 0;
            boolean zeroes = false;
            boolean leading = true;
            int digits = -1;
            int idx3 = var.indexOf('#');
            if (idx3 != -1) {
                String spec = var.substring(idx3 + 1);
                var = var.substring(0, idx3);
                if (spec.startsWith("-")) {
                    leading = false;
                    spec = spec.substring(1);
                }
                if (spec.startsWith("0")) {
                    zeroes = true;
                    leading = false;
                    spec = spec.substring(1);
                }
                int idx4 = spec.indexOf('.');
                if (idx4 == -1) {
                    digits = -1;
                    width = Integer.parseInt(spec);
                } else {
                    digits = Integer.parseInt(spec.substring(idx4 + 1));
                    width = idx4 == 0 ? 0 : Integer.parseInt(spec.substring(0, idx4));
                }
            }
            String value = var;
            try {
                Node node = Parser.parse(var, pos.filename);
                value = node.evaluate(environment).asString().getValue();
                if (digits != -1) value = String.format("%." + digits + "f", Double.parseDouble(value));
                while (value.length() < width) {
                    if (leading) value = ' ' + value;
                    else if (zeroes) value = '0' + value;
                    else value = value + ' ';
                }
            } catch (Exception e) {
                // ignore
            }
            str = str.substring(0, idx1) + value + str.substring(idx2 + 1);
            start = idx1 + value.length();
        }
    }
}
