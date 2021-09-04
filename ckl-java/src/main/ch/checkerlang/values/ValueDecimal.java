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

public class ValueDecimal extends Value {
    private double value;

    public ValueDecimal(double value) {
        this.value = value;
    }

    public double getValue() {
        return value;
    }

    public boolean isEquals(Value value) {
        if (!value.isNumerical()) return false;
        return getValue() == value.asDecimal().getValue();
    }

    public int compareTo(Value value) {
        if (!value.isNumerical()) return toString().compareTo(value.toString());
        return Double.compare(this.value, value.asDecimal().value);
    }

    public int hashCode() {
        return Double.valueOf(value).hashCode();
    }

    public String type() {
        return "decimal";
    }

    public ValueString asString() {
        return new ValueString(toString());
    }

    public ValueInt asInt() {
        return new ValueInt((long) value);
    }

    public ValueDecimal asDecimal() {
        return this;
    }

    public ValueDate asDate() {
        return new ValueDate(DateConverter.convertOADateToDate(value));
    }

    public ValueList asList() {
        return new ValueList().addItem(this);
    }

    public boolean isDecimal() {
        return true;
    }

    public String toString() {
        return Double.toString(value);
    }

}
