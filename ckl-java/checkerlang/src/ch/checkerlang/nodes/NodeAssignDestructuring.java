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
import ch.checkerlang.values.ValueNull;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class NodeAssignDestructuring implements Node {
    private List<String> identifiers = new ArrayList<>();
    private Node expression;

    private SourcePos pos;

    public NodeAssignDestructuring(List<String> identifiers, Node expression, SourcePos pos) {
        this.identifiers.addAll(identifiers);
        this.expression = expression;
        this.pos = pos;
    }

    public Value evaluate(Environment environment) {
        Value value = expression.evaluate(environment);
        List<Value> list = value.asList().getValue();
        Value result = ValueNull.NULL;
        for (int i = 0; i < identifiers.size(); i++) {
            if (!environment.isDefined(identifiers.get(i))) throw new ControlErrorException("Variable '" + identifiers.get(i) + "' is not defined", pos);
            if (i < list.size()) {
                environment.set(identifiers.get(i), list.get(i));
                result = list.get(i);
            } else {
                environment.set(identifiers.get(i), ValueNull.NULL);
                result = ValueNull.NULL;
            }
        }
        return result;
    }

    public String toString() {
        StringBuilder result = new StringBuilder();
        result.append("([");
        for (String identifier : identifiers) {
            result.append(identifier).append(", ");
        }
        if (identifiers.size() > 0) result.setLength(result.length() - 2);
        return result.append("] = ").append(expression).append(")").toString();
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        expression.collectVars(freeVars, boundVars, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
