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

import java.util.Arrays;
import java.util.List;

public class FuncAdd extends FuncBase {
    public FuncAdd() {
        super("add");
        info = "add(a, b)\r\n" +
                "\r\n" +
                "Returns the sum of a and b. For numerical values this uses usual arithmetic.\r\n" +
                "For lists and strings it concatenates. For sets it uses union.\r\n" +
                "\r\n" +
                ": add(1, 2) ==> 3\r\n" +
                ": add(date('20100201'), 3) ==> '20100204000000'\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("a", "b");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value a = args.get("a");
        Value b = args.get("b");

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isInt() && b.isInt()) {
            return new ValueInt(a.asInt().getValue() + b.asInt().getValue());
        }

        if (a.isNumerical() && b.isNumerical()) {
            return new ValueDecimal(a.asDecimal().getValue() + b.asDecimal().getValue());
        }

        if (a.isList()) {
            if (b.isCollection()) {
                return new ValueList(a.asList().getValue()).addItems(b.asList().getValue());
            } else {
                return new ValueList(a.asList().getValue()).addItem(b);
            }
        }

        if (a.isSet()) {
            if (b.isCollection()) {
                return new ValueSet(a.asSet().getValue()).addItems(b.asSet().getValue());
            } else {
                return new ValueSet(a.asSet().getValue()).addItem(b);
            }
        }

        if (b.isList()) {
            ValueList result = new ValueList();
            result.addItem(a);
            result.addItems(b.asList().getValue());
            return result;
        }

        if (b.isSet()) {
            ValueSet result = new ValueSet();
            result.addItem(a);
            result.addItems(b.asSet().getValue());
            return result;
        }

        if (a.isDate() && b.isNumerical()) {
            return new ValueDate(DateConverter.convertOADateToDate(DateConverter.convertDateToOADate(a.asDate().getValue()) + args.getAsDecimal("b").asDecimal().getValue()));
        }

        if ((a.isString() && b.isAtomic()) || (a.isAtomic() && b.isString())) {
            return new ValueString(a.asString().getValue() + b.asString().getValue());
        }

        throw new ControlErrorException("Cannot add " + a + " and " + b, pos);
    }
}
