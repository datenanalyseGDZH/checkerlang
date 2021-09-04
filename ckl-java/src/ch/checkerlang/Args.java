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

import ch.checkerlang.values.*;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

public class Args {
    private List<String> argNames = new ArrayList<>();
    private Map<String, Value> args = new TreeMap<String, Value>();
    private String restArgName = null;
    private SourcePos pos;

    public Args(String name, Value value, SourcePos pos) {
        this.pos = pos;
        argNames.add(name);
        args.put(name, value);
    }


    public Args(String name1, String name2, Value value1, Value value2, SourcePos pos) {
        this.pos = pos;
        argNames.add(name1);
        argNames.add(name2);
        args.put(name1, value1);
        args.put(name2, value2);
    }

    public Args(List<String> argnames, SourcePos pos) {
        this.pos = pos;
        for (int i = 0; i < argnames.size(); i++) {
            if (!argnames.get(i).endsWith("...")) {
                argNames.add(argnames.get(i));
            } else {
                restArgName = argnames.get(i);
            }
        }
    }

    @Override
    public String toString() {
        StringBuilder result = new StringBuilder();
        for(String argname : args.keySet())
        {
            result.append(argname).append("=").append(args.get(argname)).append(", ");
        }
        if (result.length() > 0) result.setLength(result.length() - 2);
        return result.toString();
    }

    public String toStringAbbrev() {
        StringBuilder result = new StringBuilder();
        for(String argname : args.keySet()) {
            String value = args.get(argname).toString();
            if (value.length() > 50) value = value.substring(0, 50) + "... " + value.substring(value.length() - 5);
            result.append(argname).append("=").append(value).append(", ");
        }
        if (result.length() > 0) result.setLength(result.length() - 2);
        return result.toString();
    }

    public void setArgs(List<String> names, List<Value> values) {
        ValueList rest = new ValueList();
        for (int i = 0; i < values.size(); i++) {
            if (names.get(i) != null) {
                if (!argNames.contains(names.get(i)))
                    throw new ControlErrorException("Argument " + names.get(i) + " is unknown", pos);
                args.put(names.get(i), values.get(i));
            }
        }

        boolean inKeywords = false;
        for (int i = 0; i < values.size(); i++) {
            if (names.get(i) == null) {
                if (inKeywords) {
                    throw new ControlErrorException("Positional arguments need to be placed before named arguments", pos);
                }
                String argName = getNextPositionalArgName();
                if (argName == null) {
                    if (restArgName == null) throw new ControlErrorException("Too many arguments", pos);
                    rest.addItem(values.get(i));
                } else if (!args.containsKey(argName)) {
                    args.put(argName, values.get(i));
                } else {
                    rest.addItem(values.get(i));
                }
            } else {
                inKeywords = true;
                if (!argNames.contains(names.get(i))) {
                    throw new ControlErrorException("Argument " + names.get(i) + " is unknown", pos);
                }
                args.put(names.get(i), values.get(i));
            }
        }

        if (restArgName != null) {
            args.put(restArgName, rest);
        }
    }

    private String getNextPositionalArgName() {
        for (String argname : argNames)
        {
            if (!args.containsKey(argname)) return argname;
        }
        return null;
    }

    public boolean hasArg(String name) {
        return args.containsKey(name);
    }

    public Value get(String name) {
        if (!hasArg(name)) throw new ControlErrorException("Missing argument " + name, pos);
        return args.get(name);
    }

    public boolean isNull(String name) {
        if (!hasArg(name)) return false;
        return get(name).isNull();
    }

    public ValueString getString(String name) {
        Value value = get(name);
        if (!value.isString()) throw new ControlErrorException("String required but got " + value.type(), pos);
        return value.asString();
    }

    public ValueBoolean getBoolean(String name) {
        Value value = get(name);
        if (!value.isBoolean()) throw new ControlErrorException("Boolean required but got " + value.type(), pos);
        return value.asBoolean();
    }

    public ValueString getString(String name, String defaultValue) {
        if (!hasArg(name)) return new ValueString(defaultValue);
        Value value = get(name);
        if (!value.isString()) throw new ControlErrorException("String required but got " + value.type(), pos);
        return value.asString();
    }

    public ValueInt getInt(String name) {
        Value value = get(name);
        if (!value.isInt()) throw new ControlErrorException("Int required but got " + value.type(), pos);
        return value.asInt();
    }

    public ValueInt getInt(String name, long defaultValue) {
        if (!hasArg(name)) return new ValueInt(defaultValue);
        Value value = get(name);
        if (!value.isInt()) throw new ControlErrorException("Int required but got " + value.type(), pos);
        return value.asInt();
    }

    public ValueDecimal getDecimal(String name) {
        Value value = get(name);
        if (!value.isDecimal()) throw new ControlErrorException("Decimal required but got " + value.type(), pos);
        return value.asDecimal();
    }

    public ValueDecimal getDecimal(String name, double defaultValue) {
        if (!hasArg(name)) return new ValueDecimal(defaultValue);
        Value value = get(name);
        if (!value.isDecimal()) throw new ControlErrorException("Decimal required but got " + value.type(), pos);
        return value.asDecimal();
    }

    public ValueDecimal getNumerical(String name) {
        Value value = get(name);
        if (!value.isDecimal() && !value.isInt()) throw new ControlErrorException("Numerical required but got " + value.type(), pos);
        return value.asDecimal();
    }

    public ValueDecimal getNumerical(String name, double defaultValue) {
        if (!hasArg(name)) return new ValueDecimal(defaultValue);
        Value value = get(name);
        if (!value.isDecimal() && !value.isInt()) throw new ControlErrorException("Numerical required but got " + value.type(), pos);
        return value.asDecimal();
    }

    public ValueList getList(String name) {
        Value value = get(name);
        if (!value.isList()) throw new ControlErrorException("List required but got " + value.type(), pos);
        return value.asList();
    }

    public ValueMap getMap(String name) {
        Value value = get(name);
        if (!value.isMap()) throw new ControlErrorException("Map required but got " + value.type(), pos);
        return value.asMap();
    }

    public ValueInput getInput(String name) {
        Value value = get(name);
        if (!value.isInput()) throw new ControlErrorException("Input required but got " + value.type(), pos);
        return value.asInput();
    }

    public ValueInput getInput(String name, ValueInput defaultValue) {
        if (!hasArg(name)) return defaultValue;
        return getInput(name);
    }

    public ValueOutput getOutput(String name) {
        Value value = get(name);
        if (!value.isOutput()) throw new ControlErrorException("Output required but got " + value.type(), pos);
        return value.asOutput();
    }

    public ValueOutput getOutput(String name, ValueOutput defaultValue) {
        if (!hasArg(name)) return defaultValue;
        return getOutput(name);
    }

    public ValueFunc getFunc(String name) {
        Value value = get(name);
        if (!value.isFunc()) throw new ControlErrorException("Func required but got " + value.type(), pos);
        return value.asFunc();
    }

    public ValueDate getDate(String name) {
        Value value = get(name);
        if (!value.isDate()) throw new ControlErrorException("Date required but got " + value.type(), pos);
        return value.asDate();
    }

    public ValueBoolean getAsBoolean(String name) {
        Value value = get(name);
        try {
            return value.asBoolean();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValueNode getAsNode(String name) {
        Value value = get(name);
        try {
            return value.asNode();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValueDate getAsDate(String name) {
        Value value = get(name);
        try {
            return value.asDate();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValueString getAsString(String name) {
        Value value = get(name);
        try {
            return value.asString();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValuePattern getAsPattern(String name) {
        Value value = get(name);
        try {
            return value.asPattern();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValuePattern getAsPattern(String name, ValuePattern defaultValue) {
        if (!hasArg(name)) return defaultValue;
        return getAsPattern(name);
    }

    public ValueList getAsList(String name) {
        Value value = get(name);
        try {
            return value.asList();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValueSet getAsSet(String name) {
        Value value = get(name);
        try {
            return value.asSet();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValueObject getAsObject(String name) {
        Value value = get(name);
        try {
            return value.asObject();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValueMap getAsMap(String name) {
        Value value = get(name);
        try {
            return value.asMap();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValueInt getAsInt(String name) {
        Value value = get(name);
        try {
            return value.asInt();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }

    public ValueDecimal getAsDecimal(String name) {
        Value value = get(name);
        try {
            return value.asDecimal();
        } catch (ControlErrorException e) {
            e.pos = pos;
            throw e;
        }
    }


}