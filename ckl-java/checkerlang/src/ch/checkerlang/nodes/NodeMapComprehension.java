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
import ch.checkerlang.values.*;

import java.util.Collection;
import java.util.HashSet;
import java.util.Set;

public class NodeMapComprehension implements Node {
    private Node keyExpr;
    private Node valueExpr;
    private String identifier;
    private Node listExpr;
    private Node conditionExpr;

    private SourcePos pos;

    public NodeMapComprehension(Node keyExpr, Node valueExpr, String identifier, Node listExpr, SourcePos pos) {
        this.keyExpr = keyExpr;
        this.valueExpr = valueExpr;
        this.identifier = identifier;
        this.listExpr = listExpr;
        this.pos = pos;
    }

    public void setCondition(Node conditionExpr) {
        this.conditionExpr = conditionExpr;
    }

    public Value evaluate(Environment environment) {
        ValueMap result = new ValueMap();
        Environment localEnv = environment.newEnv();
        Value list = listExpr.evaluate(environment);
        if (list.isString()) {
            String s = list.asString().getValue();
            ValueList slist = new ValueList();
            for (int i = 0; i < s.length(); i++) {
                slist.addItem(new ValueString(s.substring(i, i + 1)));
            }
            list = slist;
        }
        for (Value listValue : list.asList().getValue()) {
            localEnv.put(identifier, listValue);
            Value key = keyExpr.evaluate(localEnv);
            Value value = valueExpr.evaluate(localEnv);
            if (conditionExpr != null) {
                Value condition = conditionExpr.evaluate(localEnv);
                if (!condition.isBoolean()) {
                    throw new ControlErrorException("Condition must be boolean but got " + condition.type(), pos);
                }
                if (condition.asBoolean().getValue()) {
                    result.addItem(key, value);
                }
            } else {
                result.addItem(key, value);
            }
        }
        return result;
    }

    public String toString() {
        return "<<<" + keyExpr + " => " + valueExpr + " for " + identifier + " in " + listExpr + (conditionExpr == null ? "" : (" if " + conditionExpr)) + ">>>";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        Set<String> boundVarsLocal = new HashSet<>(boundVars);
        boundVarsLocal.add(identifier);
        keyExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
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
