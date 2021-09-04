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

import java.util.Collection;
import java.util.HashSet;
import java.util.Set;

public class NodeListComprehensionProduct implements Node {
    private Node valueExpr;
    private String identifier1;
    private Node listExpr1;
    private String what1;
    private String identifier2;
    private Node listExpr2;
    private String what2;
    private Node conditionExpr;

    private SourcePos pos;

    public NodeListComprehensionProduct(Node valueExpr, String identifier1, Node listExpr1, String what1, String identifier2, Node listExpr2, String what2, SourcePos pos) {
        this.valueExpr = valueExpr;
        this.identifier1 = identifier1;
        this.listExpr1 = listExpr1;
        this.what1 = what1;
        this.identifier2 = identifier2;
        this.listExpr2 = listExpr2;
        this.what2 = what2;
        this.pos = pos;
    }

    public void setCondition(Node conditionExpr) {
        this.conditionExpr = conditionExpr;
    }

    public Value evaluate(Environment environment) {
        ValueList result = new ValueList();
        Environment localEnv = environment.newEnv();
        ValueList list1 = AsList.from(listExpr1.evaluate(environment), what1);
        ValueList list2 = AsList.from(listExpr2.evaluate(environment), what2);
        for (Value value1 : list1.getValue()) {
            localEnv.put(identifier1, value1);
            for (Value value2 : list2.getValue()) {
                localEnv.put(identifier2, value2);
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
        }
        return result;
    }

    public String toString() {
        return "[" + valueExpr +
                " for " + identifier1 + " in " + (what1 != null ? what1 + " " : "") + listExpr1 +
                " for " + identifier2 + " in " + (what2 != null ? what2 + " " : "") + listExpr2 +
                (conditionExpr == null ? "" : (" if " + conditionExpr)) + "]";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        Set<String> boundVarsLocal = new HashSet<>(boundVars);
        boundVarsLocal.add(identifier1);
        boundVarsLocal.add(identifier2);
        valueExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
        listExpr1.collectVars(freeVars, boundVars, additionalBoundVars);
        listExpr2.collectVars(freeVars, boundVars, additionalBoundVars);
        if (conditionExpr != null) conditionExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
