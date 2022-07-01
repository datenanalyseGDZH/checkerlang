/*  Copyright (c) 2022 Damian Brunold, Gesundheitsdirektion Kanton Zürich

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
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class FuncSprintf extends FuncBase {
    private Pattern PLACEHOLDER_PATTERN = Pattern.compile("\\{([0-9]+)(#[^}]+)?\\}");

    public FuncSprintf() {
        super("sprintf");

        info = "sprintf(fmt, args...)\r\n" +
            "\r\n" +
            "Formats a string format using the provided args. Each\r\n" +
            "value can be referred to in the fmt string using the\r\n" +
            "{0} syntax, where 0 means the first argument passed.\r\n" +
            "\r\n" +
            "This uses the same formatting suffixes as the s function.\r\n" + 
            "See there for an explanation of available formatting suffixes.\r\n" +
            "\r\n" +
            ": sprintf('{0} {1}', 1, 2) ==> '1 2'\r\n" +
            ": sprintf('{0} {1}', 'a', 'b') ==> 'a b'\r\n" +
            ": sprintf('{0#5} {1#5}', 1, 2) ==> '    1     2'\r\n" +
            ": sprintf('{0#-5} {1#-5}', 1, 2) ==> '1     2    '\r\n" +
            ": sprintf('{0#05} {1#05}', 1, 2) ==> '00001 00002'\r\n" +
            ": require Math; sprintf('{0#.4}', Math->PI) ==> '3.1416'\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("fmt", "args...");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("fmt")) return ValueNull.NULL;
        String fmt = args.getString("fmt").getValue();
        List<Value> arguments = args.get("args...").asList().getValue();
        String result = "";
        int lastidx = 0;
        Matcher matcher = PLACEHOLDER_PATTERN.matcher(fmt);
        while (matcher.find()) {
            int start = matcher.start(0);
            int end = matcher.end(0);
            int var = Integer.parseInt(matcher.group(1));
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

            String value;
            if (base != 10) {
                value = String.format("%x", arguments.get(var).asInt().getValue());
            } else if (digits != -1) {
                value = String.format("%." + digits + "f", arguments.get(var).asDecimal().getValue());
            } else {
                value = arguments.get(var).asString().getValue();
            }
            while (value.length() < width) {
                if (leading) value = ' ' + value;
                else if (zeroes) value = '0' + value;
                else value = value + ' ';
            }

            if (lastidx < start) result += fmt.substring(lastidx, start);
            result += value;
            lastidx = end;
        }
        if (lastidx < fmt.length()) result += fmt.substring(lastidx);
        return new ValueString(result);
    }
}
