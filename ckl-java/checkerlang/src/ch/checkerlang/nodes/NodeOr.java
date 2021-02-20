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

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class NodeOr implements Node {
    private List<Node> expressions = new ArrayList<>();

    private SourcePos pos;

    public NodeOr(SourcePos pos) {
        this.pos = pos;
    }

    public NodeOr(Node expression, SourcePos pos) {
        expressions.add(expression);
        this.pos = pos;
    }

    public void addOrClause(Node expression) {
        expressions.add(expression);
    }

    public Node getSimplified() {
        if (expressions.size() == 1) {
            return expressions.get(0);
        }
        return this;
    }

    public Value evaluate(Environment environment) {
        for (Node expression : expressions) {
            if (expression.evaluate(environment).asBoolean().isTrue()) {
                return ValueBoolean.TRUE;
            }
        }
        return ValueBoolean.FALSE;
    }

    public String toString() {
        StringBuilder result = new StringBuilder();
        result.append("(");
        for (Node expression : expressions) {
            result.append(expression).append(" or ");
        }
        if (result.length() > 1) result.setLength(result.length() - " or ".length());
        result.append(")");
        return result.toString();
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        for (Node expression : expressions) {
            expression.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
