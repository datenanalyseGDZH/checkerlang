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

import ch.checkerlang.*;
import ch.checkerlang.values.*;

import java.util.*;

public class FuncSub extends FuncBase {
    public FuncSub() {
        super("sub");
        info = "sub(a, b)\r\n" +
                "\r\n" +
                "Returns the subtraction of b from a. For numerical values this uses usual arithmetic.\r\n" +
                "For lists and sets, this returns lists and sets minus the element b. If a is a datetime\r\n" +
                "value and b is datetime value, then the date difference is returned. If a is a datetime\r\n" +
                "value and b is a numeric value, then b is interpreted as number of days and the corresponding\r\n" +
                "datetime after subtracting these number of days is returned.\r\n" +
                "\r\n" +
                ": sub(1, 2) ==> -1\r\n" +
                ": sub([1, 2, 3], 2) ==> [1, 3]\r\n" +
                ": sub(date('20170405'), date('20170402')) ==> 3\r\n" +
                ": sub(date('20170405'), 3) ==> '20170402'\r\n" +
                ": sub(set([3, 1, 2]), 2) ==> <<1, 3>>";
    }

    public List<String> getArgNames() {
        return Arrays.asList("a", "b");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value a = args.get("a");
        Value b = args.get("b");

        if (a.isList()) {
            ValueList result = new ValueList();
            for (Value item : a.asList().getValue()) {
                boolean add = true;
                for (Value val : args.getAsList("b").getValue()) {
                    if (!item.isEquals(val)) continue;
                    add = false;
                    break;
                }
                if (add) result.addItem(item);
            }

            return result;
        }

        if (a.isSet()) {
            Set<Value> minus = new TreeSet<>();
            if (b.isSet()) {
                minus = b.asSet().getValue();
            } else if (b.isList()) {
                for (Value element : b.asList().getValue()) {
                    minus.add(element);
                }
            } else {
                minus.add(b);
            }
            ValueSet result = new ValueSet();
            for (Value element : a.asSet().getValue()) {
                if (!minus.contains(element)) result.addItem(element);
            }
            return result;
        }

        if (a.isDate()) {
            if (b.isDate()) {
                long diff = a.asInt().getValue() - b.asInt().getValue();
                return new ValueInt(diff);
            }
            return new ValueDate(DateConverter.convertOADateToDate(DateConverter.convertDateToOADate(a.asDate().getValue()) - args.getAsDecimal("b").getValue()));
        }

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isInt() && b.isInt()) {
            return new ValueInt(a.asInt().getValue() - b.asInt().getValue());
        }

        if (a.isNumerical() && b.isNumerical()) {
            return new ValueDecimal(a.asDecimal().getValue() - b.asDecimal().getValue());
        }

        throw new ControlErrorException("Cannot subtract " + b + " from " + a, pos);
    }
}
