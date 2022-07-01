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
import java.util.regex.Matcher;
import java.util.regex.Pattern;

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
                ": def n = 123; s('n = {n#x}') ==> 'n = 7b'\r\n" +
                ": def n = 255; s('n = {n#04x}') ==> 'n = 00ff'\r\n" +
                ": s('{1} { {2}') ==> '1 { 2'\r\n" +
                ": s('{PI} is cool') ==> '3.141592653589793 is cool'\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("str", "start");
    }

    private static Pattern PLACEHOLDER_PATTERN = Pattern.compile("\\{([^#{}]+)(#-?[0-9.]*x?)?\\}");
    
    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("str")) return ValueNull.NULL;
        String str = args.getString("str").getValue();
        int start_ = (int) args.getInt("start", 0).getValue();
        if (start_ < 0) start_ = str.length() + start_;

        String result = "";
        if (start_ > 0) {
            result = str.substring(0, start_);
            str = str.substring(start_);
        }

        int lastidx = 0;
        Matcher matcher = PLACEHOLDER_PATTERN.matcher(str);
        while (matcher.find()) {
            int start = matcher.start(0);
            int end = matcher.end(0);
            String expr = matcher.group(1);
            String spec = matcher.group(2);

            int width = 0;
            boolean zeroes = false;
            boolean leading = true;
            int base = 10;
            int digits = -1;

            if (spec != null) {
                spec = spec.substring(1); // skip #
                if (spec.startsWith("-")) {
                    leading = false;
                    spec = spec.substring(1);
                }
                if (spec.startsWith("0")) {
                    zeroes = true;
                    leading = false;
                    spec = spec.substring(1);
                }
                if (spec.endsWith("x")) {
                    base = 16;
                    spec = spec.substring(0, spec.length() - 1);
                }
                int idx = spec.indexOf('.');
                if (idx == -1) {
                    digits = -1;
                    width = Integer.parseInt(spec.isEmpty() ? "0" : spec);
                } else {
                    digits = Integer.parseInt(spec.substring(idx + 1));
                    width = idx == 0 ? 0 : Integer.parseInt(spec.substring(0, idx));
                }
            }

            String value = "";
            try {
                Node node = Parser.parse(expr, pos.filename);
                Value val = node.evaluate(environment);
                if (base != 10) value = String.format("%x", val.asInt().getValue());
                else if (digits != -1) value = String.format("%." + digits + "f", val.asDecimal().getValue());
                else value = val.asString().getValue();
                while (value.length() < width) {
                    if (leading) value = ' ' + value;
                    else if (zeroes) value = '0' + value;
                    else value = value + ' ';
                }
            } catch (Exception e) {
                // ignore
            }

            if (lastidx < start) result += str.substring(lastidx, start);
            result += value;
            lastidx = end;
        }
        if (lastidx < str.length()) result += str.substring(lastidx);

        return new ValueString(result);
    }
}
