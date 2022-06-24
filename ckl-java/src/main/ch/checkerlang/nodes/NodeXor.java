/*  Copyright (c) 2022 Damian Brunold, Gesundheitsdirektion Kanton Zürich

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

import java.util.Collection;

public class NodeXor implements Node {
    private Node firstExpr = null;
    private Node secondExpr = null;

    private SourcePos pos;

    public NodeXor(SourcePos pos, Node firstExpr, Node secondExpr) {
        this.pos = pos;
        this.firstExpr = firstExpr;
        this.secondExpr = secondExpr;
    }

    public Value evaluate(Environment environment) {
        Value firstValue = firstExpr.evaluate(environment);
        Value secondValue = secondExpr.evaluate(environment);
        if (!firstValue.isBoolean()) throw new ControlErrorException("Expected boolean but got " + firstValue.type(), pos);
        if (!secondValue.isBoolean()) throw new ControlErrorException("Expected boolean but got " + secondValue.type(), pos);
        return ValueBoolean.from(firstValue.asBoolean().getValue() ^ secondValue.asBoolean().getValue());
    }

    public String toString() {
        return "(" + firstExpr + " xor " + secondExpr + ")";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        firstExpr.collectVars(freeVars, boundVars, additionalBoundVars);
        secondExpr.collectVars(freeVars, boundVars, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
