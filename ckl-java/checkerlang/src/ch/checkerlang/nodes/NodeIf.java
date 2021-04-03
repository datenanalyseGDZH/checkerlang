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
import ch.checkerlang.values.ValueBoolean;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class NodeIf implements Node {
    private List<Node> conditions = new ArrayList<>();
    private List<Node> expressions = new ArrayList<>();
    private Node elseExpression;

    private SourcePos pos;

    public NodeIf(SourcePos pos) {
        this.pos = pos;
        elseExpression = new NodeLiteral(ValueBoolean.TRUE, pos);
    }

    public void addIf(Node condition, Node expression) {
        conditions.add(condition);
        expressions.add(expression);
    }

    public void setElse(Node expression) {
        elseExpression = expression;
    }

    public Value evaluate(Environment environment) {
        for (int i = 0; i < conditions.size(); i++) {
            Value value = conditions.get(i).evaluate(environment);
            if (!value.isBoolean())
                throw new ControlErrorException("expected boolean condition but got " + value.type(), pos);
            if (value.asBoolean().isTrue()) {
                return expressions.get(i).evaluate(environment);
            }
        }
        return elseExpression.evaluate(environment);
    }

    public String toString() {
        StringBuilder result = new StringBuilder();
        result.append("(");
        for (int i = 0; i < conditions.size(); i++) {
            result.append("if ").append(conditions.get(i));
            result.append(": ").append(expressions.get(i)).append(" ");
        }
        result.append("else: ").append(elseExpression);
        result.append(")");
        return result.toString();
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        for (Node expression : conditions) {
            expression.collectVars(freeVars, boundVars, additionalBoundVars);
        }
        for (Node expression : expressions) {
            expression.collectVars(freeVars, boundVars, additionalBoundVars);
        }
        elseExpression.collectVars(freeVars, boundVars, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
