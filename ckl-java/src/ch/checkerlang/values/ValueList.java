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

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class ValueList extends Value {
    private List<Value> value = new ArrayList<>();

    public ValueList() {
        // empty
    }

    public ValueList(List<Value> value) {
        this.value.addAll(value);
    }

    public ValueList(ValueList value) {
        this.value.addAll(value.getValue());
    }

    public ValueList addItems(Collection<Value> items) {
        this.value.addAll(items);
        return this;
    }

    public ValueList addItem(Value item) {
        value.add(item);
        return this;
    }

    public List<Value> getValue() {
        return value;
    }


    public boolean isEquals(Value value) {
        if (!value.isList()) return false;
        if (this.value.size() != value.asList().getValue().size()) return false;
        for (int i = 0; i < this.value.size(); i++) {
            if (!this.value.get(i).isEquals(value.asList().getValue().get(i))) {
                return false;
            }
        }
        return true;
    }

    public int compareTo(Value value) {
        if (!value.isList()) return toString().compareTo(value.toString());
        List<Value> lst = value.asList().getValue();
        for (int i = 0; i < Math.min(getValue().size(), lst.size()); i++) {
            int cmp = getValue().get(i).compareTo(lst.get(i));
            if (cmp != 0) return cmp;
        }

        if (getValue().size() < lst.size()) return -1;
        if (getValue().size() > lst.size()) return 1;
        return 0;
    }

    public int hashCode() {
        return value.hashCode();
    }

    public String type() {
        return "list";
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
        return this;
    }

    public ValueSet asSet() {
        ValueSet result = new ValueSet();
        for (Value item : value) {
            result.addItem(item);
        }
        return result;
    }

    public ValueMap asMap() {
        ValueMap result = new ValueMap();
        for (Value item : value) {
            result.addItem(item.asList().getValue().get(0), item.asList().getValue().get(1));
        }
        return result;
    }

    public ValueObject asObject() {
        ValueObject result = new ValueObject();
        for (Value item : value) {
            result.addItem(item.asList().getValue().get(0).asString().getValue(), item.asList().getValue().get(1));
        }
        return result;
    }

    public boolean isList() {
        return true;
    }

    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("[");
        for (Value item : value) {
            builder.append(item).append(", ");
        }
        if (builder.length() > 1) builder.setLength(builder.length() - 2);
        builder.append("]");
        return builder.toString();
    }

}
