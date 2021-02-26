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

    public Environment() {
        parent = null;
    }

    public Environment(Environment parent) {
        this.parent = parent;
    }

    public static Environment getNullEnvironment() {
        return new Environment();
    }

    public static Environment getBaseEnvironment() {
        return getBaseEnvironment(true);
    }

    public static Environment getBaseEnvironment(boolean secure) {
        Environment result = getRootEnvironment(secure).newEnv();
        try {
            Node basenode = Parser.parse(new InputStreamReader(Value.class.getResourceAsStream("/base-library.ckl"), StandardCharsets.UTF_8), "{res}base-library.ckl");
            basenode.evaluate(result);
        } catch (IOException e) {
            // ignore
        }
        return result;
    }

    public static Environment getRootEnvironment() {
        return getRootEnvironment(true);
    }

    public static Environment getRootEnvironment(boolean secure) {
        Environment result = getNullEnvironment();
        add(result, new FuncAcos());
        add(result, new FuncAdd());
        add(result, new FuncAppend());
        add(result, new FuncAsin());
        add(result, new FuncAtan());
        add(result, new FuncBody());
        add(result, new FuncBoolean());
        add(result, new FuncCeiling());
        add(result, new FuncCompare());
        add(result, new FuncContains(), "str_contains");
        add(result, new FuncCos());
        add(result, new FuncDate());
        add(result, new FuncDecimal());
        add(result, new FuncDeleteAt());
        add(result, new FuncDiv());
        add(result, new FuncEndsWith(), "str_ends_with");
        add(result, new FuncEquals());
        add(result, new FuncEscapePattern());
        add(result, new FuncEval());
        add(result, new FuncExp());
        add(result, new FuncFind(), "str_find");
        add(result, new FuncFloor());
        add(result, new FuncFormatDate());
        add(result, new FuncGetOutputString());
        add(result, new FuncGreater());
        add(result, new FuncGreaterEquals());
        add(result, new FuncIfEmpty());
        add(result, new FuncIfNull());
        add(result, new FuncIfNullOrEmpty());
        add(result, new FuncInfo());
        add(result, new FuncInsertAt());
        add(result, new FuncInt());
        add(result, new FuncIsEmpty());
        add(result, new FuncIsNotEmpty());
        add(result, new FuncIsNotNull());
        add(result, new FuncIsNull());
        add(result, new FuncLength());
        add(result, new FuncLess());
        add(result, new FuncLessEquals());
        add(result, new FuncList());
        add(result, new FuncLog());
        add(result, new FuncLower());
        add(result, new FuncLs());
        add(result, new FuncMap());
        add(result, new FuncMatches(), "str_matches");
        add(result, new FuncMod());
        add(result, new FuncMul());
        add(result, new FuncNotEquals());
        add(result, new FuncParse());
        add(result, new FuncParseDate());
        add(result, new FuncParseJson());
        add(result, new FuncPattern());
        add(result, new FuncPow());
        add(result, new FuncPrint());
        add(result, new FuncPrintln());
        add(result, new FuncProcessLines());
        add(result, new FuncPut());
        add(result, new FuncRandom());
        add(result, new FuncRange());
        add(result, new FuncRead());
        add(result, new FuncReadall());
        add(result, new FuncReadln());
        add(result, new FuncRemove());
        add(result, new FuncRound());
        add(result, new FuncS());
        add(result, new FuncSet());
        add(result, new FuncSetSeed());
        add(result, new FuncSin());
        add(result, new FuncSorted());
        add(result, new FuncSplit());
        add(result, new FuncSplit2());
        add(result, new FuncSqrt());
        add(result, new FuncStartsWith(), "str_starts_with");
        add(result, new FuncStrInput());
        add(result, new FuncStrOutput());
        add(result, new FuncString());
        add(result, new FuncSub());
        add(result, new FuncSublist());
        add(result, new FuncSubstr());
        add(result, new FuncSum());
        add(result, new FuncTan());
        add(result, new FuncTimestamp());
        add(result, new FuncTrim(), "str_trim");
        add(result, new FuncType());
        add(result, new FuncUpper());
        add(result, new FuncZip());
        add(result, new FuncZipMap());
        if (!secure) {
            add(result, new FuncFileInput());
            add(result, new FuncFileOutput());
            add(result, new FuncClose());
        }
        result.put("NULL", ValueNull.NULL);
        result.put("PI", new ValueDecimal(Math.PI).withInfo("The mathematical constant pi."));
        result.put("E", new ValueDecimal(Math.E).withInfo("The mathematical constant e."));
        result.put("MAXINT", new ValueInt(Long.MAX_VALUE).withInfo("The maximal int value"));
        result.put("MININT", new ValueInt(Long.MIN_VALUE).withInfo("The minimal int value"));
        return result;
    }

    private static void add(Environment env, ValueFunc func) {
        env.put(func.getName(), func);
    }

    private static void add(Environment env, ValueFunc func, String alias) {
        env.put(func.getName(), func);
        env.put(alias, func);
    }

    public Environment withParent(Environment parent) {
        this.parent = parent;
        return this;
    }

    public Environment getParent() {
        return parent;
    }

    public List<String> getSymbols() {
        List<String> result = new ArrayList<>();
        result.addAll(map.keySet());
        if (parent != null) result.addAll(parent.getSymbols());
        Collections.sort(result);
        return result;
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
}
