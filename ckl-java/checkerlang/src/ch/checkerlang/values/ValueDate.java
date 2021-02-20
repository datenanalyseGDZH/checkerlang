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

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;

public class ValueDate extends Value {
    private static final DateFormat fmt = new SimpleDateFormat("yyyyMMdd");

    private Date value;

    public ValueDate(Date value) {
        this.value = value;
    }

    public Date getValue() {
        return value;
    }

    public boolean isEquals(Value value) {
        if (!value.isDate()) return false;
        return this.value.getTime() == value.asDate().getValue().getTime();
    }

    public int compareTo(Value value) {
        if (!value.isDate()) return toString().compareTo(value.toString());
        return this.value.compareTo(value.asDate().value);
    }

    public int hashCode() {
        return value.hashCode();
    }

    public String type() {
        return "date";
    }

    public ValueString asString() {
        return new ValueString(toString());
    }

    public ValueInt asInt() {
        return new ValueInt((int) DateConverter.convertDateToOADate(value));
    }

    public ValueDecimal asDecimal() {
        return new ValueDecimal(DateConverter.convertDateToOADate(value));
    }

    public ValueDate asDate() {
        return this;
    }

    public ValueList asList() {
        return new ValueList().addItem(this);
    }

    public boolean isDate() {
        return true;
    }

    public String toString() {
        return fmt.format(value);
    }

}
