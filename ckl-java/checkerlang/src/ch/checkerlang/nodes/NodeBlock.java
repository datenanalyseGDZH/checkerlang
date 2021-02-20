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

import java.util.*;

public class NodeBlock implements Node {
    private List<Node> expressions = new ArrayList<>();
    private List<Node> finallyexprs = new ArrayList<>();

    private SourcePos pos;

    public NodeBlock(SourcePos pos) {
        this.pos = pos;
    }

    public void add(Node expression) {
        expressions.add(expression);
    }

    public void addFinally(Node expression) { finallyexprs.add(expression); }

    public List<Node> getExpressions() { return expressions; }

    public boolean hasFinally() { return !finallyexprs.isEmpty(); }

    public Value evaluate(Environment environment) {
        Value result = ValueBoolean.TRUE;
        try {
            for (Node expression : expressions) {
                result = expression.evaluate(environment);
                if (result.isReturn()) break;
                if (result.isBreak()) break;
                if (result.isContinue()) break;
            }
        } catch (Exception e) {
            for (Node expression : finallyexprs) {
                expression.evaluate(environment);
            }
            throw e;
        }
        for (Node expression : finallyexprs) {
            expression.evaluate(environment);
        }
        return result;
    }

    public String toString() {
        StringBuilder result = new StringBuilder();
        result.append("(block ");
        for (Node expression : expressions) {
            result.append(expression).append(", ");
        }
        if (expressions.size() > 0) result.setLength(result.length() - 2);
        if (finallyexprs.size() > 0) {
            result.append("finally ");
            for (Node expression : finallyexprs) {
                result.append(expression).append(", ");
            }
        }
        if (finallyexprs.size() > 0) result.setLength(result.length() - 2);
        result.append(")");
        return result.toString();
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        Set<String> additionalBoundVarsLocal = new TreeSet<>(additionalBoundVars);
        for (Node expression : expressions) {
            if (expression instanceof NodeDef) {
                NodeDef def = (NodeDef) expression;
                additionalBoundVarsLocal.add(def.getIdentifier());
            }
            if (expression instanceof NodeDefDestructuring) {
                NodeDefDestructuring def = (NodeDefDestructuring) expression;
                additionalBoundVarsLocal.addAll(def.getIdentifiers());
            }
        }
        for (Node expression : finallyexprs) {
            if (expression instanceof NodeDef) {
                NodeDef def = (NodeDef) expression;
                additionalBoundVarsLocal.add(def.getIdentifier());
            }
            if (expression instanceof NodeDefDestructuring) {
                NodeDefDestructuring def = (NodeDefDestructuring) expression;
                additionalBoundVarsLocal.addAll(def.getIdentifiers());
            }
        }
        for (Node expression : expressions) {
            if (expression instanceof NodeDef || expression instanceof NodeDefDestructuring) {
                expression.collectVars(freeVars, boundVars, additionalBoundVarsLocal);
            } else {
                expression.collectVars(freeVars, boundVars, additionalBoundVars);
            }
        }
        for (Node expression : finallyexprs) {
            if (expression instanceof NodeDef || expression instanceof NodeDefDestructuring) {
                expression.collectVars(freeVars, boundVars, additionalBoundVarsLocal);
            } else {
                expression.collectVars(freeVars, boundVars, additionalBoundVars);
            }
        }
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
