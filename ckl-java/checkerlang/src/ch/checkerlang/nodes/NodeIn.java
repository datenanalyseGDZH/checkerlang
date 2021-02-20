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

import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueBoolean;

import java.util.Collection;
import java.util.List;

public class NodeIn implements Node {
    private Node expression;
    private Node list;

    private SourcePos pos;

    public NodeIn(Node expression, Node list, SourcePos pos) {
        this.expression = expression;
        this.list = list;
        this.pos = pos;
    }

    public Value evaluate(Environment environment) {
        Value value = expression.evaluate(environment);
        Value container = list.evaluate(environment);
        if (container.isList()) {
            List<Value> items = container.asList().getValue();
            for (Value item : items) {
                if (value.isEquals(item)) return ValueBoolean.TRUE;
            }
        } else if (container.isSet()) {
            return ValueBoolean.from(container.asSet().getValue().contains(value));
        } else if (container.isMap()) {
            return ValueBoolean.from(container.asMap().getValue().containsKey(value));
        } else if (container.isString()) {
            return ValueBoolean.from(container.asString().getValue().contains(value.asString().getValue()));
        }
        return ValueBoolean.FALSE;
    }

    public String toString() {
        return "(" + expression + " in " + list + ")";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        expression.collectVars(freeVars, boundVars, additionalBoundVars);
        list.collectVars(freeVars, boundVars, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
