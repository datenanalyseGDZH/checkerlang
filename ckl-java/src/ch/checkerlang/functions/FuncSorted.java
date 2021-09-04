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
import ch.checkerlang.values.ValueFunc;
import ch.checkerlang.values.ValueList;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class FuncSorted extends FuncBase {
    public FuncSorted() {
        super("sorted");
        info = "sorted(lst, cmp=compare, key=identity)\r\n" +
                "\r\n" +
                "Returns a sorted copy of the list. This is sorted according to the\r\n" +
                "value returned by the key function for each element of the list.\r\n" +
                "The values are compared using the compare function cmp.\r\n" +
                "\r\n" +
                ": sorted([3, 2, 1]) ==> [1, 2, 3]\r\n" +
                ": sorted([6, 2, 5, 3, 1, 4]) ==> [1, 2, 3, 4, 5, 6]\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("lst", "cmp", "key");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Environment env = environment.newEnv();
        ValueList lst = args.getAsList("lst");
        ValueFunc cmp = args.hasArg("cmp") ? args.getFunc("cmp") : environment.get("compare", pos).asFunc();
        ValueFunc key = args.hasArg("key") ? args.getFunc("key") : environment.get("identity", pos).asFunc();
        List<Value> result = new ArrayList<>();
        result.addAll(lst.getValue());
        for (int i = 1; i < result.size(); i++) {
            Value v = key.execute(new Args(key.getArgNames().get(0), result.get(i), pos), env, pos);
            for (int j = i - 1; j >= 0; j--) {
                Value v2 = key.execute(new Args(key.getArgNames().get(0), result.get(j), pos), env, pos);
                Args cmpargs = new Args(cmp.getArgNames().get(0), cmp.getArgNames().get(1), v, v2, pos);
                int comparison = (int) cmp.execute(cmpargs, env, pos).asInt().getValue();
                if (comparison < 0) {
                    Value temp = result.get(j + 1);
                    result.set(j + 1, result.get(j));
                    result.set(j, temp);
                } else {
                    break;
                }
            }
        }
        return new ValueList(result);
    }
}
