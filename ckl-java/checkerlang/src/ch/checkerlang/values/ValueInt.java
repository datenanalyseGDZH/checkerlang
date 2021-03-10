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
package ch.checkerlang.values;

import ch.checkerlang.DateConverter;

public class ValueInt extends Value {
    private long value;

    public ValueInt(long value) {
        this.value = value;
    }

    public long getValue() {
        return value;
    }

    public boolean isEquals(Value value) {
        if (!value.isNumerical()) return false;
        if (value.isDecimal()) return asDecimal().isEquals(value);
        return this.value == value.asInt().getValue();
    }

    public int compareTo(Value value) {
        if (!value.isNumerical()) return asString().toString().compareTo(value.toString());
        if (value.isDecimal()) return asDecimal().compareTo(value);
        return Long.compare(this.value, value.asInt().value);
    }

    public int hashCode() {
        return Long.valueOf(value).hashCode();
    }

    public String type() {
        return "int";
    }

    public ValueString asString() {
        return new ValueString(toString());
    }


    public ValueInt asInt() {
        return this;
    }

    public ValueDecimal asDecimal() {
        return new ValueDecimal((double) value);
    }

    public ValueBoolean asBoolean() {
        return ValueBoolean.from(value != 0);
    }

    public ValueDate asDate() {
        return new ValueDate(DateConverter.convertOADateToDate(value));
    }

    public ValueList asList() {
        return new ValueList().addItem(this);
    }

    public boolean isInt() {
        return true;
    }

    public String toString() {
        return Long.toString(value);
    }

}
