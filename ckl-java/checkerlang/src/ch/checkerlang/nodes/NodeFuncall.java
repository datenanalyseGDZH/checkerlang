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

import ch.checkerlang.Args;
import ch.checkerlang.ControlErrorException;
import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueFunc;
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueMap;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Map;

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
        if (!fn.isFunc()) throw new ControlErrorException(fn + " is not a function", pos);

        List<Value> values = new ArrayList<>();
        List<String> names = new ArrayList<>();
        for (int i = 0; i < args.size(); i++) {
            Node arg = args.get(i);
            if (arg instanceof NodeSpread) {
                Value argvalue = arg.evaluate(environment);
                if (argvalue.isMap()) {
                    ValueMap map = argvalue.asMap();
                    for (Map.Entry<Value, Value> entry : map.getValue().entrySet()) {
                        values.add(entry.getValue());
                        if (entry.getKey().isString()) {
                            names.add(entry.getKey().asString().getValue());
                        } else {
                            names.add(null);
                        }
                    }
                } else {
                    ValueList list = argvalue.asList();
                    for (Value value : list.getValue()) {
                        values.add(value);
                        names.add(null);
                    }
                }
            } else {
                values.add(arg.evaluate(environment));
                names.add(this.names.get(i));
            }
        }
        this.names = names;

        Args args_ = new Args(fn.asFunc().getArgNames(), pos);
        args_.setArgs(this.names, values);

        try {
            return fn.asFunc().execute(args_, environment, pos);
        } catch (ControlErrorException e) {
            e.addStacktraceElement(getFuncallString(fn.asFunc(), args_), pos);
            throw e;
        }
    }

    private String getFuncallString(ValueFunc fn, Args args) {
        return fn.getName() + "(" + args.toStringAbbrev() + ")";
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
