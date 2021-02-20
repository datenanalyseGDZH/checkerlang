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
package ch.checkerlang.functions;

import ch.checkerlang.Args;
import ch.checkerlang.ControlErrorException;
import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.nodes.Node;
import ch.checkerlang.values.Value;

import java.util.ArrayList;
import java.util.List;

public class FuncLambda extends FuncBase {
    private Environment lexicalEnv;
    private List<String> argNames = new ArrayList<>();
    private List<Node> defValues = new ArrayList<>();
    private Node body;

    public FuncLambda(Environment lexicalEnv) {
        super("lambda");
        this.lexicalEnv = lexicalEnv;
    }

    public List<String> getArgNames() {
        return argNames;
    }

    public void setName(String name) {
        this.name = name;
    }

    public void addArg(String name) {
        argNames.add(name);
        defValues.add(null);
    }

    public void addArg(String name, Node defaultValue) {
        argNames.add(name);
        defValues.add(defaultValue);
    }

    public void setBody(Node body) {
        this.body = body;
    }

    public Node getBody() { return body; }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Environment env = lexicalEnv.newEnv();
        for (int i = 0; i < argNames.size(); i++) {
            if (args.hasArg(argNames.get(i))) {
                env.put(argNames.get(i), args.get(argNames.get(i)));
            } else if (defValues.get(i) != null) {
                env.put(argNames.get(i), defValues.get(i).evaluate(env));
            } else {
                throw new ControlErrorException("Missing argument " + argNames.get(i), pos);
            }
        }

        Value result = body.evaluate(env);
        if (result.isReturn()) {
            return result.asReturn().value;
        } else if (result.isBreak()) {
            throw new ControlErrorException("Cannot use break without surrounding loop", result.asBreak().pos);
        } else if (result.isContinue()) {
            throw new ControlErrorException("Cannot use continue without surrounding loop", result.asContinue().pos);
        }
        return result;
    }
}
