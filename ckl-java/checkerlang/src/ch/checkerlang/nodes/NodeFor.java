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
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.*;

import java.io.IOException;
import java.util.*;

public class NodeFor implements Node {
    private List<String> identifiers = new ArrayList<>();
    private Node expression;
    private Node block;

    private SourcePos pos;

    public NodeFor(List<String> identifiers, Node expression, Node block, SourcePos pos) {
        this.identifiers.addAll(identifiers);
        this.expression = expression;
        this.block = block;
        this.pos = pos;
    }

    public Value evaluate(Environment environment) {
        Value list = expression.evaluate(environment);
        if (list.isInput()) {
            ValueInput input = list.asInput();
            Value result = ValueBoolean.TRUE;
            String line = null;
            try {
                line = input.readLine();
                while (line != null) {
                    Value value = new ValueString(line);
                    if (identifiers.size() == 1) {
                        environment.put(identifiers.get(0), value);
                    } else {
                        List<Value> vals = value.asList().getValue();
                        for (int i = 0; i < identifiers.size(); i++) {
                            environment.put(identifiers.get(i), vals.get(i));
                        }
                    }
                    result = block.evaluate(environment);
                    if (result.isBreak()) {
                        result = ValueBoolean.TRUE;
                        break;
                    } else if (result.isContinue()) {
                        result = ValueBoolean.TRUE;
                        // continue
                    } else if (result.isReturn()) {
                        break;
                    }
                    line = input.readLine();
                }
                if (identifiers.size() == 1) {
                    environment.remove(identifiers.get(0));
                } else {
                    for (int i = 0; i < identifiers.size(); i++) {
                        environment.remove(identifiers.get(i));
                    }
                }
            } catch (IOException e) {
                throw new ControlErrorException("Cannot read from input");
            }
            return result;
        }
        if (list.isList()) {
            Value result = ValueBoolean.TRUE;
            for (Value value : list.asList().getValue()) {
                if (identifiers.size() == 1) {
                    environment.put(identifiers.get(0), value);
                } else {
                    List<Value> vals = value.asList().getValue();
                    for (int i = 0; i < identifiers.size(); i++) {
                        environment.put(identifiers.get(i), vals.get(i));
                    }
                }
                result = block.evaluate(environment);
                if (result.isBreak()) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result.isContinue()) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result.isReturn()) {
                    break;
                }
            }
            if (identifiers.size() == 1) {
                environment.remove(identifiers.get(0));
            } else {
                for (int i = 0; i < identifiers.size(); i++) {
                    environment.remove(identifiers.get(i));
                }
            }
            return result;
        }
        if (list.isSet()) {
            Value result = ValueBoolean.TRUE;
            for (Value value : list.asSet().getValue()) {
                if (identifiers.size() == 1) {
                    environment.put(identifiers.get(0), value);
                } else {
                    List<Value> vals = value.asList().getValue();
                    for (int i = 0; i < identifiers.size(); i++) {
                        environment.put(identifiers.get(i), vals.get(i));
                    }
                }
                result = block.evaluate(environment);
                if (result.isBreak()) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result.isContinue()) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result.isReturn()) {
                    break;
                }
            }
            if (identifiers.size() == 1) {
                environment.remove(identifiers.get(0));
            } else {
                for (int i = 0; i < identifiers.size(); i++) {
                    environment.remove(identifiers.get(i));
                }
            }
            return result;
        }
        if (list.isMap()) {
            Value result = ValueBoolean.TRUE;
            for (Value value : list.asMap().getValue().values()) {
                if (identifiers.size() == 1) {
                    environment.put(identifiers.get(0), value);
                } else {
                    List<Value> vals = value.asList().getValue();
                    for (int i = 0; i < identifiers.size(); i++) {
                        environment.put(identifiers.get(i), vals.get(i));
                    }
                }
                result = block.evaluate(environment);
                if (result.isBreak()) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result.isContinue()) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result.isReturn()) {
                    break;
                }
            }
            if (identifiers.size() == 1) {
                environment.remove(identifiers.get(0));
            } else {
                for (int i = 0; i < identifiers.size(); i++) {
                    environment.remove(identifiers.get(i));
                }
            }
            return result;
        }
        if (list.isObject()) {
            Value result = ValueBoolean.TRUE;
            for (Value value : list.asObject().value.values()) {
                if (identifiers.size() == 1) {
                    environment.put(identifiers.get(0), value);
                } else {
                    List<Value> vals = value.asList().getValue();
                    for (int i = 0; i < identifiers.size(); i++) {
                        environment.put(identifiers.get(i), vals.get(i));
                    }
                }
                result = block.evaluate(environment);
                if (result.isBreak()) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result.isContinue()) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result.isReturn()) {
                    break;
                }
            }
            if (identifiers.size() == 1) {
                environment.remove(identifiers.get(0));
            } else {
                for (int i = 0; i < identifiers.size(); i++) {
                    environment.remove(identifiers.get(i));
                }
            }
            return result;
        }
        if (list.isString()) {
            String str = list.asString().getValue();
            Value result = ValueBoolean.TRUE;
            for (int i = 0; i < str.length(); i++) {
                environment.put(identifiers.get(0), new ValueString(str.substring(i, i + 1)));
                result = block.evaluate(environment);
                if (result.isBreak()) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result.isContinue()) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result.isReturn()) {
                    break;
                }
            }
            environment.remove(identifiers.get(0));
            return result;
        }
        throw new ControlErrorException("Cannot iterate over " + list, pos);
    }

    public String toString() {
        return "(for " + (identifiers.size() == 1 ? identifiers.get(0) : identifiers) + " in " + expression + " do " + block + ")";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        expression.collectVars(freeVars, boundVars, additionalBoundVars);
        Set<String> boundVarsLocal = new HashSet<>(boundVars);
        boundVarsLocal.addAll(identifiers);
        block.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
