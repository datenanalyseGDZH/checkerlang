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

import ch.checkerlang.Args;
import ch.checkerlang.ControlErrorException;
import ch.checkerlang.SourcePos;

import java.util.*;

public class ValueObject extends Value {
    public Map<String, Value> value = new HashMap<>();
    public boolean isModule = false;

    public ValueObject addItem(String key, Value value) {
        this.value.put(key, value);
        return this;
    }

    public boolean hasItem(String key) {
        return this.value.containsKey(key);
    }

    public Value getItem(String key) {
        return this.value.get(key);
    }

    public void removeItem(String key) {
        this.value.remove(key);
    }

    public Value resolveItem(String key) {
        if (hasItem(key)) return getItem(key);
        ValueObject current = this;
        while (current.hasItem("_proto_")) {
            current = current.getItem("_proto_").asObject();
            if (current.hasItem(key)) return current.getItem(key);
        }
        return null;
    }

    public boolean isEquals(Value value) {
        if (!value.isObject()) return false;
        Map<String, Value> other = value.asObject().value;
        if (this.value.size() != other.size()) return false;
        for (String key : this.value.keySet()) {
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
        return "object";
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
        for (String key : value.keySet()) {
            result.addItem(new ValueString(key));
        }
        return result;
    }

    public ValueMap asMap() {
        ValueMap result = new ValueMap();
        for (Map.Entry<String, Value> entry : value.entrySet()) {
            result.addItem(new ValueString(entry.getKey()), entry.getValue());
        }
        return result;
    }

    public ValueObject asObject() {
        return this;
    }

    public boolean isObject() {
        return true;
    }

    public String toString() {
        ValueFunc fn = (ValueFunc) resolveItem("_str_");
        if (fn != null) {
            Args args_ = new Args(fn.getArgNames(), SourcePos.Unknown);
            List<String> names = new ArrayList<>();
            List<Value> values = new ArrayList<>();
            names.add(null);
            values.add(this);
            args_.setArgs(names, values);

            try {
                return fn.asFunc().execute(args_, null, SourcePos.Unknown).asString().getValue();
            } catch (ControlErrorException e) {
                e.addStacktraceElement("_str_", SourcePos.Unknown);
                throw e;
            }
        } else {
            StringBuilder builder = new StringBuilder();
            builder.append("<*");
            for (String item : value.keySet()) {
                if (item.startsWith("_")) continue;
                builder.append(item).append("=").append(value.get(item)).append(", ");
            }
            if (builder.length() > 2) builder.setLength(builder.length() - 2);
            builder.append("*>");
            return builder.toString();
        }
    }

}
