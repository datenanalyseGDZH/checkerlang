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

import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueObject;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class NodeClass implements Node {
    private String identifier;
    private List<Node> members = new ArrayList<>();

    private String info;
    private SourcePos pos;

    public NodeClass(String identifier, String info, SourcePos pos) {
        this.identifier = identifier;
        this.info = info;
        this.pos = pos;
    }

    public void addMember(Node member) {
        members.add(member);
    }

    public Value evaluate(Environment environment) {
        ValueObject result = new ValueObject();
        for (Node member : members) {
            NodeDef def = (NodeDef) member;
            result.addItem(def.getIdentifier(), def.evaluate(environment));
        }
        environment.put(identifier, result);
        return result;
    }

    public String toString() {
        StringBuilder result = new StringBuilder("(class ");
        result.append(identifier);
        result.append(" ");
        for (Node member : members) {
            result.append(member);
            result.append(" ");
        }
        result.setLength(result.length() - 1);
        result.append(")");
        return result.toString();
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        for (Node expression : members) {
            expression.collectVars(freeVars, boundVars, additionalBoundVars);
        }
        if (!boundVars.contains(identifier)) {
            boundVars.add(identifier);
        }
    }

    public String getIdentifier() {
        return identifier;
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
