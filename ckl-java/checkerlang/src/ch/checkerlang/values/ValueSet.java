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

import java.util.Collection;
import java.util.TreeSet;

public class ValueSet extends Value {
    private TreeSet<Value> value = new TreeSet<>();

    public ValueSet() {
        // empty
    }

    public ValueSet(TreeSet<Value> value) {
        for (Value item : value) {
            this.value.add(item);
        }
    }

    public ValueSet(ValueSet value) {
        this.value.addAll(value.getValue());
    }

    public ValueSet addItem(Value item) {
        value.add(item);
        return this;
    }

    public ValueSet addItems(Collection<Value> items) {
        this.value.addAll(items);
        return this;
    }

    public TreeSet<Value> getValue() {
        return value;
    }


    public boolean isEquals(Value value) {
        if (!value.isSet()) return false;
        TreeSet<Value> other = value.asSet().getValue();
        if (this.value.size() != other.size()) return false;
        for (Value item : this.value) {
            if (!other.contains(item)) return false;
        }
        return true;
    }

    public int compareTo(Value value) {
        return toString().compareTo(value.toString());
    }

    public String type() {
        return "set";
    }

    public int hashCode() {
        return value.hashCode();
    }

    public ValueString asString() {
        return new ValueString(toString());
    }

    public ValueInt asInt() {
        return new ValueInt(value.size());
    }

    public ValueBoolean asBoolean() {
        return ValueBoolean.from(value.size() > 0);
    }

    public ValueList asList() {
        ValueList result = new ValueList();
        for (Value item : value) {
            result.addItem(item);
        }
        return result;
    }

    public ValueSet asSet() {
        return this;
    }

    public boolean isSet() {
        return true;
    }

    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("<<");
        for (Value item : value) {
            builder.append(item.asString().getValue()).append(", ");
        }
        if (builder.length() > "<<".length()) builder.setLength(builder.length() - 2);
        builder.append(">>");
        return builder.toString();
    }

}
