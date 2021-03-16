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
package ch.checkerlang.nodes;

import ch.checkerlang.ControlErrorException;
import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueString;

import java.util.Collection;
import java.util.List;
import java.util.Map;

public class NodeDerefAssign implements Node {
    private Node expression;
    private Node index;
    private Node value;

    private SourcePos pos;

    public NodeDerefAssign(Node expression, Node index, Node value, SourcePos pos) {
        this.expression = expression;
        this.index = index;
        this.value = value;
        this.pos = pos;
    }

    public Value evaluate(Environment environment) {
        Value idx = index.evaluate(environment);
        Value container = expression.evaluate(environment);
        Value value = this.value.evaluate(environment);
        if (container.isString()) {
            String s = container.asString().getValue();
            int i = (int) idx.asInt().getValue();
            if (i < 0) i = i + s.length();
            if (i < 0 || i >= s.length())
                throw new ControlErrorException("Index out of bounds " + s + "[" + i + "]", pos);
            return new ValueString(s.substring(0, i) + value.asString().getValue() + s.substring(i + 1));
        }
        if (container.isList()) {
            List<Value> list = container.asList().getValue();
            int i = (int) idx.asInt().getValue();
            if (i < 0) i = i + list.size();
            if (i < 0 || i >= list.size())
                throw new ControlErrorException("Index out of bounds" + this.value + "[" + i + "]", pos);
            list.set(i, value);
            return container;
        }
        if (container.isMap()) {
            Map<Value, Value> map = container.asMap().getValue();
            map.put(idx, value);
            return container;
        }
        if (container.isObject()) {
            Map<String, Value> map = container.asObject().value;
            String member = idx.asString().getValue();
            map.put(member, value);
            return container;
        }
        throw new ControlErrorException("Cannot deref-assign " + this.value, pos);
    }

    public String toString() {
        return "(" + expression + "[" + index + "] = " + value + ")";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        expression.collectVars(freeVars, boundVars, additionalBoundVars);
        index.collectVars(freeVars, boundVars, additionalBoundVars);
        value.collectVars(freeVars, boundVars, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
