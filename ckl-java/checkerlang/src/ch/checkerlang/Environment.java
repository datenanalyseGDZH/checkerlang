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

import ch.checkerlang.functions.*;
import ch.checkerlang.nodes.Node;
import ch.checkerlang.values.*;

import java.io.IOException;
import java.io.InputStreamReader;
import java.math.BigDecimal;
import java.nio.charset.StandardCharsets;
import java.util.*;
import java.util.regex.Pattern;

public class Environment {
    private Map<String, Object> map = new HashMap<>();
    private Environment parent;

    public Map<String, Environment> modules = null;
    public List<String> modulestack = null;

    public Environment() {
        parent = null;
        modules = new HashMap<>();
        modulestack = new ArrayList<>();
    }

    public Environment(Environment parent) {
        this.parent = parent;
    }

    public static Environment getNullEnvironment() {
        return new Environment();
    }

    public static Environment getBaseEnvironment() {
        return getBaseEnvironment(true, true);
    }

    public static Environment getBaseEnvironment(boolean secure, boolean legacy) {
        Environment result = getNullEnvironment();
        result.put("checkerlang_secure_mode", ValueBoolean.from(secure));
        result.put("bind_native", new FuncBindNative());
        result.put("NULL", ValueNull.NULL);
        result.put("MAXINT", new ValueInt(Long.MAX_VALUE).withInfo("MAXINT\n\nThe maximal int value"));
        result.put("MININT", new ValueInt(Long.MIN_VALUE).withInfo("MININT\n\nThe minimal int value"));
        try {
            if (legacy) {
                Node basenode = Parser.parse(new InputStreamReader(Value.class.getResourceAsStream("/module-legacy.ckl"), StandardCharsets.UTF_8), "mod:legacy.ckl");
                basenode.evaluate(result);
            } else {
                Node basenode = Parser.parse(new InputStreamReader(Value.class.getResourceAsStream("/module-base.ckl"), StandardCharsets.UTF_8), "mod:base.ckl");
                basenode.evaluate(result);
            }
        } catch (IOException e) {
            // ignore
        }
        return result;
    }

    public Environment withParent(Environment parent) {
        this.parent = parent;
        return this;
    }

    public Environment getParent() {
        return parent;
    }

    public void put(String name, Object value) {
        map.put(name, value);
    }

    public void set(String name, Object value) {
        if (map.containsKey(name)) map.put(name, value);
        else if (parent != null) parent.set(name, value);
        else throw new ControlErrorException(name + " is not defined");
    }

    public void remove(String name) {
        map.remove(name);
    }

    public Environment newEnv() {
        return new Environment(this);
    }

    public boolean isDefined(String symbol) {
        if (map.containsKey(symbol)) return true;
        if (parent != null) return parent.isDefined(symbol);
        return false;
    }

    public Value get(String symbol, SourcePos pos) {
        if (map.containsKey(symbol)) {
            Object value = map.get(symbol);
            if (value == null) return ValueNull.NULL;
            if (value instanceof Value) {
                return (Value) value;
            } else if (value instanceof Byte) {
                return new ValueInt((byte) value);
            } else if (value instanceof Short) {
                return new ValueInt((short) value);
            } else if (value instanceof Integer) {
                return new ValueInt((int) value);
            } else if (value instanceof Long) {
                return new ValueInt((long) value);
            } else if (value instanceof BigDecimal) {
                return new ValueDecimal(((BigDecimal) value).doubleValue());
            } else if (value instanceof Float) {
                return new ValueDecimal((float) value);
            } else if (value instanceof Double) {
                return new ValueDecimal((double) value);
            } else if (value instanceof Boolean) {
                return ValueBoolean.from((boolean) value);
            } else if (value instanceof Pattern) {
                return new ValuePattern(((Pattern) value).pattern());
            } else if (value instanceof Date) {
                return new ValueDate((Date) value);
            } else {
                return new ValueString(value.toString());
            }
        }
        if (parent != null) return parent.get(symbol, pos);
        throw new ControlErrorException(new ValueString("Symbol '" + symbol + "' not defined"), pos, new Stacktrace());
    }

    public Environment getBase() {
        Environment current = this;
        while (current.parent != null) current = current.parent;
        return current;
    }

    public List<String> getSymbols() {
        List<String> result = new ArrayList<>();
        result.addAll(map.keySet());
        if (parent != null) result.addAll(parent.getSymbols());
        Collections.sort(result);
        return result;
    }

    public List<String> getLocalSymbols() {
        return new ArrayList<>(map.keySet());
    }

    public Map<String, Environment> getModules() {
        Environment current = this;
        while (current.parent != null) current = current.parent;
        return current.modules;
    }

    public void pushModuleStack(String moduleidentifier, SourcePos pos) {
        Environment current = this;
        while (current.parent != null) current = current.parent;
        if (current.modulestack.contains(moduleidentifier)) throw new ControlErrorException("Found circular module dependency (" + moduleidentifier + ")", pos);
        current.modulestack.add(moduleidentifier);
    }

    public void popModuleStack() {
        Environment current = this;
        while (current.parent != null) current = current.parent;
        current.modulestack.remove(current.modulestack.size() - 1);
    }

}
