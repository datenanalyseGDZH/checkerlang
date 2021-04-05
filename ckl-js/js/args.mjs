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
import { ValueBoolean } from "./values.mjs";

import { 
    ValueDecimal,
    ValueInt,
    ValueList,
    ValueString 
} from "./values.mjs";

export class Args {
    constructor(pos) {
        this.argNames = [];
        this.args = new Map();
        this.restArgName = null;
        this.pos = pos;
    }

    addArg(name, value) {
        this.argNames.push(name);
        this.args.set(name, value);
        return this;
    }

    addArgs(names) {
        for (const name of names) {
            if (!name.endsWith("...")) {
                this.argNames.push(name);
            } else {
                this.restArgName = name;
            }
        }
        return this;
    }

    toString() {
        let result = "";
        for(let [name, value] of this.args) {
            result = result.concat(name, "=", value, ", ");
        }
        if (result.length > 0) result = result.substr(0, result.length - 2);
        return result;
    }

    toStringAbbrev() {
        let result = "";
        for(let [name, value] of this.args) {
            value = value.toString();
            if (value.length > 50) value = value.substr(0, 50) + "... " + value.substr(value.length - 5, 5);
            result = result.concat(name, "=", value, ", ");
        }
        if (result.length > 0) result = result.substr(0, result.length - 2);
        return result;
    }

    setArgs(names, values) {
        const rest = new ValueList();
        for (let i = 0; i < values.length; i++) {
            if (names[i] !== null) {
                if (!this.argNames.includes(names[i])) throw new RuntimeError("Argument " + names[i] + " is unknown", this.pos);
                this.args.set(names[i], values[i]);
            }
        }

        let inKeywords = false;
        for (let i = 0; i < values.length; i++) {
            if (names[i] === null) {
                if (inKeywords) throw new RuntimeError("Positional arguments need to be placed before named arguments", this.pos);
                const argName = this.getNextPositionalArgName();
                if (argName === null) {
                    if (this.restArgName === null) throw new RuntimeError("Too many arguments", this.pos);
                    rest.addItem(values[i]);
                } else if (!this.args.has(argName)) {
                    this.args.set(argName, values[i]);
                } else {
                    rest.addItem(values[i]);
                }
            } else {
                inKeywords = true;
                if (!this.argNames.includes(names[i])) throw new RuntimeError("Argument " + names[i] + " is unknown", this.pos);
                this.args.set(names[i], values[i]);
            }
        }

        if (this.restArgName != null) {
            this.args.set(this.restArgName, rest);
        }
    }

    getNextPositionalArgName() {
        for (const argname of this.argNames)
        {
            if (!this.args.has(argname)) return argname;
        }
        return null;
    }

    hasArg(name) {
        return this.args.has(name);
    }

    get(name) {
        if (!this.hasArg(name)) throw new RuntimeError("Missing argument " + name, this.pos);
        return this.args.get(name);
    }

    isNull(name) {
        if (!this.hasArg(name)) return false;
        return this.get(name).isNull();
    }

    getString(name, defaultValue = null) {
        if (!this.hasArg(name) && defaultValue !== null) return new ValueString(defaultValue);
        const value = this.get(name);
        if (!value.isString()) throw new RuntimeError("String required but got " + value.type(), this.pos);
        return value;
    }

    getBoolean(name, defaultValue = null) {
        if (!this.hasArg(name) && defaultValue !== null) return ValueBoolean.from(defaultValue);
        const value = this.get(name);
        if (!value.isBoolean()) throw new RuntimeError("Boolean required but got " + value.type(), this.pos);
        return value;
    }

    getInt(name, defaultValue = null) {
        if (!this.hasArg(name) && defaultValue !== null) return new ValueInt(defaultValue);
        const value = this.get(name);
        if (!value.isInt()) throw new RuntimeError("Int required but got " + value.type(), this.pos);
        return value;
    }

    getDecimal(name, defaultValue = null) {
        if (!this.hasArg(name) && defaultValue !== null) return new ValueDecimal(defaultValue);
        const value = this.get(name);
        if (!value.isDecimal()) throw new RuntimeError("Decimal required but got " + value.type(), this.pos);
        return value;
    }

    getNumerical(name, defaultValue = null) {
        if (!this.hasArg(name) && defaultValue !== null) return new ValueDecimal(defaultValue);
        const value = this.get(name);
        if (!value.isNumerical()) throw new RuntimeError("Numerical required but got " + value.type(), this.pos);
        return value;
    }

    getList(name) {
        const value = this.get(name);
        if (!value.isList()) throw new RuntimeError("List required but got " + value.type(), this.pos);
        return value;
    }

    getMap(name) {
        const value = this.get(name);
        if (!value.isMap()) throw new RuntimeError("Map required but got " + value.type(), this.pos);
        return value;
    }

    getInput(name, defaultValue = null) {
        if (!this.hasArg(name) && defaultValue !== null) return defaultValue;
        const value = this.get(name);
        if (!value.isInput()) throw new RuntimeError("Input required but got " + value.type(), this.pos);
        return value;
    }

    getOutput(name, defaultValue = null) {
        if (!this.hasArg(name) && defaultValue !== null) return defaultValue;
        const value = this.get(name);
        if (!value.isOutput()) throw new RuntimeError("Output required but got " + value.type(), this.pos);
        return value;
    }

    getFunc(name) {
        const value = this.get(name);
        if (!value.isFunc()) throw new RuntimeError("Func required but got " + value.type(), this.pos);
        return value;
    }

    getDate(name) {
        const value = this.get(name);
        if (!value.isDate()) throw new RuntimeError("Date required but got " + value.type(), this.pos);
        return value;
    }

    getAsBoolean(name) {
        const value = this.get(name);
        try {
            return value.asBoolean();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsNode(name) {
        const value = this.get(name);
        try {
            return value.asNode();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsDate(name) {
        const value = this.get(name);
        try {
            return value.asDate();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsString(name) {
        const value = this.get(name);
        try {
            return value.asString();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsPattern(name, defaultValue = null) {
        if (!this.hasArg(name) && defaultValue !== null) return defaultValue;
        const value = this.get(name);
        try {
            return value.asPattern();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsList(name) {
        const value = this.get(name);
        try {
            return value.asList();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsSet(name) {
        const value = this.get(name);
        try {
            return value.asSet();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsObject(name) {
        const value = this.get(name);
        try {
            return value.asObject();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsMap(name) {
        const value = this.get(name);
        try {
            return value.asMap();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsInt(name) {
        const value = this.get(name);
        try {
            return value.asInt();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }

    getAsDecimal(name) {
        const value = this.get(name);
        try {
            return value.asDecimal();
        } catch (e) {
            e.pos = this.pos;
            throw e;
        }
    }
}
