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
import ch.checkerlang.values.ValueMap;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class NodeMap implements Node {
    private List<Node> keys = new ArrayList<>();
    private List<Node> values = new ArrayList<>();

    private SourcePos pos;

    public NodeMap(SourcePos pos) {
        this.pos = pos;
    }

    public void addKeyValue(Node key, Node value) {
        keys.add(key);
        values.add(value);
    }

    public Value evaluate(Environment environment) {
        ValueMap result = new ValueMap();
        for (int i = 0; i < keys.size(); i++) {
            result.addItem(keys.get(i).evaluate(environment), values.get(i).evaluate(environment));
        }
        return result;
    }

    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("<<<");
        for (int i = 0; i < keys.size(); i++) {
            builder.append(keys.get(i)).append(" => ").append(values.get(i)).append(", ");
        }
        if (builder.length() > 3) builder.setLength(builder.length() - 2);
        builder.append(">>>");
        return builder.toString();
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        for (Node item : keys) {
            item.collectVars(freeVars, boundVars, additionalBoundVars);
        }
        for (Node item : values) {
            item.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
