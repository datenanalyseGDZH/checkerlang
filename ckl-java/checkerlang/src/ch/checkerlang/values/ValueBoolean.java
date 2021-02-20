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

public class ValueBoolean extends Value {
    public static ValueBoolean TRUE = new ValueBoolean(true);
    public static ValueBoolean FALSE = new ValueBoolean(false);

    private boolean value;

    private ValueBoolean(boolean value) {
        this.value = value;
    }

    public static ValueBoolean from(boolean value) {
        return value ? TRUE : FALSE;
    }

    public boolean getValue() {
        return value;
    }

    public boolean isTrue() {
        return value;
    }

    public boolean isFalse() {
        return !value;
    }

    public boolean isEquals(Value value) {
        if (!value.isBoolean()) return false;
        return this.value == value.asBoolean().getValue();
    }

    public int compareTo(Value value) {
        if (!value.isBoolean()) return toString().compareTo(value.toString());
        return Boolean.valueOf(this.value).compareTo(value.asBoolean().value);
    }

    public int hashCode() {
        return Boolean.valueOf(value).hashCode();
    }

    public String type() {
        return "boolean";
    }

    public ValueString asString() {
        return new ValueString(toString());
    }

    public ValueInt asInt() {
        return new ValueInt(value ? 1 : 0);
    }

    public ValueDecimal asDecimal() {
        return new ValueDecimal(value ? 1 : 0);
    }

    public ValueBoolean asBoolean() {
        return this;
    }

    public ValuePattern asPattern() {
        return asString().asPattern();
    }

    public ValueList asList() {
        return new ValueList().addItem(this);
    }

    public boolean isBoolean() {
        return true;
    }

    public String toString() {
        return value ? "TRUE" : "FALSE";
    }

}
