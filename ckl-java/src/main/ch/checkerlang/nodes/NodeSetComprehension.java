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

import ch.checkerlang.AsList;
import ch.checkerlang.ControlErrorException;
import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueSet;

import java.util.Collection;
import java.util.HashSet;
import java.util.Set;

public class NodeSetComprehension implements Node {
    private Node valueExpr;
    private String identifier;
    private Node listExpr;
    private String what;
    private Node conditionExpr;

    private SourcePos pos;

    public NodeSetComprehension(Node valueExpr, String identifier, Node listExpr, String what, SourcePos pos) {
        this.valueExpr = valueExpr;
        this.identifier = identifier;
        this.listExpr = listExpr;
        this.what = what;
        this.pos = pos;
    }

    public void setCondition(Node conditionExpr) {
        this.conditionExpr = conditionExpr;
    }

    public Value evaluate(Environment environment) {
        ValueSet result = new ValueSet();
        Environment localEnv = environment.newEnv();
        ValueList list = AsList.from(listExpr.evaluate(environment), what);
        for (Value listValue : list.getValue()) {
            localEnv.put(identifier, listValue);
            Value value = valueExpr.evaluate(localEnv);
            if (conditionExpr != null) {
                Value condition = conditionExpr.evaluate(localEnv);
                if (!condition.isBoolean()) {
                    throw new ControlErrorException("Condition must be boolean but got " + condition.type(), pos);
                }
                if (condition.asBoolean().getValue()) {
                    result.addItem(value);
                }
            } else {
                result.addItem(value);
            }
        }
        return result;
    }

    public String toString() {
        return "<<" + valueExpr + " for " + identifier + " in " + (what != null ? what + " " : "") + listExpr +
                (conditionExpr == null ? "" : (" if " + conditionExpr)) + ">>";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        Set<String> boundVarsLocal = new HashSet<>(boundVars);
        boundVarsLocal.add(identifier);
        valueExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
        listExpr.collectVars(freeVars, boundVars, additionalBoundVars);
        if (conditionExpr != null) conditionExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
