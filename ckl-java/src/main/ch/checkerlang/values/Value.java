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

public abstract class Value implements Comparable<Value> {
    public String info = "";

    public abstract boolean isEquals(Value value);

    public abstract int compareTo(Value value);

    public abstract String type();

    public abstract int hashCode();

    public ValueString asString() {
        throw new ControlErrorException("Cannot convert to String");
    }

    public ValueInt asInt() {
        throw new ControlErrorException("Cannot convert to int");
    }

    public ValueDecimal asDecimal() {
        throw new ControlErrorException("Cannot convert to decimal");
    }

    public ValueBoolean asBoolean() {
        throw new ControlErrorException("Cannot convert to booleanean");
    }

    public ValuePattern asPattern() {
        throw new ControlErrorException("Cannot convert to pattern");
    }

    public ValueDate asDate() {
        throw new ControlErrorException("Cannot convert to date");
    }

    public ValueList asList() {
        throw new ControlErrorException("Cannot convert to list");
    }

    public ValueSet asSet() {
        throw new ControlErrorException("Cannot convert to set");
    }

    public ValueMap asMap() {
        throw new ControlErrorException("Cannot convert to map");
    }

    public ValueFunc asFunc() {
        throw new ControlErrorException("Cannot convert to func");
    }

    public ValueInput asInput() {
        throw new ControlErrorException("Cannot convert to input");
    }

    public ValueOutput asOutput() {
        throw new ControlErrorException("Cannot convert to output");
    }

    public ValueNull asNull() {
        throw new ControlErrorException("Cannot convert to NULL");
    }

    public ValueNode asNode() {
        throw new ControlErrorException("Cannot convert to Node");
    }

    public ValueObject asObject() {
        throw new ControlErrorException("Cannot convert to Object");
    }

    public ValueControlBreak asBreak() {
        throw new ControlErrorException("Cannot convert to break");
    }

    public ValueControlContinue asContinue() {
        throw new ControlErrorException("Cannot convert to continue");
    }

    public ValueControlReturn asReturn() {
        throw new ControlErrorException("Cannot convert to return");
    }

    public boolean isString() {
        return false;
    }

    public boolean isInt() {
        return false;
    }

    public boolean isDecimal() {
        return false;
    }

    public boolean isBoolean() {
        return false;
    }

    public boolean isDate() {
        return false;
    }

    public boolean isPattern() {
        return false;
    }

    public boolean isList() {
        return false;
    }

    public boolean isSet() {
        return false;
    }

    public boolean isMap() {
        return false;
    }

    public boolean isFunc() {
        return false;
    }

    public boolean isInput() {
        return false;
    }

    public boolean isOutput() {
        return false;
    }

    public boolean isNull() {
        return false;
    }

    public boolean isNode() {
        return false;
    }

    public boolean isObject() {
        return false;
    }

    public boolean isBreak() {
        return false;
    }

    public boolean isContinue() {
        return false;
    }

    public boolean isReturn() {
        return false;
    }

    public boolean isCollection() { return isList() || isSet(); }

    public boolean isAtomic() {
        return isString() || isInt() || isDecimal() || isBoolean() || isDate() || isPattern() || isNull();
    }

    public boolean isNumerical() {
        return isInt() || isDecimal();
    }

    public Value withInfo(String info) {
        this.info = info;
        return this;
    }

}
