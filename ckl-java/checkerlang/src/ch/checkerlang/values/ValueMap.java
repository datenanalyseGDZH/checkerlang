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

import java.util.Map;
import java.util.TreeMap;

public class ValueMap extends Value {
    private TreeMap<Value, Value> value = new TreeMap<>();

    public ValueMap() {
        // empty
    }

    public ValueMap(Map<Value, Value> value) {
        this.value.putAll(value);
    }

    public ValueMap addItem(Value key, Value value) {
        this.value.put(key, value);
        return this;
    }

    public TreeMap<Value, Value> getValue() {
        return value;
    }

    public boolean isEquals(Value value) {
        if (!value.isMap()) return false;
        TreeMap<Value, Value> other = value.asMap().getValue();
        if (this.value.size() != other.size()) return false;
        for (Value key : this.value.keySet()) {
            if (!other.containsKey(key)) {
                return false;
            }
            if (!this.value.get(key).isEquals(other.get(key))) {
                return false;
            }
        }
        return true;
    }

    public int compareTo(Value value) {
        return toString().compareTo(value.toString());
    }

    public int hashCode() {
        return value.hashCode();
    }

    public String type() {
        return "map";
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
        for (Value item : value.values()) {
            result.addItem(item);
        }
        return result;
    }

    public ValueSet asSet() {
        ValueSet result = new ValueSet();
        for (Value key : value.keySet()) {
            result.addItem(key);
        }
        return result;
    }

    public ValueMap asMap() {
        return this;
    }

    public boolean isMap() {
        return true;
    }

    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("<<<");
        for (Value item : value.keySet()) {
            builder.append(item).append(" => ").append(value.get(item)).append(", ");
        }
        if (builder.length() > "<<<".length()) builder.setLength(builder.length() - 2);
        else if (builder.length() == "<<<".length()) builder.setLength(builder.length() - 1);
        builder.append(">>>");
        return builder.toString();
    }

}
