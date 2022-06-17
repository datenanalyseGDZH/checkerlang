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

import { RuntimeError } from "./errors.mjs";
import { Parser } from "./parser.mjs";
import { Args } from "./args.mjs";
import { system } from "./system.mjs";
import { modulebase } from "./module_base.mjs";
import { modulelegacy } from "./module_legacy.mjs";

import { 
    convertDateToOADate, 
    convertOADateToDate,
    StringInput,
    FileInput,
    FileOutput,
    StringOutput,
    Value,
    ValueBoolean,
    ValueControlBreak,
    ValueControlContinue,
    ValueControlReturn,
    ValueDate,
    ValueDecimal,
    ValueFunc,
    ValueInput,
    ValueInt,
    ValueList,
    ValueMap,
    ValueNode,
    ValueNull,
    ValueObject,
    ValueOutput,
    ValuePattern,
    ValueSet,
    ValueString 
} from "./values.mjs";

export class Environment {
    constructor(parent = null) {
        this.map = new Map();
        this.parent = parent;
        if (this.parent == null) {
            this.modules = new Map();
            this.modulestack = [];
        }
    }

    static getNullEnvironment() {
        return new Environment();
    }

    static getBaseEnvironment(secure = true, legacy = true) {
        const result = Environment.getNullEnvironment();
        result.put("checkerlang_secure_mode", ValueBoolean.from(secure));
        bind_native(result, "bind_native");
        result.put("NULL", ValueNull.NULL);
        result.put("MAXINT", new ValueInt(Number.MAX_SAFE_INTEGER).withInfo("MAXINT\n\nThe maximal int value"));
        result.put("MININT", new ValueInt(Number.MIN_SAFE_INTEGER).withInfo("MININT\n\nThe minimal int value"));
        if (legacy) Parser.parseScript(modulelegacy, ":legacy").evaluate(result);
        else Parser.parseScript(modulebase, ":base").evaluate(result);
        return result;
    }

    static add(env, func, alias) {
        if (alias !== undefined && alias !== null) env.put(alias, func);
        env.put(func.name, func);
    }

    withParent(parent) {
        this.parent = parent;
        return this;
    }

    getBase() {
        let current = this;
        while (current.parent !== null) current = current.parent;
        return current;
    }

    getSymbols() {
        let result = new Set();
        if (this.parent !== null) {
            for (const symbol of this.parent.getSymbols()) {
                result.add(symbol);
            }
        }
        for (const symbol of this.map.keys()) {
            result.add(symbol);
        }
        result = [...result];
        result.sort();
        return result;
    }

    getLocalSymbols() {
        return this.map.keys();
    }

    getModules() {
        return this.getBase().modules;
    }

    pushModuleStack(moduleidentifier, pos) {
        let base = this.getBase();
        if (base.modulestack.includes(moduleidentifier)) throw new RuntimeError("ERROR", "Found circular module dependency (" + moduleidentifier + ")", pos);
        base.modulestack.push(moduleidentifier);
    }

    popModuleStack() {
        this.getBase().modulestack.pop();
    }

    put(name, value) {
        this.map.set(name, value);
    }

    set(name, value) {
        if (this.map.has(name)) this.map.set(name, value);
        else if (this.parent !== null) this.parent.set(name, value);
        else throw new RuntimeError("ERROR", name + " is not defined");
    }

    remove(name) {
        this.map.delete(name);
    }

    newEnv() {
        return new Environment(this);
    }

    isDefined(symbol) {
        if (this.map.has(symbol)) return true;
        if (this.parent !== null) return this.parent.isDefined(symbol);
        return false;
    }

    get(symbol, pos) {
        if (this.map.has(symbol)) {
            const value = this.map.get(symbol);
            if (value === null) return ValueNull.NULL;
            if (value instanceof Value) {
                return value;
            } else if (value instanceof Number) {
                if (Math.trunc(value) == value) return new ValueInt(value);
                return new ValueDecimal(value);
            } else if (value instanceof BigInt) {
                return new ValueInt(Number(value));
            } else if (value instanceof Boolean) {
                return ValueBoolean.from(value);
            } else if (value instanceof RegExp) {
                return new ValuePattern(value.source);
            } else if (value instanceof Date) {
                return new ValueDate(value);
            } else {
                return new ValueString(value.toString());
            }
        }
        if (this.parent !== null) return this.parent.get(symbol, pos);
        throw new RuntimeError("ERROR", "Symbol '" + symbol + "' not defined", pos);
    }
}

const bind_native = function(environment, native, alias = null) {
    switch (native) {
        case "acos": bind_native_fun(environment, new FuncAcos(), alias); break;
        case "add": bind_native_fun(environment, new FuncAdd(), alias); break;
        case "append": bind_native_fun(environment, new FuncAppend(), alias); break;
        case "asin": bind_native_fun(environment, new FuncAsin(), alias); break;
        case "atan": bind_native_fun(environment, new FuncAtan(), alias); break;
        case "atan2": bind_native_fun(environment, new FuncAtan2(), alias); break;
        case "bind_native": bind_native_fun(environment, new FuncBindNative(), alias); break;
        case "bit_and": bind_native_fun(environment, new FuncBitAnd(), alias); break;
        case "bit_or": bind_native_fun(environment, new FuncBitOr(), alias); break;
        case "bit_not": bind_native_fun(environment, new FuncBitNot(), alias); break;
        case "bit_xor": bind_native_fun(environment, new FuncBitXor(), alias); break;
        case "bit_rotate_left": bind_native_fun(environment, new FuncBitRotateLeft(), alias); break;
        case "bit_rotate_right": bind_native_fun(environment, new FuncBitRotateRight(), alias); break;
        case "bit_shift_left": bind_native_fun(environment, new FuncBitShiftLeft(), alias); break;
        case "bit_shift_right": bind_native_fun(environment, new FuncBitShiftRight(), alias); break;
        case "body": bind_native_fun(environment, new FuncBody(), alias); break;
        case "boolean": bind_native_fun(environment, new FuncBoolean(), alias); break;
        case "ceiling": bind_native_fun(environment, new FuncCeiling(), alias); break;
        case "chr": bind_native_fun(environment, new FuncChr(), alias); break;
        case "close": bind_native_fun(environment, new FuncClose(), alias); break;
        case "compare": bind_native_fun(environment, new FuncCompare(), alias); break;
        case "contains": bind_native_fun(environment, new FuncContains(), alias); break;
        case "cos": bind_native_fun(environment, new FuncCos(), alias); break;
        case "date": bind_native_fun(environment, new FuncDate(), alias); break;
        case "decimal": bind_native_fun(environment, new FuncDecimal(), alias); break;
        case "delete_at": bind_native_fun(environment, new FuncDeleteAt(), alias); break;
        case "div": bind_native_fun(environment, new FuncDiv(), alias); break;
        case "ends_with": bind_native_fun(environment, new FuncEndsWith(), alias); break;
        case "equals": bind_native_fun(environment, new FuncEquals(), alias); break;
        case "escape_pattern": bind_native_fun(environment, new FuncEscapePattern(), alias); break;
        case "eval": bind_native_fun(environment, new FuncEval(), alias); break;
        case "execute": bind_native_fun(environment, new FuncExecute(system), alias); break;
        case "exp": bind_native_fun(environment, new FuncExp(), alias); break;
        case "file_input": bind_native_fun(environment, new FuncFileInput(system.fs)); break;
        case "file_copy": bind_native_fun(environment, new FuncFileCopy(system), alias); break;
        case "file_delete": bind_native_fun(environment, new FuncFileDelete(system)); break;
        case "file_exists": bind_native_fun(environment, new FuncFileExists(system)); break;
        case "file_info": bind_native_fun(environment, new FuncFileInfo(system)); break;
        case "file_move": bind_native_fun(environment, new FuncFileMove(system), alias); break;
        case "file_output": bind_native_fun(environment, new FuncFileOutput(system.fs)); break;
        case "find": bind_native_fun(environment, new FuncFind(), alias); break;
        case "find_last": bind_native_fun(environment, new FuncFindLast(), alias); break;
        case "floor": bind_native_fun(environment, new FuncFloor(), alias); break;
        case "format_date": bind_native_fun(environment, new FuncFormatDate(), alias); break;
        case "get_env": bind_native_fun(environment, new FuncGetEnv(system), alias); break;
        case "get_output_string": bind_native_fun(environment, new FuncGetOutputString(), alias); break;
        case "greater": bind_native_fun(environment, new FuncGreater(), alias); break;
        case "greater_equals": bind_native_fun(environment, new FuncGreaterEquals(), alias); break;
        case "identity": bind_native_fun(environment, new FuncIdentity(), alias); break;
        case "if_empty": bind_native_fun(environment, new FuncIfEmpty(), alias); break;
        case "if_null": bind_native_fun(environment, new FuncIfNull(), alias); break;
        case "if_null_or_empty": bind_native_fun(environment, new FuncIfNullOrEmpty(), alias); break;
        case "info": bind_native_fun(environment, new FuncInfo(), alias); break;
        case "insert_at": bind_native_fun(environment, new FuncInsertAt(), alias); break;
        case "int": bind_native_fun(environment, new FuncInt(), alias); break;
        case "is_empty": bind_native_fun(environment, new FuncIsEmpty(), alias); break;
        case "is_not_empty": bind_native_fun(environment, new FuncIsNotEmpty(), alias); break;
        case "is_not_null": bind_native_fun(environment, new FuncIsNotNull(), alias); break;
        case "is_null": bind_native_fun(environment, new FuncIsNull(), alias); break;
        case "length": bind_native_fun(environment, new FuncLength(), alias); break;
        case "less": bind_native_fun(environment, new FuncLess(), alias); break;
        case "less_equals": bind_native_fun(environment, new FuncLessEquals(), alias); break;
        case "list": bind_native_fun(environment, new FuncList(), alias); break;
        case "list_dir": bind_native_fun(environment, new FuncListDir(system)); break;
        case "log": bind_native_fun(environment, new FuncLog(), alias); break;
        case "lower": bind_native_fun(environment, new FuncLower(), alias); break;
        case "ls": bind_native_fun(environment, new FuncLs(), alias); break;
        case "make_dir": bind_native_fun(environment, new FuncMakeDir(system)); break;
        case "map": bind_native_fun(environment, new FuncMap(), alias); break;
        case "matches": bind_native_fun(environment, new FuncMatches(), alias); break;
        case "mod": bind_native_fun(environment, new FuncMod(), alias); break;
        case "mul": bind_native_fun(environment, new FuncMul(), alias); break;
        case "not_equals": bind_native_fun(environment, new FuncNotEquals(), alias); break;
        case "object": bind_native_fun(environment, new FuncObject(), alias); break;
        case "ord": bind_native_fun(environment, new FuncOrd(), alias); break;
        case "parse": bind_native_fun(environment, new FuncParse(), alias); break;
        case "parse_date": bind_native_fun(environment, new FuncParseDate(), alias); break;
        case "parse_json": bind_native_fun(environment, new FuncParseJson(), alias); break;
        case "pattern": bind_native_fun(environment, new FuncPattern(), alias); break;
        case "pow": bind_native_fun(environment, new FuncPow(), alias); break;
        case "print": bind_native_fun(environment, new FuncPrint(), alias); break;
        case "println": bind_native_fun(environment, new FuncPrintln(), alias); break;
        case "process_lines": bind_native_fun(environment, new FuncProcessLines(), alias); break;
        case "put": bind_native_fun(environment, new FuncPut(), alias); break;
        case "random": bind_native_fun(environment, new FuncRandom(), alias); break;
        case "range": bind_native_fun(environment, new FuncRange(), alias); break;
        case "read": bind_native_fun(environment, new FuncRead(), alias); break;
        case "read_all": bind_native_fun(environment, new FuncReadall(), alias); break;
        case "readln": bind_native_fun(environment, new FuncReadln(), alias); break;
        case "remove": bind_native_fun(environment, new FuncRemove(), alias); break;
        case "round": bind_native_fun(environment, new FuncRound(), alias); break;
        case "s": bind_native_fun(environment, new FuncS(), alias); break;
        case "set": bind_native_fun(environment, new FuncSet(), alias); break;
        case "set_seed": bind_native_fun(environment, new FuncSetSeed(), alias); break;
        case "sin": bind_native_fun(environment, new FuncSin(), alias); break;
        case "sorted": bind_native_fun(environment, new FuncSorted(), alias); break;
        case "split": bind_native_fun(environment, new FuncSplit(), alias); break;
        case "split2": bind_native_fun(environment, new FuncSplit2(), alias); break;
        case "sqrt": bind_native_fun(environment, new FuncSqrt(), alias); break;
        case "str_input": bind_native_fun(environment, new FuncStrInput(), alias); break;
        case "starts_with": bind_native_fun(environment, new FuncStartsWith(), alias); break;
        case "str_output": bind_native_fun(environment, new FuncStrOutput(), alias); break;
        case "string": bind_native_fun(environment, new FuncString(), alias); break;
        case "sub": bind_native_fun(environment, new FuncSub(), alias); break;
        case "sublist": bind_native_fun(environment, new FuncSublist(), alias); break;
        case "substr": bind_native_fun(environment, new FuncSubstr(), alias); break;
        case "sum": bind_native_fun(environment, new FuncSum(), alias); break;
        case "tan": bind_native_fun(environment, new FuncTan(), alias); break;
        case "timestamp": bind_native_fun(environment, new FuncTimestamp(), alias); break;
        case "trim": bind_native_fun(environment, new FuncTrim(), alias); break;
        case "type": bind_native_fun(environment, new FuncType(), alias); break;
        case "upper": bind_native_fun(environment, new FuncUpper(), alias); break;
        case "zip": bind_native_fun(environment, new FuncZip(), alias); break;
        case "zip_map": bind_native_fun(environment, new FuncZipMap(), alias); break;
        case "E": environment.put("E", new ValueDecimal(Math.E).withInfo("E\n\nThe mathematical constant E (Eulers number)")); break;
        case "PI": environment.put("PI", new ValueDecimal(Math.PI).withInfo("PI\n\nThe mathematical constant PI")); break;
        case "PS": environment.put("PS", new ValueString(system.path !== null ? system.path.sep : "/").withInfo("PS\n\nThe OS path separator (posix: /, windows: \\).")); break;
        case "LS": environment.put("LS", new ValueString(system.os !== null ? system.os.EOL : "\n").withInfo("PS\n\nThe OS line separator (posix: \\n, windows: \\r\\n).")); break;
        case "FS": environment.put("FS", new ValueString(system.path !== null ? system.path.delimiter : ":").withInfo("FS\n\nThe OS field separator (posix: :, windows: ;).")); break;
        case "OS_NAME": environment.put("OS_NAME", new ValueString(get_os_name()).withInfo("OS_NAME\n\nThe name of the operating system, one of Windows, Linux, macOS")); break;
        case "OS_VERSION": environment.put("OS_VERSION", new ValueString(get_os_version()).withInfo("OS_VERSION\n\nThe version of the operating system.")); break;
        case "OS_ARCH": environment.put("OS_ARCH", new ValueString(get_os_arch()).withInfo("OS_ARCH\n\nThe architecture of the operating system, one of x86, amd64.")); break;
        default: throw new RuntimeError("ERROR", "Unknown native " + native, this.pos);
    }
}

const bind_native_fun = function(environment, func, alias) {
    if (environment.getBase().get("checkerlang_secure_mode").value && !func.secure) return;
    Environment.add(environment, func, alias);
};

const get_os_version = function() {
    if (system.os === null) return "Unknown";
    return system.os.release() + " " + system.os.version();
}

const get_os_name = function() {
    if (system.os === null) return "Unknown";
    const type = system.os.type();
    if (type === "Linux") return "Linux";
    if (type === "Darwin") return "macOS";
    if (type === "Windows_NT") return "Windows";
    return "Unknown";
};

const get_os_arch = function() {
    if (system.os === null) return "Unkown";
    const arch = system.os.arch();
    if (arch == "x32" || arch == "ia32") return "x86";
    if (arch == "x64") return "amd64";
    return "Unknown";
};

export class FuncAcos extends ValueFunc {
    constructor() {
        super("acos");
        this.info = "acos(x)\r\n" +
                "\r\n" +
                "Returns the arcus cosinus of x.\r\n" +
                "\r\n" +
                ": acos(1) ==> 0.0\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.acos(args.getNumerical("x").value));
    }
}

export class FuncAdd extends ValueFunc {
    constructor() {
        super("add");
        this.info = "add(a, b)\r\n" +
                "\r\n" +
                "Returns the sum of a and b. For numerical values this uses usual arithmetic.\r\n" +
                "For lists and strings it concatenates. For sets it uses union.\r\n" +
                "\r\n" +
                ": add(1, 2) ==> 3\r\n" +
                ": add(date('20100201'), 3) ==> '20100204000000'\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isInt() && b.isInt()) {
            return new ValueInt(a.value + b.value);
        }

        if (a.isNumerical() && b.isNumerical()) {
            return new ValueDecimal(a.asDecimal().value + b.asDecimal().value);
        }

        if (a.isList()) {
            if (b.isCollection()) {
                return new ValueList().addItems(a.asList().value).addItems(b.asList().value);
            } else {
                return new ValueList().addItems(a.asList().value).addItem(b);
            }
        }

        if (a.isSet()) {
            if (b.isCollection()) {
                return new ValueSet().addItems(a.asSet().value.values()).addItems(b.asSet().value.values());
            } else {
                return new ValueSet().addItems(a.asSet().value.values()).addItem(b);
            }
        }

        if (b.isList()) {
            const result = new ValueList();
            result.addItem(a);
            result.addItems(b.asList().value);
            return result;
        }

        if (b.isSet()) {
            const result = new ValueSet();
            result.addItem(a);
            result.addItems(b.asSet().value.values());
            return result;
        }

        if (a.isDate() && b.isNumerical()) {
            return new ValueDate(new Date(convertOADateToDate(convertDateToOADate(a.value) + args.getAsDecimal("b").value)));
        }

        if ((a.isString() && b.isAtomic()) || (a.isAtomic() && b.isString())) {
            return new ValueString(a.asString().value + b.asString().value);
        }

        throw new RuntimeError("ERROR", "Cannot add " + a.type() + " and " + b.type(), pos);
    }
}

export class FuncAppend extends ValueFunc {
    constructor() {
        super("append");
        this.info = "append(lst, element)\r\n" +
                "\r\n" +
                "Appends the element to the list lst. The lst may also be a set.\r\n" +
                "Returns the changed list.\r\n" +
                "\r\n" +
                ": append([1, 2], 3) ==> [1, 2, 3]\r\n" +
                ": append(set([1, 2]), 3) ==> set([1, 2, 3])\r\n";
    }

    getArgNames() {
        return ["lst", "element"];
    }

    execute(args, environment, pos) {
        const lst = args.get("lst");
        const element = args.get("element");

        if (lst.isList()) {
            lst.addItem(element);
            return lst;
        }

        if (lst.isSet()) {
            lst.addItem(element);
            return lst;
        }

        throw new RuntimeError("ERROR", "Cannot append to " + lst.type(), pos);
    }
}

export class FuncAsin extends ValueFunc {
    constructor() {
        super("asin");
        this.info = "asin(x)\r\n" +
                "\r\n" +
                "Returns the arcus sinus of x.\r\n" +
                "\r\n" +
                ": asin(0) ==> 0.0\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.asin(args.getNumerical("x").value));
    }
}

export class FuncAtan extends ValueFunc {
    constructor() {
        super("atan");
        this.info = "atan(x)\r\n" +
                "\r\n" +
                "Returns the arcus tangens of x.\r\n" +
                "\r\n" +
                ": atan(0) ==> 0.0\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.atan(args.getNumerical("x").value));
    }
}

export class FuncAtan2 extends ValueFunc {
    constructor() {
        super("atan2");
        this.info = "atan2(y, x)\r\n" +
                "\r\n" +
                "Returns the arcus tangens of y / x.\r\n" +
                "\r\n" +
                ": atan2(0, 1) ==> 0.0\r\n";
    }

    getArgNames() {
        return ["y", "x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("y")) return ValueNull.NULL;
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.atan2(args.getNumerical("y").value, args.getNumerical("x").value));
    }
}

export class FuncBindNative extends ValueFunc {
    constructor() {
        super("bind_native");
        this.info = "bind_native(native)\r\n" +
                    "bind_native(native, alias)\r\n" +
                    "\r\n" +
                    "Binds a native function in the current environment.\r\n";
    }

    getArgNames() {
        return ["native", "alias"];
    }

    execute(args, environment, pos) {
        const native = args.getString("native").value;
        let alias = null;
        if (args.hasArg("alias")) alias = args.getString("alias").value;
        bind_native(environment, native, alias);
        return ValueNull.NULL;
    }
}

export class FuncBitAnd extends ValueFunc {
    constructor() {
        super("bit_and");
        this.info = "bit_and(a, b)\r\n" +
                    "\r\n" +
                    "Performs bitwise and for the two 32bit values a and b.\r\n" +
                    ": bit_and(5, 6) ==> 4\r\n" +
                    ": bit_and(4, 4) ==> 4\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.getInt("a").value;
        const b = args.getInt("b").value;
        return new ValueInt((a & b) >>> 0);
    }
}

export class FuncBitOr extends ValueFunc {
    constructor() {
        super("bit_or");
        this.info = "bit_or(a, b)\r\n" +
                    "\r\n" +
                    "Performs bitwise or for the two 32bit values a and b.\r\n" +
                    ": bit_or(1, 2) ==> 3\r\n" +
                    ": bit_or(3, 4) ==> 7\r\n" +
                    ": bit_or(4, 4) ==> 4\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.getInt("a").value;
        const b = args.getInt("b").value;
        return new ValueInt((a | b) >>> 0);
    }
}

export class FuncBitNot extends ValueFunc {
    constructor() {
        super("bit_not");
        this.info = "bit_not(a)\r\n" +
                    "\r\n" +
                    "Performs bitwise not for the 32bit value a.\r\n" +
                    ": bit_not(1) ==> 4294967294\r\n" +
                    ": bit_not(0) ==> 4294967295\r\n";
    }

    getArgNames() {
        return ["a"];
    }

    execute(args, environment, pos) {
        const a = args.getInt("a").value;
        return new ValueInt((~a) >>> 0);
    }
}

export class FuncBitXor extends ValueFunc {
    constructor() {
        super("bit_xor");
        this.info = "bit_xor(a, b)\r\n" +
                    "\r\n" +
                    "Performs bitwise xor for the two 32bit values a and b.\r\n" +
                    ": bit_xor(1, 2) ==> 3\r\n" +
                    ": bit_xor(1, 3) ==> 2\r\n" +
                    ": bit_xor(4, 4) ==> 0\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.getInt("a").value;
        const b = args.getInt("b").value;
        return new ValueInt((a ^ b) >>> 0);
    }
}

export class FuncBitRotateLeft extends ValueFunc {
    constructor() {
        super("bit_rotate_left");
        this.info = "bit_rotate_left(a, n)\r\n" +
                    "\r\n" +
                    "Performs bitwise rotate of 32bit value a by n bits to the left.\r\n" +
                    ": bit_rotate_left(1, 2) ==> 4\r\n" +
                    ": bit_rotate_left(1, 3) ==> 8\r\n" +
                    ": bit_rotate_left(4, 4) ==> 64\r\n";
    }

    getArgNames() {
        return ["a", "n"];
    }

    execute(args, environment, pos) {
        const a = args.getInt("a").value;
        const n = args.getInt("n").value;
        return new ValueInt(((a << n) | (a >>> (32 - n))) >>> 0);
    }
}

export class FuncBitRotateRight extends ValueFunc {
    constructor() {
        super("bit_rotate_right");
        this.info = "bit_rotate_right(a, n)\r\n" +
                    "\r\n" +
                    "Performs bitwise rotate of 32bit value a by n bits to the right.\r\n" +
                    ": bit_rotate_right(1, 2) ==> 1073741824\r\n" +
                    ": bit_rotate_right(1, 3) ==> 536870912\r\n" +
                    ": bit_rotate_right(4, 4) ==> 1073741824\r\n";
    }

    getArgNames() {
        return ["a", "n"];
    }

    execute(args, environment, pos) {
        const a = args.getInt("a").value;
        const n = args.getInt("n").value;
        return new ValueInt(((a >>> n) | (a << (32 - n))) >>> 0);
    }
}

export class FuncBitShiftLeft extends ValueFunc {
    constructor() {
        super("bit_shift_left");
        this.info = "bit_shift_left(a, n)\r\n" +
                    "\r\n" +
                    "Performs bitwise shift of 32bit value a by n bits to the left.\r\n" +
                    ": bit_shift_left(1, 2) ==> 4\r\n" +
                    ": bit_shift_left(1, 3) ==> 8\r\n" +
                    ": bit_shift_left(1, 4) ==> 16\r\n";
    }

    getArgNames() {
        return ["a", "n"];
    }

    execute(args, environment, pos) {
        const a = args.getInt("a").value;
        const n = args.getInt("n").value;
        return new ValueInt(a << n);
    }
}

export class FuncBitShiftRight extends ValueFunc {
    constructor() {
        super("bit_shift_right");
        this.info = "bit_shift_right(a, n)\r\n" +
                    "\r\n" +
                    "Performs bitwise shift of 32bit value a by n bits to the right.\r\n" +
                    ": bit_shift_right(4, 1) ==> 2\r\n" +
                    ": bit_shift_right(4, 3) ==> 0\r\n" +
                    ": bit_shift_right(4, 2) ==> 1\r\n";
    }

    getArgNames() {
        return ["a", "n"];
    }

    execute(args, environment, pos) {
        const a = args.getInt("a").value;
        const n = args.getInt("n").value;
        return new ValueInt(a >>> n);
    }
}

export class FuncBody extends ValueFunc {
    constructor() {
        super("body");
        this.info = "body(f)\r\n" +
                "\r\n" +
                "Returns the body of the lambda f.\r\n" +
                "\r\n" +
                ": body(fn(x) 2 * x) ==> '(mul 2, x)'\r\n";
    }

    getArgNames() {
        return ["f"];
    }

    execute(args, environment, pos) {
        const f = args.get("f");
        if (f instanceof FuncLambda)
        {
            return new ValueNode(f.body);
        }
        throw new RuntimeError("ERROR", "f is not a lambda function", pos);
    }

}

export class FuncBoolean extends ValueFunc {
    constructor() {
        super("boolean");
        this.info = "boolean(obj)\r\n" +
                "\r\n" +
                "Converts the obj to a boolean, if possible.\r\n" +
                "\r\n" +
                ": boolean(1) ==> TRUE\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return args.getAsBoolean("obj");
    }
}

export class FuncCeiling extends ValueFunc {
    constructor() {
        super("ceiling");
        this.info = "ceiling(x)\r\n" +
                "\r\n" +
                "Returns the integral decimal value that is equal to or next higher than x.\r\n" +
                "\r\n" +
                ": ceiling(1.3) ==> 2.0\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.ceil(args.getNumerical("x").value));
    }
}

export class FuncChr extends ValueFunc {
    constructor() {
        super("chr");
        this.info = "chr(n)\r\n" +
                "\r\n" +
                "Returns a single character string for the code point integer n.\r\n" +
                "\r\n" +
                ": chr(97) ==> 'a'\r\n" +
                ": chr(32) ==> ' '\r\n";
    }

    getArgNames() {
        return ["n"];
    }

    execute(args, environment, pos) {
        if (args.isNull("n")) return ValueNull.NULL;
        return new ValueString(String.fromCodePoint(args.getInt("n").value));
    }
}

export class FuncClose extends ValueFunc {
    constructor() {
        super("close");
        this.info = "close(conn)\r\n" +
                "\r\n" +
                "Closes the input or output connection and releases system resources.\r\n";
    }

    getArgNames() {
        return ["conn"];
    }

    execute(args, environment, pos) {
        const conn = args.get("conn");
        if (conn.isInput() || conn.isOutput()) {
            try {
                conn.close();
            } catch (e) {
                throw new RuntimeError("ERROR", "Could not close connection", pos);
            }
            return ValueNull.NULL;
        }
        throw new RuntimeError("ERROR", "Cannot close " + conn.type(), pos);
    }
}

export class FuncCompare extends ValueFunc {
    constructor() {
        super("compare");
        this.info = "compare(a, b)\r\n" +
                "\r\n" +
                "Returns an int less than 0 if a is less than b,\r\n" +
                "0 if a is equal to b, and an int greater than 0\r\n" +
                "if a is greater than b.\r\n" +
                "\r\n" +
                ": compare(1, 2) < 0 ==> TRUE\r\n" +
                ": compare(3, 1) > 0 ==> TRUE\r\n" +
                ": compare(1, 1) == 0 ==> TRUE\r\n" +
                ": compare('1', 2) < 0 ==> TRUE\r\n" +
                ": compare('2', 1) < 0 ==> TRUE\r\n" +
                ": compare(100, '100') > 0 ==> TRUE\r\n" +
                ": compare(NULL, 1) > 0 ==> TRUE\r\n" +
                ": compare(NULL, NULL) == 0 ==> TRUE\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");

        return new ValueInt(a.compareTo(b));
    }
}

export class FuncContains extends ValueFunc {
    constructor() {
        super("contains");
        this.info = "contains(obj, part)\r\n" +
                "\r\n" +
                "Returns TRUE if the string obj contains part.\r\n" +
                "If obj is a list, set or map, TRUE is returned,\r\n" +
                "if part is contained.\r\n" +
                "\r\n" +
                ": contains('abcdef', 'abc') ==> TRUE\r\n" +
                ": contains('abcdef', 'cde') ==> TRUE\r\n" +
                ": contains('abcdef', 'def') ==> TRUE\r\n" +
                ": contains('abcdef', 'efg') ==> FALSE\r\n" +
                ": contains(NULL, 'abc') ==> FALSE\r\n" +
                ": contains([1, 2, 3], 2) ==> TRUE\r\n" + 
                ": <<1, 2, 3>> !> contains(3) ==> TRUE\r\n" +
                ": <<<a => 1, b => 2>>> !> contains('b') ==> TRUE\r\n";
    }

    getArgNames() {
        return ["obj", "part"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueBoolean.FALSE;
        const obj = args.get("obj");
        if (obj.isList()) {
            for (const item of obj.value) {
                if (item.isEquals(args.get("part"))) return ValueBoolean.TRUE;
            }
            return ValueBoolean.FALSE;
        } else if (obj.isSet() || obj.isMap()) {
            return ValueBoolean.from(obj.value.has(args.get("part")));
        } 
        return ValueBoolean.from(obj.asString().value.indexOf(args.getString("part").value) != -1);
    }
}

export class FuncCos extends ValueFunc {
    constructor() {
        super("cos");
        this.info = "cos(x)\r\n" +
                "\r\n" +
                "Returns the cosinus of x.\r\n" +
                "\r\n" +
                ": cos(PI) ==> -1.0\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.cos(args.getNumerical("x").value));
    }
}

export class FuncDate extends ValueFunc {
    constructor() {
        super("date");
        this.info = "date(obj)\r\n" +
                "\r\n" +
                "Converts the obj to a date, if possible.\r\n" +
                "If obj is a string, the format yyyyMMdd is assumed.\r\n" +
                "If this fails, the fallback yyyyMMddHH is tried.\r\n" +
                "If this fails, the fallback yyyyMMddHHmmss is tried.\r\n" +
                "\r\n" +
                "See parse_date for handling other formats.\r\n" +
                "\r\n" +
                ": string(date('20170102')) ==> '20170102000000'\r\n" +
                ": string(date('2017010212')) ==> '20170102120000'\r\n" +
                ": string(date('20170102123456')) ==> '20170102123456'\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        if (!args.hasArg("obj")) return new ValueDate(new Date());
        return args.getAsDate("obj");
    }
}

export class FuncDecimal extends ValueFunc {
    constructor() {
        super("decimal");
        this.info = "decimal(obj)\r\n" +
                "\r\n" +
                "Converts the obj to a decimal, if possible.\r\n" +
                "\r\n" +
                ": decimal('1.2') ==> 1.2\r\n";
    }


    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return args.getAsDecimal("obj");
    }
}

export class FuncDeleteAt extends ValueFunc {
    constructor() {
        super("delete_at");
        this.info = "delete_at(lst, index)\r\n" +
                "\r\n" +
                "Removes the element at the given index from the list lst.\r\n" +
                "The list is changed in place. Returns the removed element or\r\n" +
                "NULL, if no element was removed\r\n" +
                "\r\n" +
                ": delete_at(['a', 'b', 'c', 'd'], 2) ==> 'c'\r\n" +
                ": delete_at(['a', 'b', 'c', 'd'], -3) ==> 'b'\r\n" +
                ": def lst = ['a', 'b', 'c', 'd']; delete_at(lst, 2); lst ==> ['a', 'b', 'd']\r\n" +
                ": delete_at(['a', 'b', 'c', 'd'], 4) ==> NULL\r\n";
    }

    getArgNames() {
        return ["lst", "index"];
    }

    execute(args, environment, pos) {
        const lst = args.get("lst");
        const index = args.getInt("index").value;

        if (lst.isList()) {
            return lst.deleteAt(index);
        }

        throw new RuntimeError("ERROR", "Cannot delete from " + lst.type(), pos);
    }
}

export class FuncDiv extends ValueFunc {
    constructor() {
        super("div");
        this.info = "div(a, b)\r\n" +
                "\r\n" +
                "Returns the value of a divided by b. If both values are ints,\r\n" +
                "then the result is also an int. Otherwise, it is a decimal.\r\n" +
                "\r\n" +
                ": div(6, 2) ==> 3\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isInt() && b.isInt()) {
            const divisor = b.value;
            if (divisor === 0) {
                if (environment.isDefined("DIV_0_VALUE") &&
                        environment.get("DIV_0_VALUE", pos) != ValueNull.NULL) {
                    return environment.get("DIV_0_VALUE", pos);
                }
                throw new RuntimeError("ERROR", "divide by zero", pos);
            }
            return new ValueInt(Math.trunc(a.value / divisor));
        }

        if (a.isNumerical() && b.isNumerical()) {
            const divisor = b.asDecimal().value;
            if (divisor === 0.0) {
                if (environment.isDefined("DIV_0_VALUE") &&
                        environment.get("DIV_0_VALUE", pos) != ValueNull.NULL) {
                    return environment.get("DIV_0_VALUE", pos);
                }
                throw new RuntimeError("ERROR", "divide by zero", pos);
            }
            return new ValueDecimal(a.asDecimal().value / divisor);
        }

        throw new RuntimeError("ERROR", "Cannot divide " + a.type() + " by " + b.type(), pos);
    }
}

export class FuncEndsWith extends ValueFunc {
    constructor() {
        super("ends_with");
        this.info = "ends_with(str, part)\r\n" +
                "\r\n" +
                "Returns TRUE if the string str ends with part.\r\n" +
                "\r\n" +
                ": ends_with('abcdef', 'def') ==> TRUE\r\n" +
                ": ends_with('abcdef', 'abc') ==> FALSE\r\n" +
                ": ends_with(NULL, 'abc') ==> FALSE\r\n";
    }

    getArgNames() {
        return ["str", "part"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueBoolean.FALSE;
        return ValueBoolean.from(args.getString("str").value.endsWith(args.getString("part").value));
    }
}

export class FuncEquals extends ValueFunc {
    constructor() {
        super("equals");
        this.info = "equals(a, b)\r\n" +
                "\r\n" +
                "Returns TRUE if a is equals to b.\r\n" +
                "\r\n" +
                "Integer values are propagated to decimal values, if required.\r\n" +
                "\r\n" +
                ": equals(1, 2) ==> FALSE\r\n" +
                ": equals(1, 1) ==> TRUE\r\n" +
                ": equals(1, 1.0) ==> TRUE\r\n" +
                ": equals('a', 'b') ==> FALSE\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");
        return ValueBoolean.from(a.isEquals(b));
    }

}

export class FuncEscapePattern extends ValueFunc {
    constructor() {
        super("escape_pattern");
        this.info = "escape_pattern(s)\r\n" +
                "\r\n" +
                "Escapes special characters in the string s, so that\r\n" +
                "the result can be used in pattern matching to match\r\n" +
                "the literal string.\r\n" +
                "\r\n" +
                "Currently, the | and . characters are escaped.\r\n" +
                "\r\n" +
                ": escape_pattern('|') ==> '\\\\|'\r\n" +
                ": escape_pattern('|.|') ==> '\\\\|\\\\.\\\\|'\r\n";
    }

    getArgNames() {
        return ["s"];
    }

    execute(args, environment, pos) {
        if (args.isNull("s")) return ValueNull.NULL;
        const value = args.getString("s").value;
        return new ValueString(value.replace(/\|/g, "\\|").replace(/\./g, "\\."));
    }

}

export class FuncEval extends ValueFunc {
    constructor() {
        super("eval");
        this.info = "eval(s)\r\n" +
                "\r\n" +
                "Evaluates the string or node s.\r\n" +
                "\r\n" +
                ": eval('1+1') ==> 2\r\n";
    }

    getArgNames() {
        return ["s"];
    }

    execute(args, environment, pos) {
        if (args.get("s").isNode()) {
            return args.getAsNode("s").value.evaluate(environment);
        }
        const s = args.getString("s").value;
        try {
            const node = Parser.parseScript(s, pos.filename);
            return node.evaluate(environment);
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot evaluate expression", pos);
        }
    }
}

export class FuncExecute extends ValueFunc {
    constructor(system) {
        super("execute");
        this.info = "execute(program, args, work_dir = NULL, echo = FALSE, output_file = NULL)\r\n" +
                "\r\n" +
                "Executes the program and provides the specified arguments in the list args.\r\n";
        this.system = system;
        this.secure = false;
    }

    getArgNames() {
        return ["program", "args", "work_dir", "echo", "output_file"];
    }

    execute(args, environment, pos) {
        const program = args.getString("program").value;
        const arglist = [];
        if (!args.get("args").isList()) throw new RuntimeError("ERROR", "Expected argument list but got " + args.get("args").type());
        for (let arg of args.get("args").value) {
            arglist.push(arg.asString().value);
        }
        let work_dir = null;
        if (args.hasArg("work_dir")) work_dir = args.getString("work_dir").value;
        let echo = false;
        if (args.hasArg("echo")) echo = args.getBoolean("echo").value;
        let output_file = null;
        if (args.hasArg("output_file")) output_file = args.getString("output_file").value;
        if (output_file !== null) {
            const options = { cwd: work_dir === null ? undefined : work_dir, stdio: "pipe" };
            if (echo) console.log(program + " " + arglist.join(" "));
            const result = system.child_process.spawnSync(program, arglist, options);
            const output = system.fs.openSync(output_file, "w");
            system.fs.writeSync(output, result.stdout, null);
            system.fs.closeSync(output);
            return result.status === null ? ValueNull.NULL : new ValueInt(result.status);
        } else {
            const options = { cwd: work_dir === null ? undefined : work_dir, stdio: "inherit" };
            if (echo) console.log(program + " " + arglist.join(" "));
            const result = system.child_process.spawnSync(program, arglist, options);
            return result.status === null ? ValueNull.NULL : new ValueInt(result.status);
        }
    }
}

export class FuncExp extends ValueFunc {
    constructor() {
        super("exp");
        this.info = "exp(x)\r\n" +
                "\r\n" +
                "Returns the power e ^ x.\r\n" +
                "\r\n" +
                ": exp(0) ==> 1\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.exp(args.getNumerical("x").value));
    }
}

export class FuncFileInput extends ValueFunc {
    constructor(fs) {
        super("file_input");
        this.info = "file_input(filename, encoding = 'UTF-8')\r\n" +
                "\r\n" +
                "Returns an input object, that reads the characters from the given file.\r\n";
        this.fs = fs;
        this.secure = false;
    }

    getArgNames() {
        return ["filename", "encoding"];
    }

    execute(args, environment, pos) {
        const filename = args.getString("filename").value;
        let encoding = "utf-8";
        if (args.hasArg("encoding")) {
            encoding = args.getString("encoding").value;
        }
        try {
            return new ValueInput(new FileInput(filename, encoding, this.fs));
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot open file " + filename, pos);
        }
    }
}

export class FuncFileCopy extends ValueFunc {
    constructor(system) {
        super("file_copy");
        this.info = "file_copy(src, dest)\r\n" +
                "\r\n" +
                "Copies the specified file.\r\n";
        this.fs = system.fs;
        this.path = system.path;
        this.secure = false;
    }

    getArgNames() {
        return ["src", "dest"];
    }

    execute(args, environment, pos) {
        const src = args.getString("src").value;
        const dest = args.getString("dest").value;
        this.fs.copyFileSync(src, dest);
        return ValueNull.NULL;
    }
}

export class FuncFileDelete extends ValueFunc {
    constructor(system) {
        super("file_delete");
        this.info = "file_delete(filename)\r\n" +
                "\r\n" +
                "Deletes the specified file.\r\n";
        this.fs = system.fs;
        this.path = system.path;
        this.secure = false;
    }

    getArgNames() {
        return ["filename"];
    }

    execute(args, environment, pos) {
        const filename = args.getString("filename").value;
        try {
            if (this.fs.lstatSync(filename).isDirectory()) {
                this.fs.rmdirSync(filename);
            } else {
                this.fs.rmSync(filename);
            }
        } catch {
            throw new RuntimeError("ERROR", "Cannot delete file " + filename, pos);
        }
        return ValueNull.NULL;
    }
}

export class FuncFileExists extends ValueFunc {
    constructor(system) {
        super("file_exists");
        this.info = "file_exists(filename)\r\n" +
                "\r\n" +
                "Returns TRUE if the specified file exists.\r\n";
        this.fs = system.fs;
        this.path = system.path;
        this.secure = false;
    }

    getArgNames() {
        return ["filename"];
    }

    execute(args, environment, pos) {
        const filename = args.getString("filename").value;
        return ValueBoolean.from(this.fs.existsSync(filename));
    }
}

export class FuncFileInfo extends ValueFunc {
    constructor(system) {
        super("file_info");
        this.info = "file_info(filename)\r\n" +
                "\r\n" +
                "Returns information about the specified file (e.g. modification date, size).\r\n";
        this.fs = system.fs;
        this.path = system.path;
        this.secure = false;
    }

    getArgNames() {
        return ["filename"];
    }

    execute(args, environment, pos) {
        const filename = args.getString("filename").value;
        const result = new ValueObject();
        try {
            const stats = this.fs.lstatSync(filename);
            result.addItem("size", new ValueInt(stats.size));
            result.addItem("is_dir", ValueBoolean.from(stats.isDirectory()));
            result.addItem("modified", new ValueDate(stats.mtime));
            result.addItem("created", new ValueDate(stats.ctime));
            return result;
        } catch {
            return ValueNull.NULL;
        }
    }
}

export class FuncFileMove extends ValueFunc {
    constructor(system) {
        super("file_move");
        this.info = "file_move(src, dest)\r\n" +
                "\r\n" +
                "Moves the specified file.\r\n";
        this.fs = system.fs;
        this.path = system.path;
        this.secure = false;
    }

    getArgNames() {
        return ["src", "dest"];
    }

    execute(args, environment, pos) {
        const src = args.getString("src").value;
        const dest = args.getString("dest").value;
        this.fs.renameSync(src, dest);
        return ValueNull.NULL;
    }
}

export class FuncFileOutput extends ValueFunc {
    constructor(fs) {
        super("file_output");
        this.fs = fs;
        this.info = "file_output(filename, encoding = 'UTF-8', append = FALSE)\r\n" +
                "\r\n" +
                "Returns an output object, that writes to the given file. If\r\n" +
                "the file exists it is overwritten.\r\n";
        this.secure = false;
    }

    getArgNames() {
        return ["filename", "encoding", "append"];
    }

    execute(args, environment, pos) {
        const filename = args.getString("filename").value;
        let encoding = "utf-8";
        if (args.hasArg("encoding")) {
            encoding = args.getString("encoding").value;
        }
        let append = false;
        if (args.hasArg("append")) {
            append = args.getAsBoolean("append").value;
        }
        try {
            return new ValueOutput(new FileOutput(filename, encoding, append, this.fs));
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot open file " + filename, pos);
        }
    }
}

export class FuncFind extends ValueFunc {
    constructor() {
        super("find");
        this.info = "find(obj, part, key = identity, start = 0)\r\n" +
                "\r\n" +
                "Returns the index of the first occurence of part in obj.\r\n" +
                "If part is not contained in obj, then -1 is returned. Start specifies\r\n" +
                "the search start index. It defaults to 0.\r\n" +
                "Obj can be a string or a list. In case of a string, part can be any\r\n" +
                "substring, in case of a list, a single element.\r\n" +
                "In case of lists, the elements can be accessed using the\r\n" +
                "key function.\r\n" +
                "\r\n" +
                ": find('abcdefg', 'cde') ==> 2\r\n" +
                ": find('abc|def|ghi', '|', start = 4) ==> 7\r\n" +
                ": find('abcxyabc', 'abc', start = 5) ==> 5\r\n" +
                ": find([1, 2, 3, 4], 3) ==> 2\r\n" +
                ": find(['abc', 'def'], 'e', key = fn(x) x[1]) ==> 1\r\n";
    }

    getArgNames() {
        return ["obj", "part", "key", "start"];
    }

    execute(args, environment, pos) {
        if (args.isNull("obj")) return ValueNull.NULL;
        const obj = args.get("obj");
        const start = args.getInt("start", 0).value;
        const key = args.hasArg("key") ? args.getFunc("key") : null;
        if (obj.isString()) {
            const part = args.getString("part").value;
            const start = args.getInt("start", 0).value;
            return new ValueInt(obj.value.indexOf(part, start));
        } else if (obj.isList()) {
            let env = environment;
            if (key !== null) env = environment.newEnv();
            const item = args.get("part");
            const list = obj.value;
            for (let idx = start; idx < list.length; idx++) {
                let elem = list[idx];
                if (key !== null) elem = key.execute(new Args(pos).addArg(key.getArgNames()[0], elem), env, pos);
                if (elem.isEquals(item)) return new ValueInt(idx);
            }
            return new ValueInt(-1);
        }
        throw new RuntimeError("ERROR", "Find only works with strings and lists", pos);
    }
}

export class FuncFindLast extends ValueFunc {
    constructor() {
        super("find_last");
        this.info = "find_last(obj, part, key = identity, start = length(obj) - 1)\r\n" +
                "\r\n" +
                "Returns the index of the last  occurence of part in obj.\r\n" +
                "If part is not contained in obj, then -1 is returned. Start specifies\r\n" +
                "the search start index. It defaults to length(obj) - 1.\r\n" +
                "Obj can be a string or a list. In case of a string, part can be any\r\n" +
                "substring, in case of a list, a single element.\r\n" +
                "In case of lists, the elements can be accessed using the\r\n" +
                "key function.\r\n" +
                "\r\n" +
                ": find_last('abcdefgcdexy', 'cde') ==> 7\r\n" +
                ": find_last('abc|def|ghi|jkl', '|', start = 10) ==> 7\r\n" +
                ": find_last('abcxyabc', 'abc', start = 4) ==> 0\r\n" +
                ": find_last([1, 2, 3, 4, 3], 3) ==> 4\r\n" +
                ": find_last(['abc', 'def'], 'e', key = fn(x) x[1]) ==> 1\r\n";
    }

    getArgNames() {
        return ["obj", "part", "key", "start"];
    }

    execute(args, environment, pos) {
        if (args.isNull("obj")) return ValueNull.NULL;
        const obj = args.get("obj");
        const key = args.hasArg("key") ? args.getFunc("key") : null;
        if (obj.isString()) {
            const str = obj.value;
            const part = args.getString("part").value;
            const start = args.getInt("start", str.length - 1).value;
            return new ValueInt(obj.value.lastIndexOf(part, start));
        } else if (obj.isList()) {
            let env = environment;
            if (key !== null) env = environment.newEnv();
            const item = args.get("part");
            const list = obj.value;
            const start = args.getInt("start", list.length - 1).value;
            for (let idx = start; idx >= 0; idx--) {
                let elem = list[idx];
                if (key !== null) elem = key.execute(new Args(pos).addArg(key.getArgNames()[0], elem), env, pos);
                if (elem.isEquals(item)) return new ValueInt(idx);
            }
            return new ValueInt(-1);
        }
        throw new RuntimeError("ERROR", "Find_last only works with strings and lists", pos);
    }
}

export class FuncFloor extends ValueFunc {
    constructor() {
        super("floor");
        this.info = "floor(x)\r\n" +
                "\r\n" +
                "Returns the integral decimal value that is equal to or next lower than x.\r\n" +
                "\r\n" +
                ": floor(1.3) ==> 1.0\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.floor(args.getNumerical("x").value));
    }
}

export class FuncFormatDate extends ValueFunc {
    constructor() {
        super("format_date");
        this.info = "format_date(date, fmt = 'yyyy-MM-dd HH:mm:ss')\r\n" +
                "\r\n" +
                "Formats the date value according to fmt and returns a string value.\r\n" +
                "\r\n" +
                ": format_date(date('20170102')) ==> '2017-01-02 00:00:00'\r\n" +
                ": format_date(NULL) ==> NULL\r\n" +
                ": format_date(date('2017010212'), fmt = 'HH') ==> '12'\r\n";
    }

    getArgNames() {
        return ["date", "fmt"];
    }

    execute(args, environment, pos) {
        if (args.isNull("date")) return ValueNull.NULL;
        const date = args.getDate("date").value;
        let fmt = args.getString("fmt", "yyyy-MM-dd HH:mm:ss").value;
        if (fmt.indexOf("yyyy") != -1) fmt = fmt.replace(/yyyy/g, this.fill(date.getFullYear(), 4));
        if (fmt.indexOf("yy") != -1) fmt = fmt.replace(/yy/g, this.fill(date.getYear(), 2));
        if (fmt.indexOf("MM") != -1) fmt = fmt.replace(/MM/g, this.fill(date.getMonth() + 1, 2));
        if (fmt.indexOf("dd") != -1) fmt = fmt.replace(/dd/g, this.fill(date.getDate(), 2));
        if (fmt.indexOf("HH") != -1) fmt = fmt.replace(/HH/g, this.fill(date.getHours(), 2));
        if (fmt.indexOf("mm") != -1) fmt = fmt.replace(/mm/g, this.fill(date.getMinutes(), 2));
        if (fmt.indexOf("ss") != -1) fmt = fmt.replace(/ss/g, this.fill(date.getSeconds(), 2));
        return new ValueString(fmt);
    }

    fill(val, len = 2) {
        let result = val.toString();
        while (result.length < len) result = "0" + result;
        return result;
    }
}

export class FuncGetEnv extends ValueFunc {
    constructor(system) {
        super("get_env");
        this.info = "get_env(var)\r\n" +
                "\r\n" +
                "Returns the value of the environment variable var.\r\n";
        this.system = system;
        this.secure = false;
    }

    getArgNames() {
        return ["var"];
    }

    execute(args, environment, pos) {
        return new ValueString(system.process.env[args.getString("var").value] || "");
    }
}

export class FuncGetOutputString extends ValueFunc {
    constructor() {
        super("get_output_string");
        this.info = "get_output_string(output)\r\n" +
                "\r\n" +
                "Returns the value of a string output object.\r\n" +
                "\r\n" +
                ": do def o = str_output(); print('abc', out = o); get_output_string(o); end ==> 'abc'\r\n";
    }

    getArgNames() {
        return ["output"];
    }

    execute(args, environment, pos) {
        const output = args.getOutput("output");
        return new ValueString(output.output.output);
    }
}

export class FuncGreater extends ValueFunc {
    constructor() {
        super("greater");
        this.info = "greater(a, b)\r\n" +
                "\r\n" +
                "Returns TRUE if a is greater than b.\r\n" +
                "\r\n" +
                ": greater(1, 2) ==> FALSE\r\n" +
                ": greater(1, 1) ==> FALSE\r\n" +
                ": greater(2, 1) ==> TRUE\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");
        return ValueBoolean.from(a.compareTo(b) > 0);
    }
}

export class FuncGreaterEquals extends ValueFunc {
    constructor() {
        super("greater_equals");
        this.info = "greater_equals(a, b)\r\n" +
                "\r\n" +
                "Returns TRUE if a is greater than or equals to b.\r\n" +
                "\r\n" +
                ": greater_equals(1, 2) ==> FALSE\r\n" +
                ": greater_equals(1, 1) ==> TRUE\r\n" +
                ": greater_equals(2, 1) ==> TRUE\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");
        return ValueBoolean.from(a.compareTo(b) >= 0);
    }
}

export class FuncIdentity extends ValueFunc {
    constructor() {
        super("identity");
        this.info = "identity(obj)\r\n" +
                "\r\n" +
                "Returns obj.\r\n" +
                "\r\n" +
                ": identity(1) ==> 1\r\n" +
                ": identity('a') ==> 'a'\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return args.get("obj");
    }
}

export class FuncIfEmpty extends ValueFunc {
    constructor() {
        super("if_empty");
        this.info = "if_empty(a, b)\r\n" +
                "\r\n" +
                "Returns b if a is an empty string otherwise returns a.\r\n" +
                "\r\n" +
                ": if_empty(1, 2) ==> 1\r\n" +
                ": if_empty('', 2) ==> 2\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        if (a.isString() && a.value.length === 0) return args.get("b");
        return a;
    }
}

export class FuncIfNull extends ValueFunc {
    constructor() {
        super("if_null");
        this.info = "if_null(a, b)\r\n" +
                "\r\n" +
                "Returns b if a is NULL otherwise returns a.\r\n" +
                "\r\n" +
                ": if_null(1, 2) ==> 1\r\n" +
                ": if_null(NULL, 2) ==> 2\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        if (a.isNull()) return args.get("b");
        return a;
    }
}

export class FuncIfNullOrEmpty extends ValueFunc {
    constructor() {
        super("if_null_or_empty");
        this.info = "if_null_or_empty(a, b)\r\n" +
                "\r\n" +
                "Returns b if a is null or an empty string otherwise returns a.\r\n" +
                "\r\n" +
                ": if_null_or_empty(1, 2) ==> 1\r\n" +
                ": if_null_or_empty(NULL, 2) ==> 2\r\n" +
                ": if_null_or_empty('', 2) ==> 2\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        if (a.isNull()) return args.get("b");
        if (a.isString() && a.value.length === 0) return args.get("b");
        return a;
    }
}

export class FuncInfo extends ValueFunc {
    constructor() {
        super("info");
        this.info = "info(obj)\r\n" +
                "\r\n" +
                "Returns the info associated with an object.\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return new ValueString(args.get("obj").info);
    }
}

export class FuncInsertAt extends ValueFunc {
    constructor() {
        super("insert_at");
        this.info = "insert_at(lst, index, value)\r\n" +
                "\r\n" +
                "Inserts the element at the given index of the list lst.\r\n" +
                "The list is changed in place. Returns the changed list.\r\n" +
                "If index is out of bounds, the list is not changed at all.\r\n" +
                "\r\n" +
                ": insert_at([1, 2, 3], 0, 9) ==> [9, 1, 2, 3]\r\n" +
                ": insert_at([1, 2, 3], 2, 9) ==> [1, 2, 9, 3]\r\n" +
                ": insert_at([1, 2, 3], 3, 9) ==> [1, 2, 3, 9]\r\n" +
                ": insert_at([1, 2, 3], -1, 9) ==> [1, 2, 3, 9]\r\n" +
                ": insert_at([1, 2, 3], -2, 9) ==> [1, 2, 9, 3]\r\n" +
                ": insert_at([1, 2, 3], 4, 9) ==> [1, 2, 3]\r\n";
    }

    getArgNames() {
        return ["lst", "index", "value"];
    }

    execute(args, environment, pos) {
        const lst = args.get("lst");

        if (!lst.isList()) throw new RuntimeError("ERROR", "Cannot insert into " + lst.type(), pos);

        let index = args.getInt("index").value;
        if (index < 0) index = lst.value.length + index + 1;
        const value = args.get("value");

        return lst.insertAt(index, value);
    }
}

export class FuncInt extends ValueFunc {
    constructor() {
        super("int");
        this.info = "int(obj)\r\n" +
                "\r\n" +
                "Converts the obj to an int, if possible.\r\n" +
                "\r\n" +
                ": int('1') ==> 1\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return args.getAsInt("obj");
    }
}

export class FuncIsEmpty extends ValueFunc {
    constructor() {
        super("is_empty");
        this.info = "is_empty(obj)\r\n" +
                "\r\n" +
                "Returns TRUE, if the obj is empty.\r\n" +
                "Lists, sets and maps are empty, if they do not contain elements.\r\n" +
                "Strings are empty, if the contain no characters. NULL is always empty.\r\n" +
                "\r\n" +
                ": is_empty(NULL) ==> TRUE\r\n" +
                ": is_empty(1) ==> FALSE\r\n" +
                ": is_empty([]) ==> TRUE\r\n" +
                ": is_empty(<<>>) ==> TRUE\r\n" +
                ": is_empty(set([1, 2])) ==> FALSE\r\n" +
                ": is_empty('') ==> TRUE\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        const obj = args.get("obj");
        if (obj.isNull()) return ValueBoolean.TRUE;
        if (obj.isNumerical()) return ValueBoolean.FALSE;
        if (obj.isString()) return ValueBoolean.from(obj.value === "");
        if (obj.isList()) return ValueBoolean.from(obj.value.length === 0);
        if (obj.isSet()) return ValueBoolean.from(obj.value.size === 0);
        if (obj.isMap()) return ValueBoolean.from(obj.value.size === 0);
        if (obj.isObject()) return ValueBoolean.from(obj.value.size === 0);
        return ValueBoolean.FALSE;
    }
}

export class FuncIsNotEmpty extends ValueFunc {
    constructor() {
        super("is_not_empty");
        this.info = "is_not_empty(obj)\r\n" +
                "\r\n" +
                "Returns TRUE, if the obj is not empty.\r\n" +
                "Lists, sets and maps are empty, if they do not contain elements.\r\n" +
                "Strings are empty, if the contain no characters. NULL is always empty.\r\n" +
                "\r\n" +
                ": is_not_empty([]) ==> FALSE\r\n" +
                ": is_not_empty(set([1, 2])) ==> TRUE\r\n" +
                ": is_not_empty('a') ==> TRUE\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        const obj = args.get("obj");
        if (obj.isNull()) return ValueBoolean.FALSE;
        if (obj.isNumerical()) return ValueBoolean.TRUE;
        if (obj.isString()) return ValueBoolean.from(obj.value !== "");
        if (obj.isList()) return ValueBoolean.from(obj.value.length > 0);
        if (obj.isSet()) return ValueBoolean.from(obj.value.size > 0);
        if (obj.isMap()) return ValueBoolean.from(obj.value.size > 0);
        if (obj.isObject()) return ValueBoolean.from(obj.value.size > 0);
        return ValueBoolean.TRUE;
    }
}

export class FuncIsNotNull extends ValueFunc {
    constructor() {
        super("is_not_null");
        this.info = "is_not_null(obj)\r\n" +
                "\r\n" +
                "Returns TRUE, if the obj is not NULL.\r\n" +
                "\r\n" +
                ": is_not_null('') ==> TRUE\r\n" +
                ": is_not_null(1) ==> TRUE\r\n" +
                ": is_not_null(NULL) ==> FALSE\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return ValueBoolean.from(!args.get("obj").isNull());
    }
}

export class FuncIsNull extends ValueFunc {
    constructor() {
        super("is_null");
        this.info = "is_null(obj)\r\n" +
                "\r\n" +
                "Returns TRUE, if the obj is NULL.\r\n" +
                "\r\n" +
                ": is_null('') ==> FALSE\r\n" +
                ": is_null(1) ==> FALSE\r\n" +
                ": is_null(NULL) ==> TRUE\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return ValueBoolean.from(args.get("obj").isNull());
    }
}

export class FuncLambda extends ValueFunc {
    constructor(lexicalEnv) {
        super("lambda");
        this.lexicalEnv = lexicalEnv;
        this.argNames = [];
        this.defValues = [];
        this.body = null;
    }

    getArgNames() {
        return this.argNames;
    }

    setName(name) {
        this.name = name;
    }

    addArg(name) {
        this.argNames.push(name);
        this.defValues.push(null);
    }

    addArg(name, defaultValue) {
        this.argNames.push(name);
        this.defValues.push(defaultValue);
    }

    setBody(body) {
        this.body = body;
    }

    execute(args, environment, pos) {
        const env = this.lexicalEnv.newEnv();
        for (let i = 0; i < this.argNames.length; i++) {
            if (args.hasArg(this.argNames[i])) {
                env.put(this.argNames[i], args.get(this.argNames[i]));
            } else if (this.defValues[i] != null) {
                env.put(this.argNames[i], this.defValues[i].evaluate(env));
            } else {
                throw new RuntimeError("ERROR", "Missing argument " + this.argNames[i], pos);
            }
        }
        const result = this.body.evaluate(env);
        if (result instanceof ValueControlReturn) {
            return result.value;
        } else if (result instanceof ValueControlBreak) {
            throw new RuntimeError("ERROR", "Cannot use break without surrounding loop", result.pos);
        } else if (result instanceof ValueControlContinue) {
            throw new RuntimeError("ERROR", "Cannot use continue without surrounding loop", result.pos);
        }
        return result;
    }
}

export class FuncLength extends ValueFunc {
    constructor() {
        super("length");
        this.info = "length(obj)\r\n" +
                "\r\n" +
                "Returns the length of obj. This only works for strings, lists, sets and maps.\r\n" +
                "\r\n" +
                ": length('123') ==> 3\r\n" +
                ": length([1, 2, 3]) ==> 3\r\n" +
                ": length(<<1, 2, 3>>) ==> 3\r\n" +
                ": <<<'a' => 1, 'b' => 2, 'c' =>3>>> !> length() ==> 3\r\n" +
                ": length(object()) ==> 0\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        const arg = args.get("obj");
        if (arg.isString()) return new ValueInt(arg.value.length);
        if (arg.isList()) return new ValueInt(arg.value.length);
        if (arg.isSet()) return new ValueInt(arg.value.size);
        if (arg.isMap()) return new ValueInt(arg.value.size);
        if (arg.isObject()) return new ValueInt(arg.value.size);
        throw new RuntimeError("Cannot determine length of " + arg.type(), pos);
    }
}

export class FuncLess extends ValueFunc {
    constructor() {
        super("less");
        this.info = "less(obj)\r\n" +
                "\r\n" +
                "Returns TRUE if a is less than b.\r\n" +
                "\r\n" +
                ": less(1, 2) ==> TRUE\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");
        return ValueBoolean.from(a.compareTo(b) < 0);
    }
}

export class FuncLessEquals extends ValueFunc {
    constructor() {
        super("less_equals");
        this.info = "less_equals(a, b)\r\n" +
                "\r\n" +
                "Returns TRUE if a is less than or equals to b.\r\n" +
                "\r\n" +
                ": less_equals(1, 2) ==> TRUE\r\n" +
                ": less_equals(2, 1) ==> FALSE\r\n" +
                ": less_equals(1, 1) ==> TRUE\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");
        return ValueBoolean.from(a.compareTo(b) <= 0);
    }
}

export class FuncList extends ValueFunc {
    constructor() {
        super("list");
        this.info = "list(obj)\r\n" +
                "\r\n" +
                "Converts the obj to a list.\r\n" +
                "\r\n" +
                ": list(1) ==> [1]\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        if (!args.hasArg("obj")) return new ValueList();
        return args.getAsList("obj");
    }
}

export class FuncListDir extends ValueFunc {
    constructor(system) {
        super("list_dir");
        this.info = "list_dir(dir, recursive = FALSE, include_path = FALSE, include_dirs = FALSE)\r\n" +
                "\r\n" +
                "Enumerates the files and directories in the specified directory and\r\n" +
                "returns a list of filename or paths.\r\n";
        this.fs = system.fs;
        this.path = system.path;
        this.secure = false;
    }

    getArgNames() {
        return ["dir", "recursive", "include_path", "include_dirs"];
    }

    execute(args, environment, pos) {
        const directory = args.getString("dir").value;
        let recursive = false;
        if (args.hasArg("recursive")) recursive = args.getBoolean("recursive").value;
        let include_path = recursive;
        if (args.hasArg("include_path")) include_path = args.getBoolean("include_path").value;
        let include_dirs = false;
        if (args.hasArg("include_dirs")) include_dirs = args.getBoolean("include_dirs").value;
        const result = new ValueList();
        this.collectFiles(directory, recursive, include_path, include_dirs, result);
        return result;
    }

    collectFiles(dir, recursive, include_path, include_dirs, result) {
        const files = this.fs.readdirSync(dir, {withFileTypes: true});
        files.forEach(dirent => {
            if (include_dirs || !dirent.isDirectory()) {
                let path = dirent.name;
                if (include_path) path = this.path.join(dir, path);
                result.addItem(new ValueString(path));
            }
            if (recursive && dirent.isDirectory()) {
                this.collectFiles(this.path.join(dir, dirent.name), recursive, include_path, include_dirs, result);
            }
        });
    }
}

export class FuncLog extends ValueFunc {
    constructor() {
        super("log");
        this.info = "log(x)\r\n" +
                "\r\n" +
                "Returns the natural logarithm of x.\r\n" +
                "\r\n" +
                ": int(log(E)) ==> 1\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.log(args.getNumerical("x").value));
    }
}

export class FuncLower extends ValueFunc {
    constructor() {
        super("lower");
        this.info = "lower(str)\r\n" +
                "\r\n" +
                "Converts str to lower case letters.\r\n" +
                "\r\n" +
                ": lower('Hello') ==> 'hello'\r\n";
    }

    getArgNames() {
        return ["str"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueNull.NULL;
        return new ValueString(args.getString("str").value.toLowerCase());
    }
}

export class FuncLs extends ValueFunc {
    constructor() {
        super("ls");
        this.info = "ls()\r\n" +
                    "ls(module)\r\n" +
                    "\r\n" +
                    "Returns a list of all defined symbols (functions and constants) in\r\n" +
                    "the current environment or in the specified module.";
    }

    getArgNames() {
        return ["module"];
    }

    execute(args, environment, pos) {
        const result = new ValueList();
        if (args.hasArg("module")) {
            const moduleArg = args.get("module");
            let module;
            if (moduleArg.isString()) module = environment.get(moduleArg.value, pos).value;
            else module = args.get("module").asObject().value;
            for (const symbol of module.keys()) {
                result.addItem(new ValueString(symbol));
            }
        } else {
            for (const symbol of environment.getSymbols()) {
                result.addItem(new ValueString(symbol));
            }
        }
        return result;
    }
}

export class FuncMakeDir extends ValueFunc {
    constructor(system) {
        super("make_dir");
        this.info = "make_dir(dir, with_parents = FALSE)\r\n" +
                "\r\n" +
                "Creates a new directory.\r\n";
        this.fs = system.fs;
        this.path = system.path;
        this.secure = false;
    }

    getArgNames() {
        return ["dir", "with_parents"];
    }

    execute(args, environment, pos) {
        const dir = args.getString("dir").value;
        let with_parents = false;
        if (args.hasArg("with_parents")) with_parents = args.getBoolean("with_parents").value;
        try {
            if (with_parents) {
                this.fs.mkdirSync(dir, {recursive: true});
            } else {
                this.fs.mkdirSync(dir);
            }
        } catch {
            throw new RuntimeError("ERROR", "Cannot create directory " + dir, pos);
        }
        return ValueNull.NULL;
    }
}

export class FuncMap extends ValueFunc {
    constructor() {
        super("map");
        this.info = "map(obj)\r\n" +
                    "map()\r\n" +
                    "\r\n" +
                    "Converts the obj to a map, if possible. If obj is omitted,\r\n" +
                    "an empty map is returned.\r\n" +
                    "\r\n" +
                    ": map([[1, 2], [3, 4]]) ==> <<<1 => 2, 3 => 4>>>\r\n" +
                    ": map() ==> <<<>>>\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        if (!args.hasArg("obj")) return new ValueMap();
        return args.getAsMap("obj");
    }
}

export class FuncMatches extends ValueFunc {
    constructor() {
        super("matches");
        this.info = "matches(str, pattern)\r\n" +
                "\r\n" +
                "Returns TRUE, if str matches the regular expression pattern.\r\n" +
                "\r\n" +
                ": matches('abc12', //[a-c]+[1-9]+//) ==> TRUE\r\n" +
                ": matches(NULL, //[a-c]+[1-9]+//) ==> FALSE\r\n" ;
    }

    getArgNames() {
        return ["str", "pattern"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueBoolean.FALSE;
        return ValueBoolean.from(args.getString("str").matches(args.getAsPattern("pattern")));
    }
}

export class FuncMod extends ValueFunc {
    constructor() {
        super("mod");
        this.info = "mod(a, b)\r\n" +
                "\r\n" +
                "Returns the modulus of a modulo b.\r\n" +
                "\r\n" +
                ": mod(7, 2) ==> 1\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isInt() && b.isInt()) {
            return new ValueInt(a.value % b.value);
        }

        if (a.isNumerical() && b.isNumerical()) {
            return new ValueDecimal(a.asDecimal().value % b.asDecimal().value);
        }

        throw new RuntimeError("ERROR", "Cannot calculate modulus of " + a.type() + " by " + b.type(), pos);
    }
}

export class FuncMul extends ValueFunc {
    constructor() {
        super("mul");
        this.info = "mul(a, b)\r\n" +
                "\r\n" +
                "Returns the product of a and b. For numerical values this uses the usual arithmetic.\r\n" +
                "If a is a string and b is an int, then the string a is repeated b times. If a is a\r\n" +
                "list and b is an int, then the list is repeated b times.\r\n" +
                "\r\n" +
                ": mul(2, 3) ==> 6\r\n" +
                ": mul('2', 3) ==> '222'\r\n" +
                ": mul([1, 2], 3) ==> [1, 2, 1, 2, 1, 2]\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isString() && b.isInt()) {
            const s = a.value;
            let r = "";
            for (let i = 0; i < b.value; i++) {
                r = r.concat(s);
            }
            return new ValueString(r);
        }

        if (a.isList() && b.isInt()) {
            const result = new ValueList();
            for (let i = 0; i < b.value; i++) {
                result.addItems(a.value);
            }
            return result;
        }

        if (a.isInt() && b.isInt()) {
            return new ValueInt(a.value * b.value);
        }

        if (a.isNumerical() && b.isNumerical()) {
            return new ValueDecimal(a.asDecimal().value * b.asDecimal().value);
        }

        throw new RuntimeError("ERROR", "Cannot multiply " + a.type() + " by " + b.type(), pos);
    }
}

export class FuncNotEquals extends ValueFunc {
    constructor() {
        super("not_equals");
        this.info = "not_equals(a, b)\r\n" +
                "\r\n" +
                "Returns TRUE if a is not equals to b.\r\n" +
                "\r\n" +
                "Integer values are propagated to decimal values, if required.\r\n" +
                "\r\n" +
                ": not_equals(1, 2) ==> TRUE\r\n" +
                ": not_equals(1, 1) ==> FALSE\r\n" +
                ": not_equals(1, 1.0) ==> FALSE\r\n" +
                ": not_equals('a', 'b') ==> TRUE\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");
        return ValueBoolean.from(!a.isEquals(b));
    }
}

export class FuncObject extends ValueFunc {
    constructor() {
        super("object");
        this.info = "object()\r\n" +
                    "object(obj)\r\n" +
                    "\r\n" +
                    "Creates an empty object value or converts a list of pairs or a map to an object.\r\n" +
                    "\r\n" +
                    ": object() ==> <**>\r\n" +
                    ": object(<<<a => 1>>>) ==> <*a=1*>\r\n" +
                    ": object([['a', 1]]) ==> <*a=1*>\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        if (!args.hasArg("obj")) return new ValueObject();
        return args.getAsObject("obj");
    }
}

export class FuncOrd extends ValueFunc {
    constructor() {
        super("ord");
        this.info = "ord(ch)\r\n" +
                "\r\n" +
                "Returns the code point integer of the character ch.\r\n" +
                "\r\n" +
                ": ord('a') ==> 97\r\n" +
                ": ord(' ') ==> 32\r\n";
    }

    getArgNames() {
        return ["ch"];
    }

    execute(args, environment, pos) {
        if (args.isNull("ch")) return ValueNull.NULL;
        return new ValueInt(args.getString("ch").value.charCodeAt(0));
    }
}

export class FuncParse extends ValueFunc {
    constructor() {
        super("parse");
        this.info = "parse(s)\r\n" +
                "\r\n" +
                "Parses the string s.\r\n" +
                "\r\n" +
                ": parse('2+3') ==> '(add 2, 3)'\r\n";
    }

    getArgNames() {
        return ["s"];
    }

    execute(args, environment, pos) {
        try {
            return new ValueNode(Parser.parseScript(args.getString("s").value, pos.filename));
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot parse expression " + args.getString("s"), pos);
        }
    }
}

export class FuncParseDate extends ValueFunc {
    constructor() {
        super("parse_date");
        this.info = "parse_date(str, fmt = 'yyyyMMdd')\r\n" +
                "\r\n" +
                "Parses the string str according to fmt and returns a datetime value.\r\n" +
                "If the format does not match or if the date is invalid, the NULL\r\n" +
                "value is returned.\r\n" +
                "\r\n" +
                "It is possible to pass a list of formats to the fmt parameter.\r\n" +
                "The function sequentially tries to convert the str and if it\r\n" +
                "works, returns the value.\r\n" +
                "\r\n" +
                ": parse_date('20170102') ==> '20170102000000'\r\n" +
                ": parse_date('20170102', fmt = 'yyyyMMdd') ==> '20170102000000'\r\n" +
                ": parse_date('2017010222', fmt = 'yyyyMMdd') ==> NULL\r\n" +
                ": parse_date('20170102', fmt = 'yyyyMMddHH') ==> NULL\r\n" +
                ": parse_date('20170102', fmt = ['yyyyMMdd']) ==> '20170102000000'\r\n" +
                ": parse_date('201701022015', fmt = ['yyyyMMddHHmm', 'yyyyMMddHH', 'yyyyMMdd']) ==> '20170102201500'\r\n" +
                ": parse_date('20170112', fmt = ['yyyyMM', 'yyyy']) ==> NULL\r\n" +
                ": parse_date('20170144') ==> NULL\r\n";
    }

    getArgNames() {
        return ["str", "fmt"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueNull.NULL;
        const x = args.getString("str");
        const fmts = [];
        if (args.hasArg("fmt")) {
            if (args.get("fmt").isList()) {
                for (const fmt_ of args.get("fmt").value) {
                    fmts.push(fmt_.asString().value);
                }
            } else {
                fmts.push(args.get("fmt").asString().value);
            }
        } else {
            fmts.push("yyyyMMdd");
        }

        for (let fmt of fmts) {
            let str = x.value;
            const vals = {};
            for (const part of ["yyyy", "yy", "MM", "dd", "HH", "mm", "ss"]) {
                const idx = fmt.indexOf(part);
                if (idx === -1) continue;
                vals[part] = Number(str.substr(idx, part.length));
                str = str.substring(0, idx) + str.substring(idx + part.length);
                fmt = fmt.substring(0, idx) + fmt.substring(idx + part.length);
                if (str === "") break;
            }
            if (fmt.indexOf("y") === -1 && fmt.indexOf("M") === -1 && fmt.indexOf("d") === -1 &&
                fmt.indexOf("H") === -1 && fmt.indexOf("m") === -1 && fmt.indexOf("s") === -1 &&
                str === '') {
                const date = new Date(1970, 0, 1, 0, 0, 0, 0);
                for (const part in vals) {
                    switch (part) {
                        case "yyyy": date.setFullYear(vals[part]); break;
                        case "yy": date.setFullYear(vals[part] + 2000); break;
                        case "MM": date.setMonth(vals[part] - 1); break;
                        case "dd": date.setDate(vals[part]); break;
                        case "HH": date.setHours(vals[part]); break;
                        case "mm": date.setMinutes(vals[part]); break;
                        case "ss": date.setSeconds(vals[part]); break;
                    }
                }
                let valid_date = true;
                for (const part in vals) {
                    switch (part) {
                        case "MM": if (date.getMonth() + 1 != vals[part]) valid_date = false; break;
                        case "dd": if (date.getDate() != vals[part]) valid_date = false;  break;
                        case "HH": if (date.getHours() != vals[part]) valid_date = false;  break;
                        case "mm": if (date.getMinutes() != vals[part]) valid_date = false;  break;
                        case "ss": if (date.getSeconds() != vals[part]) valid_date = false;  break;
                    }
                }
                if (valid_date) return new ValueDate(date);
            }
        }

        return ValueNull.NULL;
    }
}

export class FuncParseJson extends ValueFunc {
    constructor() {
        super("parse_json");
        this.info = "parse_json(s)\r\n" +
                "\r\n" +
                "Parses the JSON string s and returns a map or list.\r\n" +
                "\r\n" +
                ": parse_json('{\"a\": 12, \"b\": [1, 2, 3, 4]}') ==> '<<<\\\'a\\\' => 12, \\\'b\\\' => [1, 2, 3, 4]>>>'\r\n" +
                ": parse_json('[1, 2.5, 3, 4]') ==> '[1, 2.5, 3, 4]'\r\n";
    }

    getArgNames() {
        return ["s"];
    }

    objAsMap(obj) {
        const result = new ValueMap();
        for (const [key, value] of Object.entries(obj)) {
            result.addItem(this.convertObj(key), this.convertObj(value));
        }
        return result;
    }

    arrayAsList(arr) {
        const result = new ValueList();
        for (const item of arr) {
            result.addItem(this.convertObj(item));
        }
        return result;
    }

    convertObj(obj) {
        if (typeof obj === "string") return new ValueString(obj);
        if (typeof obj === "number") {
            if (Math.trunc(obj) === obj) return new ValueInt(obj);
            else return new ValueDecimal(obj);
        }
        if (typeof obj === "boolean") return ValueBoolean.from(obj);
        if (obj instanceof Array) return this.arrayAsList(obj);
        return this.objAsMap(obj);
    }

    execute(args, environment, pos) {
        try {
            const json = JSON.parse(args.getString("s").value);
            return this.convertObj(json);
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot parse string as JSON", pos);
        }
    }
}

export class FuncPattern extends ValueFunc {
    constructor() {
        super("pattern");
        this.info = "pattern(obj)\r\n" +
                "\r\n" +
                "Converts the obj to a regexp pattern, if possible.\r\n" +
                "\r\n" +
                ": pattern('xy[1-9]{3}') ==> //xy[1-9]{3}//\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return args.getAsPattern("obj");
    }
}

export class FuncPow extends ValueFunc {
    constructor() {
        super("pow");
        this.info = "pow(x, y)\r\n" +
                "\r\n" +
                "Returns the power x ^ y.\r\n" +
                "\r\n" +
                ": pow(2, 3) ==> 8\r\n" +
                ": pow(2.5, 2) ==> 6.25\r\n" +
                ": pow(4, 2) ==> 16\r\n" +
                ": pow(4.0, 2.0) ==> 16.0\r\n" +
                ": round(pow(2, 1.5), digits = 3) ==> 2.828\r\n";
    }

    getArgNames() {
        return ["x", "y"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        if (args.isNull("y")) return ValueNull.NULL;
        if (args.get("y").isInt()) {
            if (args.get("x").isInt()) {
                const x = args.getInt("x").value;
                const y = args.getInt("y").value;
                let result = 1;
                for (let i = 0; i < y; i++) {
                    result *= x;
                }
                return new ValueInt(result);
            } else {
                const x = args.getDecimal("x").value;
                const y = args.getInt("y").value;
                let result = 1.0;
                for (let i = 0; i < y; i++) {
                    result *= x;
                }
                return new ValueDecimal(result);
            }
        } else {
            return new ValueDecimal(Math.pow(args.getNumerical("x").value, args.getNumerical("y").value));
        }
    }
}

export class FuncPrint extends ValueFunc {
    constructor() {
        super("print");
        this.info = "print(obj, out = stdout)\r\n" +
                "\r\n" +
                "Prints the obj to the output out. Default output is stdout which\r\n" +
                "may be connected to the console (e.g. in case of REPL) or a file\r\n" +
                "or be silently ignored.\r\n" +
                "\r\n" +
                ": print('hello') ==> NULL\r\n";
    }

    getArgNames() {
        return ["obj", "out"];
    }

    execute(args, environment, pos) {
        const obj = args.getAsString("obj");
        const output = args.getOutput("out", environment.get("stdout", pos));
        try {
            output.write(obj.value);
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot write to output", pos);
        }
        return ValueNull.NULL;
    }
}

export class FuncPrintln extends ValueFunc {
    constructor() {
        super("println");
        this.info = "println(obj = '', out = stdout)\r\n" +
                "\r\n" +
                "Prints the obj to the output out and terminates the line. Default\r\n" +
                "output is stdout which may be connected to the console (e.g. in\r\n" +
                "case of REPL) or a file or be silently ignored.\r\n" +
                "\r\n" +
                ": println('hello') ==> NULL\r\n";
    }

    getArgNames() {
        return ["obj", "out"];
    }

    execute(args, environment, pos) {
        let obj;
        if (args.hasArg("obj")) {
            obj = args.getAsString("obj");
        } else {
            obj = new ValueString("");
        }
        const output = args.getOutput("out", environment.get("stdout", pos));
        try {
            output.writeLine(obj.value);
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot write to output", pos);
        }
        return ValueNull.NULL;
    }
}

export class FuncProcessLines extends ValueFunc {
    constructor() {
        super("process_lines");
        this.info = "process_lines(input, callback)\r\n" +
                "\r\n" +
                "Reads lines from the input and calls the callback function\r\n" + 
                "once for each line. The line string is the single argument\r\n" +
                "of the callback function.\r\n" +
                "\r\n" +
                "If input is a list, then each list element is converted to\r\n" + 
                "a string and processed as a line\r\n" +
                "\r\n" +
                "The function returns the number of processed lines." +
                "\r\n" +
                ": def result = []; str_input('one\\ntwo\\nthree') !> process_lines(fn(line) result !> append(line)); result ==> ['one', 'two', 'three']\r\n" +
                ": str_input('one\\ntwo\\nthree') !> process_lines(fn(line) line) ==> 3\r\n" +
                ": def result = ''; process_lines(['a', 'b', 'c'], fn(line) result += line); result ==> 'abc'\r\n";
    }

    getArgNames() {
        return ["input", "callback"];
    }

    execute(args, environment, pos) {
        const inparg = args.get("input");
        const callback = args.get("callback").asFunc();
        const env = environment.newEnv();
        if (inparg instanceof ValueInput) {
            const input = inparg.asInput();
            return new ValueInt(input.process(line => {
                const args = new Args(pos).addArg(callback.getArgNames()[0], line);
                callback.execute(args, env, pos);
            }));
        } else if (inparg instanceof ValueList) {
            const list = inparg.asList().value;
            for (const element of list) {
                const args = new Args(pos).addArg(callback.getArgNames()[0], element.asString());
                callback.execute(args, env, pos);
            };
            return new ValueInt(list.length);
        } else {
            throw new RuntimeError("ERROR", "Cannot process lines from " + inparg.toString(), pos);
        }
    }
}

export class FuncPut extends ValueFunc {
    constructor() {
        super("put");
        this.info = "put(m, key, value)\r\n" +
                "\r\n" +
                "Puts the value into the map m at the given key.\r\n" +
                "\r\n" +
                ": def m = map([[1, 2], [3, 4]]); put(m, 1, 9) ==> <<<1 => 9, 3 => 4>>>\r\n";
    }

    getArgNames() {
        return ["m", "key", "value"];
    }

    execute(args, environment, pos) {
        const m =  args.getMap("m");
        m.addItem(args.get("key"), args.get("value"));
        return m;
    }
}

export class FuncRandom extends ValueFunc {
    constructor() {
        super("random");
        this.info = "random()\r\n" +
                "random(a)\r\n" +
                "random(a, b)\r\n" +
                "\r\n" +
                "Returns a random number. If no argument is provided, a decimal\r\n" +
                "value in the range [0, 1) is returned. If only a is provided, then \r\n" +
                "an int value in the range [0, a) is returned. If both a and b are\r\n" +
                "provided, then an int value in the range [a, b) is returned.\r\n" +
                "\r\n" +
                ": set_seed(1); random(5) ==> 1\r\n";
    }

    static seed = Math.random();

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        if (args.hasArg("a") && !args.hasArg("b")) {
            return new ValueInt(this.getRandomInt(0, args.getInt("a").value));
        }

        if (args.hasArg("a") && args.hasArg("b")) {
            return new ValueInt(this.getRandomInt(args.getInt("a").value, args.getInt("b").value));
        }

        return new ValueDecimal(this.getRandomDouble());
    }

    getRandomInt(min, max) {
        min = Math.ceil(min);
        max = Math.floor(max);
        return Math.floor(this.seededRandom() * (max - min)) + min;
    }

    getRandomDouble() {
        return this.seededRandom();
    }

    seededRandom() {
        FuncRandom.seed = (FuncRandom.seed * 9301 + 49297) % 233280;
        return FuncRandom.seed / 233280;
    }    
}

export class FuncRange extends ValueFunc {
    constructor() {
        super("range");
        this.info = "range(a)\r\n" +
                "range(a, b)\r\n" +
                "range(a, b, step)\r\n" +
                "\r\n" +
                "Returns a list containing int values in the range. If only a is\r\n" +
                "provided, the range is [0, a). If both a and b are provided, the\r\n" +
                "range is [a, b). If step is given, then only every step element\r\n" +
                "is included in the list.\r\n" +
                "\r\n" +
                ": range(4) ==> [0, 1, 2, 3]\r\n" +
                ": range(3, 6) ==> [3, 4, 5]\r\n" +
                ": range(10, step = 3) ==> [0, 3, 6, 9]\r\n" +
                ": range(10, 0, step = -2) ==> [10, 8, 6, 4, 2]\r\n";
    }

    getArgNames() {
        return ["a", "b", "step"];
    }

    execute(args, environment, pos) {
        let start = 0;
        let end = 0;
        let step = 1;
        if (args.hasArg("a") && !args.hasArg("b")) {
            end = args.getInt("a").value;
        } else if (args.hasArg("a") && args.hasArg("b")) {
            start = args.getInt("a").value;
            end = args.getInt("b").value;
        }
        if (args.hasArg("step")) {
            step = args.getInt("step").value;
        }

        const result = new ValueList();
        let i = start;
        if (step > 0) {
            while (i < end) {
                result.addItem(new ValueInt(i));
                i += step;
            }
        } else if (step < 0) {
            while (i > end) {
                result.addItem(new ValueInt(i));
                i += step;
            }
        }
        return result;
    }
}

export class FuncRead extends ValueFunc {
    constructor() {
        super("read");
        this.info = "read(input = stdin)\r\n" +
                "\r\n" +
                "Read a character from the input. If end of input is reached, an empty string is returned.\r\n" +
                "\r\n" +
                ": def s = str_input('hello'); read(s) ==> 'h'\r\n";
    }

    getArgNames() {
        return ["input"];
    }

    execute(args, environment, pos) {
        const input = args.getInput("input", environment.get("stdin", pos));
        try {
            const str = input.read();
            if (str == null) return ValueNull.NULL;
            return new ValueString(str);
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot read from input", pos);
        }
    }
}

export class FuncReadall extends ValueFunc {
    constructor() {
        super("read_all");
        this.info = "read_all(input = stdin)\r\n" +
                "\r\n" +
                "Read the whole input. If end of input is reached, NULL is returned.\r\n" +
                "\r\n" +
                ": def s = str_input('hello'); read_all(s) ==> 'hello'\r\n";
    }

    getArgNames() {
        return ["input"];
    }

    execute(args, environment, pos) {
        const input = args.getInput("input", environment.get("stdin", pos));
        try {
            const str = input.readAll();
            if (str == null) return ValueNull.NULL;
            return new ValueString(str);
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot read from input", pos);
        }
    }
}

export class FuncReadln extends ValueFunc {
    constructor() {
        super("readln");
        this.info = "readln(input = stdin)\r\n" +
                "\r\n" +
                "Read one line from the input. If end of input is reached, NULL is returned.\r\n" +
                "\r\n" +
                ": def s = str_input('hello'); readln(s) ==> 'hello'\r\n";
    }

    getArgNames() {
        return ["input"];
    }

    execute(args, environment, pos) {
        const input = args.getInput("input", environment.get("stdin", pos));
        try {
            const line = input.readLine();
            if (line == null) return ValueNull.NULL;
            return new ValueString(line);
        } catch (e) {
            throw new RuntimeError("ERROR", "Cannot read from input", pos);
        }
    }
}

export class FuncRemove extends ValueFunc {
    constructor() {
        super("remove");
        this.info = "remove(lst, element)\r\n" +
                "\r\n" +
                "Removes the element from the list lst. The lst may also be a set or a map.\r\n" +
                "Returns the changed list, but the list is changed in place.\r\n" +
                "\r\n" +
                ": remove([1, 2, 3, 4], 3) ==> [1, 2, 4]\r\n" +
                ": remove(<<1, 2, 3, 4>>, 3) ==> <<1, 2, 4>>\r\n" +
                ": remove(<<<a => 1, b => 2, c => 3, d => 4>>>, 'c') ==> <<<'a' => 1, 'b' => 2, 'd' => 4>>>\r\n" +
                ": remove(<*a=1, b=2*>, 'b') ==> <*a=1*>\r\n";
    }

    getArgNames() {
        return ["lst", "element"];
    }

    execute(args, environment, pos) {
        const lst = args.get("lst");
        const element = args.get("element");

        if (lst.isList() || lst.isSet() || lst.isMap()) {
            lst.removeItem(element);
            return lst;
        } else if (lst.isObject()) {
            lst.value.delete(element.value);
            return lst;
        }

        throw new RuntimeError("ERROR", "Cannot remove from " + lst.type(), pos);
    }
}

export class FuncRound extends ValueFunc {
    constructor() {
        super("round");
        this.info = "round(x, digits = 0)\r\n" +
                "\r\n" +
                "Returns the decimal value x rounded to the specified number of digits.\r\n" +
                "Default for digits is 0.\r\n" +
                "\r\n" +
                ": round(1.345, digits = 1) ==> 1.3\r\n";
    }

    getArgNames() {
        return ["x", "digits"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;

        const x = args.getNumerical("x");
        let digits = 0;
        if (args.hasArg("digits")) {
            digits = args.getInt("digits").value;
        }

        let factor = 1.0;
        for (let i = 0; i < digits; i++) {
            factor *= 10;
        }

        return new ValueDecimal(Math.round(x.asDecimal().value * factor) / factor);
    }
}

export class FuncRun extends ValueFunc {
    constructor(interpreter, fs) {
        super("run");
        this.interpreter = interpreter;
        this.fs = fs;
        this.info = "run(file)\r\n" +
                "\r\n" +
                "Loads and interprets the file.\r\n";
        this.secure = false;
    }

    getArgNames() {
        return ["file"];
    }

    execute(args, environment, pos) {
        const file = args.getString("file").value;
        let path = file;
        if (!path.startsWith("/") && !path.startsWith(".")) path = process.cwd() + "/" + path;
        let script = "";
        try {
            script = this.fs.readFileSync(path, {encoding: 'utf8', flag: 'r'});
        } catch (e) {
            throw new RuntimeError("ERROR", "File " + path + " not found", pos);
        }
        return this.interpreter.interpret(script, file);
    }
}

export class FuncS extends ValueFunc {
    constructor() {
        super("s");
        this.info = "s(str, start = 0)\r\n" +
                "\r\n" +
                "Returns a string, where all placeholders are replaced with their\r\n" +
                "appropriate values. Placeholder have the form '{var}' and result\r\n" +
                "in the value of the variable var inserted at this location.\r\n" +
                "\r\n" +
                "The placeholder can also be expressions and their result will\r\n" +
                "be inserted instead of the placeholder.\r\n" +
                "\r\n" +
                "There are formatting suffixes to the placeholder, which allow\r\n" +
                "some control over the formatting. They formatting spec starts after\r\n" +
                "a # character and consists of align/fill, width and precision fields.\r\n" +
                "For example #06.2 will format the decimal to a width of six characters\r\n" +
                "and uses two digits after the decimal point. If the number is less than\r\n" +
                "six characters wide, then it is prefixed with zeroes until the width\r\n" +
                "is reached, e.g. '001.23'. Please refer to the examples below.\r\n" +
                "\r\n" +
                ": def name = 'damian'; s('hello {name}') ==> 'hello damian'\r\n" +
                ": def foo = '{bar}'; def bar = 'baz'; s('{foo}{bar}') ==> '{bar}baz'\r\n" +
                ": def a = 'abc'; s('\"{a#-8}\"') ==> '\"abc     \"'\r\n" +
                ": def a = 'abc'; s('\"{a#8}\"') ==> '\"     abc\"'\r\n" +
                ": def a = 'abc'; s('a = {a#5}') ==> 'a =   abc'\r\n" +
                ": def a = 'abc'; s('a = {a#-5}') ==> 'a = abc  '\r\n" +
                ": def n = 12; s('n = {n#5}') ==> 'n =    12'\r\n" +
                ": def n = 12; s('n = {n#-5}') ==> 'n = 12   '\r\n" +
                ": def n = 12; s('n = {n#05}') ==> 'n = 00012'\r\n" +
                ": def n = 1.2345678; s('n = {n#.2}') ==> 'n = 1.23'\r\n" +
                ": def n = 1.2345678; s('n = {n#06.2}') ==> 'n = 001.23'\r\n" +
                ": s('2x3 = {2*3}') ==> '2x3 = 6'\r\n" +
                ": def n = 123; s('n = {n#x}') ==> 'n = 7b'\r\n" +
                ": def n = 255; s('n = {n#04x}') ==> 'n = 00ff'\r\n" +
                ": require Math; s('{Math->PI} is cool') ==> '3.141592653589793 is cool'\r\n";
    }

    getArgNames() {
        return ["str", "start"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueNull.NULL;
        let str = args.getString("str").value;
        let start = args.getInt("start", 0).value;
        if (start < 0) start = str.length + start;
        while (true) {
            const idx1 = str.indexOf('{', start);
            if (idx1 == -1) return new ValueString(str);
            const idx2 = str.indexOf('}', idx1 + 1);
            if (idx2 == -1) return new ValueString(str);
            let variable = str.substring(idx1 + 1, idx2);
            let width = 0;
            let zeroes = false;
            let leading = true;
            let digits = -1;
            let base = 10;
            const idx3 = variable.indexOf('#');
            if (idx3 != -1) {
                let spec = variable.substring(idx3 + 1);
                variable = variable.substring(0, idx3);
                if (spec.startsWith('-')) {
                    leading = false;
                    spec = spec.substring(1);
                }
                if (spec.startsWith('0')) {
                    zeroes = true;
                    leading = false;
                    spec = spec.substring(1);
                }
                if (spec.endsWith('x')) {
                    base = 16;
                    spec = spec.substring(0, spec.length - 1);
                }
                const idx4 = spec.indexOf('.');
                if (idx4 == -1) {
                    digits = -1;
                    width = Number(spec);
                } else {
                    digits = Number(spec.substring(idx4 + 1));
                    width = Number(spec.substring(0, idx4));
                }
            }
            const node = Parser.parseScript(variable, pos.filename);
            let value = node.evaluate(environment).asString().value;
            if (base != 10) value = Number(value).toString(base);
            else if (digits !== -1) value = Number(value).toFixed(digits);
            while (value.length < width) {
                if (leading) value = ' ' + value;
                else if (zeroes) value = '0' + value;
                else value = value + ' ';
            }
            str = str.substring(0, idx1) + value + str.substring(idx2 + 1);
            start = idx1 + value.length;
        }
    }
}

export class FuncSet extends ValueFunc {
    constructor() {
        super("set");
        this.info = "set(obj)\r\n" +
                "\r\n" +
                "Converts the obj to a set, if possible.\r\n" +
                "\r\n" +
                ": set([1, 2, 3]) ==> <<1, 2, 3>>\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        if (!args.hasArg("obj")) return new ValueSet();
        return args.getAsSet("obj");
    }
}

export class FuncSetSeed extends ValueFunc {
    constructor() {
        super("set_seed");
        this.info = "set_seed(n)\r\n" +
                "\r\n" +
                "Sets the seed of the random number generator to n.\r\n" +
                "\r\n" +
                ": set_seed(1) ==> 1\r\n";
    }

    getArgNames() {
        return ["n"];
    }

    execute(args, environment, pos) {
        const seed = args.getInt("n").value;
        FuncRandom.seed = seed;
        return new ValueInt(seed);
    }
}

export class FuncSin extends ValueFunc {
    constructor() {
        super("sin");
        this.info = "sin(x)\r\n" +
                "\r\n" +
                "Returns the sinus of x.\r\n" +
                "\r\n" +
                ": sin(0) ==> 0.0\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.sin(args.getNumerical("x").value));
    }
}

export class FuncSorted extends ValueFunc {
    constructor() {
        super("sorted");
        this.info = "sorted(lst, cmp=compare, key=identity)\r\n" +
                "\r\n" +
                "Returns a sorted copy of the list. This is sorted according to the\r\n" +
                "value returned by the key function for each element of the list.\r\n" +
                "The values are compared using the compare function cmp.\r\n" +
                "\r\n" +
                ": sorted([3, 2, 1]) ==> [1, 2, 3]\r\n" +
                ": sorted([6, 2, 5, 3, 1, 4]) ==> [1, 2, 3, 4, 5, 6]\r\n";
    }

    getArgNames() {
        return ["lst", "cmp", "key"];
    }

    execute(args, environment, pos) {
        const env = environment.newEnv();
        const lst = args.getAsList("lst");
        const cmp = args.hasArg("cmp") ? args.getFunc("cmp") : environment.get("compare", pos);
        const key = args.hasArg("key") ? args.getFunc("key") : environment.get("identity", pos);
        const result = [...lst.value];
        for (let i = 1; i < result.length; i++) {
            const v = key.execute(new Args(pos).addArg(key.getArgNames()[0], result[i]), env, pos);
            for (let j = i - 1; j >= 0; j--) {
                const v2 = key.execute(new Args(pos).addArg(key.getArgNames()[0], result[j]), env, pos);
                const cmpargs = new Args(pos).addArg(cmp.getArgNames()[0], v).addArg(cmp.getArgNames()[1], v2);
                const comparison = cmp.execute(cmpargs, env, pos).value;
                if (comparison < 0) {
                    const temp = result[j + 1];
                    result[j+ 1] = result[j];
                    result[j] = temp;
                } else {
                    break;
                }
            }
        }
        return new ValueList().addItems(result);
    }
}

export class FuncSplit extends ValueFunc {
    constructor() {
        super("split");
        this.info = "split(str, delim = '[ \\t]+')\r\n" +
                "\r\n" +
                "Splits the string str into parts and returns a list of strings.\r\n" +
                "The delim is a regular expression. Default is spaces or tabs.\r\n" +
                "\r\n" +
                ": split('a,b,c', //,//) ==> ['a', 'b', 'c']\r\n";
    }

    getArgNames() {
        return ["str", "delim"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueNull.NULL;

        const value = args.getString("str").value;
        const delim = args.getAsPattern("delim", new ValuePattern("[ \\t]+")).pattern;

        return FuncSplit.splitValue(value, delim);
    }

    static splitValue(value, delim) {
        if (value == "") return new ValueList();

        const result = new ValueList();
        const parts = value.split(delim);
        for (const part of parts) {
            result.addItem(new ValueString(part));
        }
        return result;
    }
}

export class FuncSplit2 extends ValueFunc {
    constructor() {
        super("split2");
        this.info = "split2(str, sep1, sep2)\r\n" +
                "\r\n" +
                "Performs a two-stage split of the string data.\r\n" +
                "This results in a list of list of strings.\r\n" +
                "\r\n" +
                ": split2('a:b:c|d:e:f', escape_pattern('|'), escape_pattern(':')) ==> [['a', 'b', 'c'], ['d', 'e', 'f']]\r\n" +
                ": split2('', //\\|//, //://) ==> []\r\n";
    }

    getArgNames() {
        return ["str", "sep1", "sep2"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueNull.NULL;

        const value = args.getString("str").value;
        const sep1 = args.getAsPattern("sep1").pattern;
        const sep2 = args.getAsPattern("sep2").pattern;
        const result = FuncSplit.splitValue(value, sep1);
        const list = result.value;
        for (let i = 0; i < list.length; i++) {
            list[i] = FuncSplit.splitValue(list[i].value, sep2);
        }
        return result;
    }
}

export class FuncSqrt extends ValueFunc {
    constructor() {
        super("sqrt");
        this.info = "sqrt(x)\r\n" +
                "\r\n" +
                "Returns the square root of num as a decimal value.\r\n" +
                "\r\n" +
                ": sqrt(4) ==> 2.0\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.sqrt(args.getNumerical("x").value));
    }
}

export class FuncStartsWith extends ValueFunc {
    constructor() {
        super("starts_with");
        this.info = "starts_with(str, part)\r\n" +
                "\r\n" +
                "Returns TRUE if the string str starts with part.\r\n" +
                "\r\n" +
                ": starts_with('abcdef', 'abc') ==> TRUE\r\n" +
                ": starts_with(NULL, 'abc') ==> FALSE\r\n";
    }

    getArgNames() {
        return ["str", "part"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueBoolean.FALSE;
        return ValueBoolean.from(args.getString("str").value.startsWith(args.getString("part").value));
    }
}

export class FuncString extends ValueFunc {
    constructor() {
        super("string");
        this.info = "string(obj)\r\n" +
                "\r\n" +
                "Converts the obj to a string, if possible.\r\n" +
                "\r\n" +
                ": string(123) ==> '123'\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return args.getAsString("obj");
    }
}

export class FuncStrInput extends ValueFunc {
    constructor() {
        super("str_input");
        this.info = "str_input(str)\r\n" +
                "\r\n" +
                "Returns an input object, that reads the characters of the given string str.\r\n" +
                "\r\n" +
                ": str_input('abc') ==> <!input-stream>\r\n";
    }

    getArgNames() {
        return ["str"];
    }

    execute(args, environment, pos) {
        return new ValueInput(new StringInput(args.getString("str").value));
    }
}

export class FuncStrOutput extends ValueFunc {
    constructor() {
        super("str_output");
        this.info = "str_output()\r\n" +
                "\r\n" +
                "Returns an output object. Things written to this output object can be retrieved using the function get_output_string.\r\n" +
                "\r\n" +
                ": do def o = str_output(); print('abc', out = o); get_output_string(o); end ==> 'abc'\r\n";
    }

    getArgNames() {
        return [];
    }

    execute(args, environment, pos) {
        return new ValueOutput(new StringOutput());
    }
}

export class FuncSub extends ValueFunc {
    constructor() {
        super("sub");
        this.info = "sub(a, b)\r\n" +
                "\r\n" +
                "Returns the subtraction of b from a. For numerical values this uses usual arithmetic.\r\n" +
                "For lists and sets, this returns lists and sets minus the element b. If a is a datetime\r\n" +
                "value and b is datetime value, then the date difference is returned. If a is a datetime\r\n" +
                "value and b is a numeric value, then b is interpreted as number of days and the corresponding\r\n" +
                "datetime after subtracting these number of days is returned.\r\n" +
                "\r\n" +
                ": sub(1, 2) ==> -1\r\n" +
                ": sub([1, 2, 3], 2) ==> [1, 3]\r\n" +
                ": sub(date('20170405'), date('20170402')) ==> 3\r\n" +
                ": sub(date('20170405'), 3) ==> '20170402000000'\r\n" +
                ": sub(<<3, 1, 2>>, 2) ==> <<1, 3>>";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");

        if (a.isList()) {
            const result = new ValueList();
            for (const item of a.value) {
                let add = true;
                for (const val of args.getAsList("b").value) {
                    if (!item.isEquals(val)) continue;
                    add = false;
                    break;
                }
                if (add) result.addItem(item);
            }
            return result;
        }

        if (a.isSet()) {
            let minus = new ValueSet();
            if (b.isSet()) {
                minus = b.asSet();
            } else if (b.isList()) {
                for (const element of b.asList().value) {
                    minus.addItem(element);
                }
            } else {
                minus.addItem(b);
            }
            const result = new ValueSet();
            for (const element of a.asSet().value.values()) {
                if (!minus.hasItem(element)) result.addItem(element);
            }
            return result;
        }

        if (a.isDate()) {
            if (b.isDate()) {
                const diff = a.asInt().value - b.asInt().value;
                return new ValueInt(diff);
            }
            return new ValueDate(convertOADateToDate(convertDateToOADate(a.value) - args.getAsDecimal("b").value));
        }

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isInt() && b.isInt()) {
            return new ValueInt(a.value - b.value);
        }

        if (a.isNumerical() && b.isNumerical()) {
            return new ValueDecimal(a.asDecimal().value - b.asDecimal().value);
        }

        throw new RuntimeError("ERROR", "Cannot subtract " + b.type() + " from " + a.type(), pos);
    }
}

export class FuncSublist extends ValueFunc {
    constructor() {
        super("sublist");
        this.info = "sublist(lst, startidx)\r\n" +
                "sublist(lst, startidx, endidx)\r\n" +
                "\r\n" +
                "Returns the sublist starting with startidx. If endidx is provided,\r\n" +
                "this marks the end of the sublist. Endidx is not included.\r\n" +
                "\r\n" +
                ": sublist([1, 2, 3, 4], 2) ==> [3, 4]\r\n";
    }

    getArgNames() {
        return ["lst", "startidx", "endidx"];
    }

    execute(args, environment, pos) {
        if (args.isNull("lst")) return ValueNull.NULL;
        const value = args.getList("lst").value;
        let start = args.getInt("startidx").value;
        if (start < 0) start = value.length + start;
        if (start > value.length) return new ValueList();
        let end = args.getInt("endidx", value.length).value;
        if (end < 0) end = value.length + end;
        if (end > value.length) end = value.length;
        const result = new ValueList();
        for (let i = start; i < end; i++) {
            result.addItem(value[i]);
        }
        return result;
    }
}

export class FuncSubstr extends ValueFunc {
    constructor() {
        super("substr");
        this.info = "substr(str, startidx)\r\n" +
                "substr(str, startidx, endidx)\r\n" +
                "\r\n" +
                "Returns the substring starting with startidx. If endidx is provided,\r\n" +
                "this marks the end of the substring. Endidx is not included.\r\n" +
                "\r\n" +
                ": substr('abcd', 2) ==> 'cd'\r\n";
    }

    getArgNames() {
        return ["str", "startidx", "endidx"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueNull.NULL;
        const value = args.getString("str").value;
        let start = args.getInt("startidx").value;
        if (start < 0) start = value.length + start;
        if (start > value.length) return new ValueString("");
        let end = args.getInt("endidx", value.length).value;
        if (end < 0) end = value.length + end;
        if (end > value.length) end = value.length;
        return new ValueString(value.substring(start, end));
    }
}

export class FuncSum extends ValueFunc {
    constructor() {
        super("sum");
        this.info = "sum(list, ignore = [])\r\n" +
                "\r\n" +
                "Returns the sum of a list of numbers. Values contained in the optional list ignore\r\n" +
                "are counted as 0.\r\n" +
                "\r\n" +
                ": sum([1, 2, 3]) ==> 6\r\n" +
                ": sum([1, 2.5, 3]) ==> 6.5\r\n" +
                ": sum([1, 2.5, 1.5, 3]) ==> 8.0\r\n" +
                ": sum([1.0, 2.0, 3.0]) ==> 6.0\r\n" +
                ": sum([1.0, 2, -3.0]) ==> 0.0\r\n" +
                ": sum([1, 2, -3]) ==> 0\r\n" +
                ": sum([1, '1', 1], ignore = ['1']) ==> 2\r\n" +
                ": sum(range(101)) ==> 5050\r\n" +
                ": sum([]) ==> 0\r\n" +
                ": sum([NULL], ignore = [NULL]) ==> 0\r\n" +
                ": sum([1, NULL, 3], ignore = [NULL]) ==> 4\r\n" +
                ": sum([1, NULL, '', 3], ignore = [NULL, '']) ==> 4\r\n";
    }

    getArgNames() {
        return ["list", "ignore"];
    }

    execute(args, environment, pos) {
        if (args.isNull("list")) return ValueNull.NULL;

        const list = args.getList("list").value;

        let ignore = [];
        if (args.hasArg("ignore")) {
            ignore = args.getList("ignore").value;
        }

        let result = 0;
        let decimalrequired = false;

        for (const value of list) {
            let skipvalue = false;
            for (const ignoreval of ignore) {
                if (ignoreval.isEquals(value)) {
                    skipvalue = true;
                    break;
                }
            }
            if (skipvalue) continue;

            if (value.isInt()) {
                result += value.value;
            } else if (value.isDecimal()) {
                result += value.value;
                decimalrequired = true;
            } else {
                throw new RuntimeError("ERROR", "Cannot sum " + value.type(), pos);
            }
        }

        if (decimalrequired) {
            return new ValueDecimal(result);
        }
        return new ValueInt(result);
    }
}

export class FuncTan extends ValueFunc {
    constructor() {
        super("tan");
        this.info = "tan(x)\r\n" +
                "\r\n" +
                "Returns the tangens of x.\r\n" +
                "\r\n" +
                ": tan(0) ==> 0\r\n";
    }

    getArgNames() {
        return ["x"];
    }

    execute(args, environment, pos) {
        if (args.isNull("x")) return ValueNull.NULL;
        return new ValueDecimal(Math.tan(args.getNumerical("x").value));
    }
}

export class FuncTimestamp extends ValueFunc {
    constructor() {
        super("timestamp");
        this.info = "timestamp(x)\r\n" +
                "\r\n" +
                "Returns current system timestamp.\r\n";
    }

    getArgNames() {
        return [];
    }

    execute(args, environment, pos) {
        return new ValueInt(new Date().getTime());
    }
}

export class FuncTrim extends ValueFunc {
    constructor() {
        super("trim");
        this.info = "trim(str)\r\n" +
                "\r\n" +
                "Trims any leading or trailing whitespace from the string str.\r\n" +
                "\r\n" +
                ": trim(' a  ') ==> 'a'\r\n";
    }

    getArgNames() {
        return ["str"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueNull.NULL;
        return new ValueString(args.getString("str").value.trim());
    }
}

export class FuncType extends ValueFunc {
    constructor() {
        super("type");
        this.info = "type(obj)\r\n" +
                "\r\n" +
                "Returns the name of the type of obj as a string.\r\n" +
                "\r\n" +
                ": type('Hello') ==> 'string'\r\n";
    }

    getArgNames() {
        return ["obj"];
    }

    execute(args, environment, pos) {
        return new ValueString(args.get("obj").type());
    }

}

export class FuncUpper extends ValueFunc {
    constructor() {
        super("upper");
        this.info = "upper(str)\r\n" +
                "\r\n" +
                "Converts str to upper case letters.\r\n" +
                "\r\n" +
                ": upper('Hello') ==> 'HELLO'\r\n";
    }

    getArgNames() {
        return ["str"];
    }

    execute(args, environment, pos) {
        if (args.isNull("str")) return ValueNull.NULL;
        return new ValueString(args.getString("str").value.toUpperCase());
    }
}

export class FuncZip extends ValueFunc {
    constructor() {
        super("zip");
        this.info = "zip(a, b)\r\n" +
                "\r\n" +
                "Returns a list where each element is a list of two items.\r\n" +
                "The first of the two items is taken from the first list,\r\n" +
                "the second from the second list. The resulting list has\r\n" +
                "the same length as the shorter of the two input lists.\r\n" +
                "\r\n" +
                ": zip([1, 2, 3], [4, 5, 6, 7]) ==> [[1, 4], [2, 5], [3, 6]]\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isList() && b.isList()) {
            const lista = a.value;
            const listb = b.value;
            const result = new ValueList();
            for (let i = 0; i < Math.min(lista.length, listb.length); i++) {
                const pair = new ValueList();
                pair.addItem(lista[i]);
                pair.addItem(listb[i]);
                result.addItem(pair);
            }
            return result;
        }

        throw new RuntimeError("ERROR", "Cannot zip " + a.type() + " and " + b.type(), pos);
    }
}

export class FuncZipMap extends ValueFunc {
    constructor() {
        super("zip_map");
        this.info = "zip_map(a, b)\r\n" +
                "\r\n" +
                "Returns a map where the key of each entry is taken from a,\r\n" +
                "and where the value of each entry is taken from b, where\r\n" +
                "a and b are lists of identical length.\r\n" +
                "\r\n" +
                ": zip_map(['a', 'b', 'c'], [1, 2, 3]) ==> <<<'a' => 1, 'b' => 2, 'c' => 3>>>\r\n";
    }

    getArgNames() {
        return ["a", "b"];
    }

    execute(args, environment, pos) {
        const a = args.get("a");
        const b = args.get("b");

        if (a.isNull() || b.isNull()) {
            return ValueNull.NULL;
        }

        if (a.isList() && b.isList()) {
            const lista = a.value;
            const listb = b.value;
            const result = new ValueMap();
            for (let i = 0; i < Math.min(lista.length, listb.length); i++) {
                result.addItem(lista[i], listb[i]);
            }
            return result;
        }

        throw new RuntimeError("ERROR", "Cannot zip_map " + a.type() + " and " + b.type(), pos);
    }
}
