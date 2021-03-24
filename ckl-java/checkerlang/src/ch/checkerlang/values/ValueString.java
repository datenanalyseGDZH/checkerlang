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

import ch.checkerlang.ControlErrorException;

import java.text.SimpleDateFormat;

public class ValueString extends Value {
    private String value;

    public ValueString(String value) {
        this.value = value;
    }

    public String getValue() {
        return value;
    }

    public boolean isEquals(Value value) {
        if (!value.isString()) return false;
        return this.value.equals(value.asString().getValue());
    }

    public int compareTo(Value value) {
        return toString().compareTo(value.toString());
    }

    public String type() {
        return "string";
    }

    public int hashCode() {
        return value.hashCode();
    }

    public boolean matches(Value pattern) {
        return pattern.asPattern().getPattern().matcher(value).matches();
    }

    public ValueString asString() {
        return this;
    }

    public ValueInt asInt() {
        try {
            return new ValueInt(Long.parseLong(value));
        } catch (Exception e) {
            throw new ControlErrorException("Cannot convert '" + value + "' to int");
        }
    }

    public ValueDecimal asDecimal() {
        try {
            return new ValueDecimal(Double.parseDouble(value));
        } catch (Exception e) {
            throw new ControlErrorException("Cannot convert '" + value + "' to decimal");
        }
    }

    public ValueBoolean asBoolean() {
        if (value.equals("1")) return ValueBoolean.TRUE;
        if (value.equals("0")) return ValueBoolean.FALSE;
        return ValueBoolean.from(Boolean.parseBoolean(value));
    }

    public ValueDate asDate() {
        String fmt = "yyyyMMdd";
        if (value.length() == 10) {
            fmt = "yyyyMMddHH";
        } else if (value.length() == 14) {
            fmt = "yyyyMMddHHmmss";
        }
        try {
            return new ValueDate(new SimpleDateFormat(fmt).parse(value));
        } catch (Exception e) {
            throw new ControlErrorException("Cannot convert '" + value + "' to date");
        }
    }

    public ValuePattern asPattern() {
        return new ValuePattern(value);
    }

    public ValueList asList() {
        return new ValueList().addItem(this);
    }

    public boolean isString() {
        return true;
    }

    public String toString() {
        return "'" + value.replace("\\", "\\\\").replace("'", "\\'") + "'";
    }

}
