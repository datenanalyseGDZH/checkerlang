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
package ch.checkerlang;

import ch.checkerlang.functions.FuncRun;
import ch.checkerlang.nodes.Node;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueInput;
import ch.checkerlang.values.ValueOutput;

import java.io.*;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;

public class Interpreter {
    private Environment baseEnvironment;
    private Environment environment;

    public Interpreter() {
        this(true, true);
    }

    public Interpreter(boolean secure, boolean legacy) {
        baseEnvironment = Environment.getBaseEnvironment(secure, legacy);
        environment = baseEnvironment.newEnv();
        baseEnvironment.put("stdout", new ValueOutput(new StringWriter()));
        baseEnvironment.put("stdin", new ValueInput(new BufferedReader(new StringReader(""))));
        if (!secure) baseEnvironment.put("run", new FuncRun(this));
    }

    public Environment getBaseEnvironment() {
        return baseEnvironment;
    }

    public Environment getEnvironment() {
        return environment;
    }

    public void pushEnvironment() {
        environment = environment.newEnv();
    }

    public void popEnvironment() {
        environment = environment.getParent();
    }

    public void setStandardOutput(Writer stdout) {
        baseEnvironment.put("stdout", new ValueOutput(stdout));
    }

    public void setStandardInput(BufferedReader stdin) {
        baseEnvironment.put("stdin", new ValueInput(stdin));
    }

    public void loadFile(String filename) throws IOException {
        loadFile(filename, StandardCharsets.UTF_8);
    }

    public void loadFile(String filename, Charset encoding) throws IOException {
        try (Reader input = new InputStreamReader(new FileInputStream(filename), encoding)) {
            interpret(input, new File(filename).getName());
        }
    }

    public Value interpret(String script, String filename) throws IOException {
        return interpret(script, filename, null);
    }

    public Value interpret(String script, String filename, Environment environment) throws IOException {
        return interpret(new StringReader(script), filename, environment);
    }

    public Value interpret(Reader input, String filename) throws IOException {
        return interpret(input, filename, null);
    }

    public Value interpret(Reader input, String filename, Environment environment) throws IOException {
        return interpret(Parser.parse(input, filename), environment);
    }

    public Value interpret(Node expression) {
        return interpret(expression, null);
    }

    public Value interpret(Node expression, Environment environment) {
        Environment savedParent = null;
        Environment env;
        if (environment == null) {
            env = this.environment;
        } else {
            Environment environment_ = environment;
            while (environment_ != null && environment_.getParent() != null) {
                environment_ = environment_.getParent();
            }
            if (environment_ != null) {
                savedParent = environment_.getParent();
                environment_.withParent(this.environment);
            }
            env = environment;
        }
        try {
            Value result = expression.evaluate(env);
            if (result.isReturn()) {
                return result.asReturn().value;
            } else if (result.isBreak()) {
                throw new ControlErrorException("Cannot use break without surrounding loop", result.asBreak().pos);
            } else if (result.isContinue()) {
                throw new ControlErrorException("Cannot use continue without surrounding loop", result.asContinue().pos);
            }
            return result;
        } finally {
            if (savedParent != null) {
                Environment environment_ = environment;
                while (environment_ != null && environment_.getParent() != null) {
                    environment_ = environment_.getParent();
                }
                if (environment_ != null) environment_.withParent(savedParent);
            }
        }
    }

}
