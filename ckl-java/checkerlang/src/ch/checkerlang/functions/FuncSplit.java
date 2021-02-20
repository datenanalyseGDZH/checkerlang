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

public class FuncSplit extends FuncBase {
    public FuncSplit() {
        super("split");
        info = "split(str, delim = '[ \\t]+')\r\n" +
                "\r\n" +
                "Splits the string str into parts and returns a list of strings.\r\n" +
                "The delim is a regular expression. Default is spaces or tabs.\r\n" +
                "\r\n" +
                ": split('a,b,c', //,//) ==> ['a', 'b', 'c']\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("str", "delim");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("str")) return ValueNull.NULL;

        String value = args.getString("str").getValue();
        String delim = args.getAsPattern("delim", new ValuePattern("[ \\t]+")).getValue();

        return splitValue(value, delim);
    }

    public static ValueList splitValue(String value, String delim) {
        if (value.equals("")) return new ValueList();

        List<Value> values = new ArrayList<>();
        String[] parts = value.split(delim);
        for (String part : parts) {
            values.add(new ValueString(part));
        }
        return new ValueList(values);
    }

}
