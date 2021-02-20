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

import java.util.regex.Pattern;

public class ValuePattern extends Value {
    private String value;
    private Pattern pattern;

    public ValuePattern(String value) {
        this.value = value;
        pattern = Pattern.compile(value);
    }

    public String getValue() {
        return value;
    }

    public Pattern getPattern() {
        return pattern;
    }

    public boolean isEquals(Value value) {
        if (!value.isPattern()) return false;
        return this.value.equals(value.asPattern().getValue());
    }

    public int compareTo(Value value) {
        if (!value.isPattern()) return toString().compareTo(value.toString());
        return this.value.compareTo(value.asPattern().value);
    }

    public int hashCode() {
        return value.hashCode();
    }

    public String type() {
        return "pattern";
    }

    public ValueString asString() {
        return new ValueString(value);
    }

    public ValuePattern asPattern() {
        return this;
    }

    public ValueList asList() {
        return new ValueList().addItem(this);
    }

    public boolean isPattern() {
        return true;
    }

    public String toString() {
        return "//" + value + "//";
    }

}
