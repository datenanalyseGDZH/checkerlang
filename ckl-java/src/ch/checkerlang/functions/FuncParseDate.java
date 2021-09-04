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
import ch.checkerlang.values.ValueDate;
import ch.checkerlang.values.ValueNull;

import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.*;

public class FuncParseDate extends FuncBase {
    public FuncParseDate() {
        super("parse_date");
        info = "parse_date(str, fmt = 'yyyyMMdd')\r\n" +
                "\r\n" +
                "Parses the string str according to fmt and returns a datetime value.\r\n" +
                "If the format does not match or if the date is invalid, the NULL\r\n" +
                "value is returned.\r\n" +
                "\r\n" +
                "It is possible to pass a list of formats to the fmt parameter.\r\n" +
                "The function sequentially tries to convert the str and if it\r\n" +
                "works, returns the value.\r\n" +
                "\r\n" +
                ": parse_date('20170102') ==> '20170102000000'\r\n" +
                ": parse_date('20170102', fmt = 'yyyyMMdd') ==> '20170102000000'\r\n" +
                ": parse_date('2017010222', fmt = 'yyyyMMdd') ==> NULL\r\n" +
                ": parse_date('20170102', fmt = 'yyyyMMddHH') ==> NULL\r\n" +
                ": parse_date('20170102', fmt = ['yyyyMMdd']) ==> '20170102000000'\r\n" +
                ": parse_date('201701022015', fmt = ['yyyyMMddHHmm', 'yyyyMMddHH', 'yyyyMMdd']) ==> '20170102201500'\r\n" +
                ": parse_date('20170112', fmt = ['yyyyMM', 'yyyy']) ==> NULL\r\n" +
                ": parse_date('20170144') ==> NULL\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("str", "fmt");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("str")) return ValueNull.NULL;
        Value x = args.getString("str");
        String str = x.asString().getValue();
        List<String> fmts = new ArrayList<>();
        if (args.hasArg("fmt")) {
            if (args.get("fmt").isList()) {
                for (Value fmt_ : args.get("fmt").asList().getValue()) {
                    fmts.add(fmt_.asString().getValue());
                }
            } else {
                fmts.add(args.get("fmt").asString().getValue());
            }
        } else {
            fmts.add("yyyyMMdd");
        }

        for (String fmt : fmts) {
            try {
                Date date;
                if (fmt.equals("yyyy")) {
                    if (str.length() != 4) continue;
                    Calendar cal = Calendar.getInstance();
                    cal.set(Integer.parseInt(str), 0, 1, 0, 0, 0);
                    date = cal.getTime();
                } else {
                    DateFormat format = new SimpleDateFormat(fmt);
                    format.setLenient(false);
                    date = format.parse(str);
                    String str2 = format.format(date);
                    if (!str.equals(str2)) continue;
                }
                return new ValueDate(date);
            } catch (ParseException e) {
                // continue;
            }
        }

        return ValueNull.NULL;
    }
}
