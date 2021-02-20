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
import ch.checkerlang.ControlErrorException;
import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueNull;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class FuncZip extends FuncBase {
    public FuncZip() {
        super("zip");
        info = "zip(a, b)\r\n" +
                "\r\n" +
                "Returns a list where each element is a list of two items.\r\n" +
                "The first of the two items is taken from the first list,\r\n" +
                "the second from the second list. The resulting list has\r\n" +
                "the same length as the shorter of the two input lists.\r\n" +
                "\r\n" +
                ": zip([1, 2, 3], [4, 5, 6, 7]) ==> [[1, 4], [2, 5], [3, 6]]\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("a", "b");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value a = args.get("a");
        Value b = args.get("b");

        if (a.isNull() || b.isNull())
        {
            return ValueNull.NULL;
        }

        if (a.isList() && b.isList())
        {
            List<Value> lista = a.asList().getValue();
            List<Value> listb = b.asList().getValue();
            List<Value> result = new ArrayList<>();
            for (int i = 0; i < Math.min(lista.size(), listb.size()); i++)
            {
                List<Value> pair = new ArrayList<>();
                pair.add(lista.get(i));
                pair.add(listb.get(i));
                result.add(new ValueList(pair));
            }

            return new ValueList(result);
        }

        throw new ControlErrorException("Cannot zip " + a + " and " + b, pos);
    }
}
