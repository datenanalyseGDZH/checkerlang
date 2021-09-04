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
import ch.checkerlang.values.ValueMap;
import ch.checkerlang.values.ValueNull;

import java.util.Arrays;
import java.util.List;

public class FuncZipMap extends FuncBase {
    public FuncZipMap() {
        super("zip_map");
        info = "zip_map(a, b)\r\n" +
                "\r\n" +
                "Returns a map where the key of each entry is taken from a,\r\n" +
                "and where the value of each entry is taken from b, where\r\n" +
                "a and b are lists of identical length.\r\n" +
                "\r\n" +
                ": zip_map(['a', 'b', 'c'], [1, 2, 3]) ==> <<<'a' => 1, 'b' => 2, 'c' => 3>>>\r\n";
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
            ValueMap result = new ValueMap();
            for (int i = 0; i < Math.min(lista.size(), listb.size()); i++)
            {
                result.addItem(lista.get(i), listb.get(i));
            }

            return result;
        }

        throw new ControlErrorException("Cannot zip_map " + a + " and " + b, pos);
    }
}
