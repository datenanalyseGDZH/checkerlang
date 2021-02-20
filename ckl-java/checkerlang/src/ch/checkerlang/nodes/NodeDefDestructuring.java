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
import ch.checkerlang.functions.FuncLambda;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueNull;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class NodeDefDestructuring implements Node {
    private List<String> identifiers = new ArrayList<>();
    private Node expression;

    private String info;
    private SourcePos pos;

    public NodeDefDestructuring(List<String> identifiers, Node expression, String info, SourcePos pos) {
        this.identifiers.addAll(identifiers);
        this.expression = expression;
        this.info = info;
        this.pos = pos;
    }

    public Value evaluate(Environment environment) {
        Value value = expression.evaluate(environment);
        value.info = info;
        List<Value> list = value.asList().getValue();
        Value result = ValueNull.NULL;
        for (int i = 0; i < identifiers.size(); i++) {
            if (i < list.size()) {
                environment.put(identifiers.get(i), list.get(i));
                if (list.get(i).isFunc() && list.get(i) instanceof FuncLambda) ((FuncLambda) list.get(i)).setName(identifiers.get(i));
                result = list.get(i);
            } else {
                environment.put(identifiers.get(i), ValueNull.NULL);
                result = ValueNull.NULL;
            }
        }
        return result;
    }

    public String toString() {
        StringBuilder result = new StringBuilder();
        result.append("(def [");
        for (String identifier : identifiers) {
            result.append(identifier).append(", ");
        }
        if (identifiers.size() > 0) result.setLength(result.length() - 2);
        return result.append("] = ").append(expression).append(")").toString();
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        expression.collectVars(freeVars, boundVars, additionalBoundVars);
        boundVars.addAll(identifiers);
    }

    public List<String> getIdentifiers() {
        return identifiers;
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
