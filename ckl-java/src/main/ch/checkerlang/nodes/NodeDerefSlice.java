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
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueNull;
import ch.checkerlang.values.ValueString;

import java.util.Collection;
import java.util.List;

public class NodeDerefSlice implements Node {
    private Node expression;
    private Node start;
    private Node end;

    private SourcePos pos;

    public NodeDerefSlice(Node expression, Node start, Node end, SourcePos pos) {
        this.expression = expression;
        this.start = start;
        this.end = end;
        this.pos = pos;
    }

    public Value evaluate(Environment environment) {
        Value value = this.expression.evaluate(environment);
        Value start = this.start.evaluate(environment);
        Value end = this.end != null ? this.end.evaluate(environment) : null;
        if (value == ValueNull.NULL) return ValueNull.NULL;
        if (value instanceof ValueString) {
            String s = value.asString().getValue();
            int idxStart = (int) start.asInt().getValue();
            int idxEnd = end == null ? s.length() : (int) end.asInt().getValue();
            if (idxStart < 0) idxStart = idxStart + s.length();
            if (idxEnd < 0) idxEnd = idxEnd + s.length();
            if (idxStart < 0) idxStart = 0;
            if (idxEnd > s.length()) idxEnd = s.length();
            return new ValueString(s.substring(idxStart, idxEnd));
        }
        if (value instanceof ValueList) {
            List<Value> list = value.asList().getValue();
            int idxStart = (int) start.asInt().getValue();
            int idxEnd = end == null ? list.size() : (int) end.asInt().getValue();
            if (idxStart < 0) idxStart = idxStart + list.size();
            if (idxEnd < 0) idxEnd = idxEnd + list.size();
            if (idxStart < 0) idxStart = 0;
            if (idxEnd > list.size()) idxEnd = list.size();
            ValueList result = new ValueList();
            for (int i = idxStart; i < idxEnd; i++) {
                result.addItem(list.get(i));
            }
            return result;
        }
        throw new ControlErrorException("Cannot slice value " + value, this.pos);
    }

    public String toString() {
        return "(" + expression + "[" + start + " " + end + "])";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        expression.collectVars(freeVars, boundVars, additionalBoundVars);
        start.collectVars(freeVars, boundVars, additionalBoundVars);
        end.collectVars(freeVars, boundVars, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
