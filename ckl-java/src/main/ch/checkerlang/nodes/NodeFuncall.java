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

import ch.checkerlang.*;
import ch.checkerlang.values.Value;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class NodeFuncall implements Node {
    private Node func;
    private List<String> names = new ArrayList<>();
    private List<Node> args = new ArrayList<>();

    private SourcePos pos;

    public NodeFuncall(Node func, SourcePos pos) {
        this.func = func;
        this.pos = pos;
    }

    public Node getFunc() { return func; }

    public int argCount() { return args.size(); }

    public Node getArg(int i) { return args.get(i); }

    public void addArg(String name, Node arg) {
        names.add(name);
        args.add(arg);
    }

    public Value evaluate(Environment environment) {
        Value fn = func.evaluate(environment);
        if (!fn.isFunc()) throw new ControlErrorException("Expected function but got " + fn.type(), pos);
        return Function.invoke(fn.asFunc(), this.names, this.args, environment, pos);
    }

    public String toString() {
        StringBuilder builder = new StringBuilder();
        for (Node expression : args) {
            builder.append(expression).append(", ");
        }
        if (builder.length() > 1) builder.setLength(builder.length() - 2);
        return "(" + func + " " + builder + ")";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        func.collectVars(freeVars, boundVars, additionalBoundVars);
        for (Node arg : args) {
            arg.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
