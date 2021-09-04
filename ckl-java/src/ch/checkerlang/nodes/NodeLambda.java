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
import ch.checkerlang.functions.FuncLambda;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;

import java.util.*;

public class NodeLambda implements Node {
    private List<String> args = new ArrayList<>();
    private List<Node> defs = new ArrayList<>();
    private Node body;

    private SourcePos pos;

    public NodeLambda(SourcePos pos) {
        this.pos = pos;
    }

    public void addArg(String arg) {
        args.add(arg);
        defs.add(null);
    }

    public void addArg(String arg, Node defaultValue) {
        args.add(arg);
        defs.add(defaultValue);
    }

    public void setBody(Node body) {
        if (body instanceof NodeBlock) {
            NodeBlock block = (NodeBlock) body;
            List<Node> expressions = block.getExpressions();
            if (expressions.size() > 0) {
                Node lastexpr = expressions.get(expressions.size() - 1);
                if (lastexpr instanceof NodeReturn) {
                    expressions.set(expressions.size() - 1, ((NodeReturn) lastexpr).getExpression());
                }
            }
        } else if (body instanceof NodeReturn) {
            body = ((NodeReturn) body).getExpression();
        }
        this.body = body;
    }

    public Value evaluate(Environment environment) {
        FuncLambda result = new FuncLambda(environment);
        for (int i = 0; i < args.size(); i++) {
            result.addArg(args.get(i), defs.get(i));
        }
        result.setBody(body);
        return result;
    }

    public String toString() {
        StringBuilder result = new StringBuilder();
        result.append("(lambda ");
        for (int i = 0; i < args.size(); i++) {
            result.append(args.get(i));
            if (defs.get(i) != null) {
                result.append("=").append(defs.get(i));
            }
            result.append(", ");
        }
        result.append(body);
        result.append(")");
        return result.toString();
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        for (Node def : defs) {
            if (def != null) def.collectVars(freeVars, boundVars, additionalBoundVars);
        }
        Set<String> boundVarsLocal = new HashSet<String>(boundVars);
        for (String arg : args) {
            boundVarsLocal.add(arg);
        }
        body.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
