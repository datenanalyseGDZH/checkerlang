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
import ch.checkerlang.Function;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.*;

import java.util.*;

public class NodeDerefInvoke implements Node {
    private Node objectExpr;
    private String member;
    private List<String> names = new ArrayList<>();
    private List<Node> args = new ArrayList<>();

    private SourcePos pos;

    public NodeDerefInvoke(Node objectExpr, String member, SourcePos pos) {
        this.objectExpr = objectExpr;
        this.member = member;
        this.pos = pos;
    }

    public void addArg(String name, Node arg) {
        this.names.add(name);
        this.args.add(arg);
    }

    public Value evaluate(Environment environment) {
        Value obj = this.objectExpr.evaluate(environment);
        if (obj.isObject()) {
            ValueObject object = obj.asObject();
            if (!object.hasItem(this.member)) throw new ControlErrorException("Member " + this.member + " not found", this.pos);
            Value fnval = object.getItem(this.member);
            if (!fnval.isFunc()) throw new ControlErrorException("Member " + this.member + " is not a function", this.pos);
            ValueFunc fn = fnval.asFunc();
            List<String> names;
            List<Node> args;
            if (object.isModule) {
                names = this.names;
                args = this.args;
            } else {
                names = new ArrayList<>();
                args = new ArrayList<>();
                names.add(null);
                names.addAll(this.names);
                args.add(new NodeLiteral(object, this.pos));
                args.addAll(this.args);
            }
            return Function.invoke(fn, names, args, environment, this.pos);
        }
        if (obj instanceof ValueMap) {
            Map<Value, Value> map = obj.asMap().getValue();
            Value fnval = map.get(new ValueString(this.member));
            if (!fnval.isFunc()) throw new ControlErrorException(this.member + " is not a function", this.pos);
            return Function.invoke(fnval.asFunc(), this.names, this.args, environment, this.pos);
        }
        throw new ControlErrorException("Cannot deref-invoke " + obj.type(), this.pos);
    }

    public String toString() {
        StringBuilder result = new StringBuilder("(");
        for (int i = 0; i < this.args.size(); i++) {
            result.append(this.names.get(i)).append("=").append(this.args.get(i)).append(", ");
        }
        if (result.length() > 1) result.setLength(result.length() - 2);
        return "(" + this.objectExpr + "->" + this.member + "(" + result.toString() + ")";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        objectExpr.collectVars(freeVars, boundVars, additionalBoundVars);
        for (Node arg : this.args) {
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
