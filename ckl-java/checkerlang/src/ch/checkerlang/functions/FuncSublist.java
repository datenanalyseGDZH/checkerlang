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
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueNull;

import java.util.Arrays;
import java.util.List;

public class FuncSublist extends FuncBase {
    public FuncSublist() {
        super("sublist");
        info = "sublist(lst, startidx)\r\n" +
                "sublist(lst, startidx, endidx)\r\n" +
                "\r\n" +
                "Returns the sublist starting with startidx. If endidx is provided,\r\n" +
                "this marks the end of the sublist. Endidx is not included.\r\n" +
                "\r\n" +
                ": sublist([1, 2, 3, 4], 2) ==> [3, 4]\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("lst", "startidx", "endidx");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        if (args.isNull("lst")) return ValueNull.NULL;
        List<Value> value = args.getList("lst").getValue();
        int start = (int) args.getInt("startidx").getValue();
        if (start < 0) start = value.size() + start;
        if (start > value.size()) return new ValueList();
        int end = (int) args.getInt("endidx", value.size()).getValue();
        if (end < 0) end = value.size() + end;
        if (end > value.size()) end = value.size();
        ValueList result = new ValueList();
        for (int i = start; i < end; i++) {
            result.addItem(value.get(i));
        }
        return result;
    }
}
