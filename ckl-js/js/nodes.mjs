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
import { FuncLambda } from "./functions.mjs";
import { Parser } from "./parser.mjs";
import { Args } from "./args.mjs";
import { moduleloader } from "./moduleloader.mjs";

import { 
    ValueBoolean,
    ValueControlBreak,
    ValueControlContinue,
    ValueControlReturn,
    ValueInput,
    ValueList,
    ValueMap,
    ValueNull,
    ValueObject,
    ValueSet,
    ValueString 
} from "./values.mjs";

function getFuncallString(fn, args) {
    return fn.name + "(" + args.toStringAbbrev() + ")";
}

export function invoke(fn, names_, args, environment, pos) {
    const values = [];
    const names = [];
    for (let i = 0; i < args.length; i++) {
        const arg = args[i];
        if (arg instanceof NodeSpread) {
            const argvalue = arg.evaluate(environment);
            if (argvalue instanceof ValueMap) {
                const map = argvalue;
                for (const [key, value] of map.value.entries()) {
                    values.push(value);
                    if (key instanceof ValueString) {
                        names.push(key.value);
                    } else {
                        names.push(null);
                    }
                }
            } else {
                const list = argvalue;
                for (const value of list.value) {
                    values.push(value);
                    names.push(null);
                }
            }
        } else {
            values.push(arg.evaluate(environment));
            names.push(names_[i]);
        }
    }

    const args_ = new Args(pos);
    args_.addArgs(fn.getArgNames());
    args_.setArgs(names, values);

    try {
        return fn.execute(args_, environment, pos);
    } catch (e) {
        if (!("stacktrace" in e)) e.stacktrace = [];
        e.stacktrace.push(getFuncallString(fn, args_) + " " + pos.toString());
        throw e;
    }
};

export class NodeAnd {
    constructor(expression, pos) {
        this.expressions = expression === null ? [] : [expression];
        this.pos = pos;
    }

    addAndClause(expression) { 
        this.expressions.push(expression); 
        return this;
    }

    getSimplified() {
        if (this.expressions.length == 1) return this.expressions[0];
        return this;
    }

    evaluate(environment) {
        for (let expression of this.expressions) {
            const value = expression.evaluate(environment);
            if (!value.isBoolean()) throw RuntimeError("Expected boolean but got " + value.type(), this.pos);
            if (!value.value) {
                return ValueBoolean.FALSE;
            }
        }
        return ValueBoolean.TRUE;
    }

    toString() {
        let result = "(";
        for (let expression of this.expressions) {
            result = result.concat(expression.toString(), " and ");
        }
        if (result.length > 1) result = result.substr(0, result.length - " and ".length);
        result = result.concat(")");
        return result;
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        for (let expression of this.expressions) {
            expression.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }
}

export class NodeAssign {
    constructor(identifier, expression, pos) {
        if (identifier.startsWith("checkerlang_")) throw new SyntaxError("Cannot assign to system variable " + identifier, this.pos);
        this.identifier = identifier;
        this.expression = expression;
        this.pos = pos;
    }

    evaluate(environment) {
        if (!environment.isDefined(this.identifier)) throw new RuntimeError("Variable " + this.identifier + " is not defined", this.pos);
        environment.set(this.identifier, this.expression.evaluate(environment));
        return environment.get(this.identifier, this.pos);
    }

    toString() {
        return "(" + this.identifier + " = " + this.expression + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
    }
}

export class NodeAssignDestructuring {
    constructor(identifiers, expression, pos) {
        for (const identifier of identifiers) {
            if (identifier.startsWith("checkerlang_")) throw new SyntaxError("Cannot assign to system variable " + identifier, this.pos);
        }
        this.identifiers = identifiers;
        this.expression = expression;
        this.pos = pos;
    }

    evaluate(environment) {
        let values = this.expression.evaluate(environment);
        if (values.isList()) values = values.value;
        else if (values.isSet()) values = values.value.sortedValues();
        else throw new RuntimeError("Destructuring assign expects list or set but got " + values.type(), this.pos);
        let result = ValueNull.NULL;
        for (let i = 0; i < this.identifiers.length; i++) {
            const identifier = this.identifiers[i];
            let value = ValueNull.NULL;
            if (i < values.length) value = values[i];
            if (!environment.isDefined(identifier)) throw new RuntimeError("Variable " + identifier + " is not defined", this.pos);
            environment.set(identifier, value);
            result = value;
        }
        return result;
    }

    toString() {
        return "([" + this.identifiers + "] = " + this.expression + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
    }
}

export class NodeBlock {
    constructor(pos, toplevel = false) {
        this.expressions = [];
        this.finallyexprs = [];
        this.pos = pos;
        this.toplevel = toplevel;
    }

    add(expression) {
        this.expressions.push(expression);
    }

    addFinally(expression) { 
        this.finallyexprs.push(expression); 
    }

    hasFinally() { 
        return this.finallyexprs.length > 0; 
    }

    evaluate(environment) {
        let result = ValueBoolean.TRUE;
        try {
            for (let expression of this.expressions) {
                result = expression.evaluate(environment);
                if (result.isReturn()) break;
                if (result.isBreak()) break;
                if (result.isContinue()) break;
            }
        } catch (e) {
            for (let expression of this.finallyexprs) {
                expression.evaluate(environment);
            }
            throw e;
        }
        for (let expression of this.finallyexprs) {
            expression.evaluate(environment);
        }
        return result;
    }

    toString() {
        let result = "(block ";
        for (let expression of this.expressions) {
            result = result.concat(expression.toString(), ", ");
        }
        if (this.expressions.length > 0) result = result.substr(0, result.length - 2);
        if (this.finallyexprs.length > 0) {
            result = result.concat(" finally ");
            for (let expression of this.finallyexprs) {
                result = result.concat(expression.toString(), ", ");
            }
        }
        if (this.finallyexprs.length > 0) result = result.substr(0, result.length - 2);
        return result.concat(")");
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        let additionalBoundVarsLocal = [...additionalBoundVars];
        for (let expression of this.expressions) {
            if (expression instanceof NodeDef) {
                if (!additionalBoundVarsLocal.includes(expression.identifier)) {
                    additionalBoundVarsLocal.push(expression.identifier);
                }
            }
            if (expression instanceof NodeDefDestructuring) {
                for (const identifier of expression.identifiers) {
                    if (!additionalBoundVarsLocal.includes(identifier)) {
                        additionalBoundVarsLocal.push(identifier);
                    }
                }
            }
        }
        for (let expression of this.finallyexprs) {
            if (expression instanceof NodeDef) {
                if (!additionalBoundVarsLocal.includes(expression.identifier)) {
                    additionalBoundVarsLocal.push(expression.identifier);
                }
            }
            if (expression instanceof NodeDefDestructuring) {
                for (const identifier of expression.identifiers) {
                    if (!additionalBoundVarsLocal.includes(identifier)) {
                        additionalBoundVarsLocal.push(identifier);
                    }
                }
            }
        }
        for (let expression of this.expressions) {
            if (expression instanceof NodeDef || expression instanceof NodeDefDestructuring) {
                expression.collectVars(freeVars, boundVars, additionalBoundVarsLocal);
            } else {
                expression.collectVars(freeVars, boundVars, additionalBoundVars);
            }
        }
        for (let expression of this.finallyexprs) {
            if (expression instanceof NodeDef || expression instanceof NodeDefDestructuring) {
                expression.collectVars(freeVars, boundVars, additionalBoundVarsLocal);
            } else {
                expression.collectVars(freeVars, boundVars, additionalBoundVars);
            }
        }
    }
}

export class NodeBreak {
    constructor(pos) {
        this.pos = pos;
    }

    evaluate(environment) {
        return new ValueControlBreak(this.pos);
    }

    toString() {
        return "(break)";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        // empty
    }
}

export class NodeContinue {
    constructor(pos) {
        this.pos = pos;
    }

    evaluate(environment) {
        return new ValueControlContinue(this.pos);
    }

    toString() {
        return "(continue)";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        // empty
    }
}

export class NodeDef {
    constructor(identifier, expression, info, pos) {
        this.identifier = identifier;
        this.expression = expression;
        this.info = info;
        this.pos = pos;
    }

    evaluate(environment) {
        let value = this.expression.evaluate(environment);
        value.info = this.info;
        environment.put(this.identifier, value);
        if (value instanceof FuncLambda) value.name = this.identifier;
        return value;
    }

    toString() {
        return "(def " + this.identifier + " = " + this.expression + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
        if (!boundVars.includes(this.identifier)) {
            boundVars.push(this.identifier);
        }
    }
}

export class NodeDefDestructuring {
    constructor(identifiers, expression, info, pos) {
        this.identifiers = identifiers;
        this.expression = expression;
        this.info = info;
        this.pos = pos;
    }

    evaluate(environment) {
        let value = this.expression.evaluate(environment);
        value.info = this.info;
        if (!value.isList() && !value.isSet()) throw new RuntimeError("Desctructuring def expects list or set but got " + value.type(), this.pos);
        let values = null;
        if (value.isList()) values = value.value;
        if (value.isSet()) values = value.value.sortedValues();
        let result = ValueNull.NULL;
        for (let i = 0; i < this.identifiers.length; i++) {
            if (i < values.length) {
                environment.put(this.identifiers[i], values[i]);
                if (values[i] instanceof FuncLambda) values[i].name = this.identifiers[i];
                result = values[i];
            } else {
                environment.put(this.identifiers[i], ValueNull.NULL);
                result = ValueNull.NULL;
            }
        }
        return result;
    }

    toString() {
        return "(def [" + this.identifiers + "] = " + this.expression + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
        for (const identifier of this.identifiers) {
            if (!boundVars.includes(identifier)) {
                boundVars.push(identifier);
            }
        }
    }
}

export class NodeDeref {
    constructor(expression, index, pos) {
        this.expression = expression;
        this.index = index;
        this.pos = pos;
    }

    evaluate(environment) {
        const idx = this.index.evaluate(environment);
        const value = this.expression.evaluate(environment);
        if (value == ValueNull.NULL) return ValueNull.NULL;
        if (value instanceof ValueString) {
            const s = value.value;
            let i = Number(idx.value);
            if (i < 0) i = i + s.length;
            if (i < 0 || i >= s.length) throw new RuntimeError("Index out of bounds " + i, this.pos);
            return new ValueString(s.charAt(i));
        }
        if (value instanceof ValueList) {
            const list = value.value;
            let i = Number(idx.value);
            if (i < 0) i = i + list.length;
            if (i < 0 || i >= list.length) throw new RuntimeError("Index out of bounds " + i, this.pos);
            return list[i];
        }
        if (value instanceof ValueMap) {
            if (!value.hasItem(idx)) throw new RuntimeError("Map does not contain key " + idx, this.pos);
            return value.getItem(idx);
        }
        if (value instanceof ValueObject) {
            const member = idx.asString().value;
            if (!value.hasItem(member)) return ValueNull.NULL;
            return value.getItem(member);
        }
        throw new RuntimeError("Cannot dereference value " + value, this.pos);
    }

    toString() {
        return this.expression + "[" + this.index + "]";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
        this.index.collectVars(freeVars, boundVars, additionalBoundVars);
    }
}

export class NodeDerefAssign {
    constructor(expression, index, value, pos) {
        this.expression = expression;
        this.value = value;
        this.index = index;
        this.pos = pos;
    }

    evaluate(environment) {
        const idx = this.index.evaluate(environment);
        const container = this.expression.evaluate(environment);
        const value = this.value.evaluate(environment);
        if (container instanceof ValueString) {
            const s = container.value;
            const i = Number(idx.value);
            if (i < 0) i = i + s.length;
            if (i < 0 || i >= s.length) throw new RuntimeError("Index out of bounds " + i, this.pos);
            return new ValueString(s.substring(0, i) + value.value + s.substring(i + 1));
        }
        if (container instanceof ValueList) {
            const list = container.value;
            const i = Number(idx.value);
            if (i < 0) i = i + list.length;
            if (i < 0 || i >= list.length) throw new RuntimeError("Index out of bounds " + i, this.pos);
            list[i] = value;
            return container;
        }
        if (container instanceof ValueMap) {
            const map = container.value;
            map.set(idx, value);
            return container;
        }
        if (container instanceof ValueObject) {
            const member = idx.asString().value;
            container.value.set(member, value);
            return container;
        }
        throw new RuntimeError("Cannot deref-assign " + this.value.type(), this.pos);
    }

    toString() {
        return "(" + this.expression + "[" + this.index + "] = " + this.value + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
        this.index.collectVars(freeVars, boundVars, additionalBoundVars);
        this.value.collectVars(freeVars, boundVars, additionalBoundVars);
    }
}

export class NodeDerefInvoke {
    constructor(objectExpr, member, pos) {
        this.objectExpr = objectExpr;
        this.member = member;
        this.names = [];
        this.args = [];
        this.pos = pos;
    }

    addArg(name, arg) {
        this.names.push(name);
        this.args.push(arg);
    }

    evaluate(environment) {
        const object = this.objectExpr.evaluate(environment);
        if (object.isObject()) {
            if (!object.hasItem(this.member)) throw new RuntimeError("Member " + this.member + " not found", this.pos);
            const fn = object.getItem(this.member);
            if (!fn.isFunc()) throw new RuntimeError("Member " + this.member + " is not a function", this.pos);
            let names;
            let args;
            if (object.isModule) {
                names = this.names;
                args = this.args;
            } else {
                names = [null].concat(this.names);
                args = [new NodeLiteral(object, this.pos)].concat(this.args);
            }
            return invoke(fn, names, args, environment, this.pos);
        }
        if (object instanceof ValueMap) {
            const map = object.value;
            const fn = map.get(new ValueString(this.member));
            if (!fn.isFunc()) throw new RuntimeError(this.member + " is not a function", this.pos);
            return invoke(fn, this.names, this.args, environment, this.pos);
        }
        throw new RuntimeError("Cannot deref-invoke " + object.type(), this.pos);
    }

    toString() {
        let result = "";
        for (let i = 0; i < this.args.length; i++) {
            result = result.concat(this.names[i], "=", this.args[i], ", ");
        }
        if (result.length > 1) result = result.substr(0, result.length - 2);
        return "(" + this.objectExpr + "->" + this.member + "(" + result + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.objectExpr.collectVars(freeVars, boundVars, additionalBoundVars);
        for (const arg of this.args) {
            arg.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }
}

export class NodeError {
    constructor(expression, pos) {
        this.expression = expression;
        this.pos = pos;
    }

    evaluate(environment) {
        throw new RuntimeError(this.expression.evaluate(environment), this.pos);
    }

    toString() {
        return "(error " + this.expression + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
    }
}

export class NodeFor {
    constructor(identifiers, expression, block, what, pos) {
        this.identifiers = identifiers;
        this.expression = expression;
        this.block = block;
        this.pos = pos;
        this.what = what;
    }

    evaluate(environment) {
        let list = this.expression.evaluate(environment);
        if (list instanceof ValueInput) {
            const input = list;
            let result = ValueBoolean.TRUE;
            let line = null;
            try {
                line = input.readLine();
                while (line != null) {
                    const value = new ValueString(line);
                    if (this.identifiers.length == 1) {
                        environment.put(this.identifiers[0], value);
                    } else {
                        let vals;
                        if (value.isList()) vals = value.value;
                        else if (value.isSet()) vals = value.value.sortedValues();
                        for (let i = 0; i < this.identifiers.length; i++) {
                            environment.put(this.identifiers[i], vals[i]);
                        }
                    }
                    result = this.block.evaluate(environment);
                    if (result instanceof ValueControlBreak) {
                        result = ValueBoolean.TRUE;
                        break;
                    } else if (result instanceof ValueControlContinue) {
                        result = ValueBoolean.TRUE;
                        // continue
                    } else if (result instanceof ValueControlReturn) {
                        break;
                    }
                    line = input.readLine();
                }
                if (this.identifiers.length == 1) {
                    environment.remove(this.identifiers[0]);
                } else {
                    for (let i = 0; i < this.identifiers.length; i++) {
                        environment.remove(this.identifiers[i]);
                    }
                }
            } catch (e) {
                throw new RuntimeError("Cannot read from input", this.pos);
            }
            return result;
        }
        if (list instanceof ValueList) {
            const values = list.value;
            let result = ValueBoolean.TRUE;
            for (const value of values) {
                if (this.identifiers.length == 1) {
                    environment.put(this.identifiers[0], value);
                } else {
                    let vals;
                    if (value.isList()) vals = value.value;
                    else if (value.isSet()) vals = value.value.sortedValues();
                    for (let i = 0; i < this.identifiers.length; i++) {
                        environment.put(this.identifiers[i], vals[i]);
                    }
                }
                result = this.block.evaluate(environment);
                if (result instanceof ValueControlBreak) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result instanceof ValueControlContinue) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result instanceof ValueControlReturn) {
                    break;
                }
            }
            if (this.identifiers.length == 1) {
                environment.remove(this.identifiers[0]);
            } else {
                for (let i = 0; i < this.identifiers.length; i++) {
                    environment.remove(this.identifiers[i]);
                }
            }
            return result;
        }
        if (list instanceof ValueSet) {
            const values = list.value.sortedValues();
            let result = ValueBoolean.TRUE;
            for (const value of values) {
                if (this.identifiers.length == 1) {
                    environment.put(this.identifiers[0], value);
                } else {
                    let vals;
                    if (value.isList()) vals = value.value;
                    else if (value.isSet()) vals = value.value.sortedValues();
                    for (let i = 0; i < this.identifiers.length; i++) {
                        environment.put(this.identifiers[i], vals[i]);
                    }
                }
                result = this.block.evaluate(environment);
                if (result instanceof ValueControlBreak) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result instanceof ValueControlContinue) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result instanceof ValueControlReturn) {
                    break;
                }
            }
            if (this.identifiers.length == 1) {
                environment.remove(this.identifiers[0]);
            } else {
                for (let i = 0; i < this.identifiers.length; i++) {
                    environment.remove(this.identifiers[i]);
                }
            }
            return result;
        }
        if (list instanceof ValueMap) {
            const values = list.value.sortedEntries();
            let result = ValueBoolean.TRUE;
            for (const [key, value] of values) {
                let val = value;
                if (this.what === "keys") val = key;
                else if (this.what === "values") val = value;
                else if (this.what === "entries") {
                    val = new ValueList();
                    val.addItem(key);
                    val.addItem(value);
                }
                if (this.identifiers.length == 1) {
                    environment.put(this.identifiers[0], val);
                } else {
                    let vals;
                    if (val.isList()) vals = val.value;
                    else if (val.isSet()) vals = val.value.sortedValues();
                    for (let i = 0; i < this.identifiers.length; i++) {
                        environment.put(this.identifiers[i], vals[i]);
                    }
                }
                result = this.block.evaluate(environment);
                if (result instanceof ValueControlBreak) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result instanceof ValueControlContinue) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result instanceof ValueControlReturn) {
                    break;
                }
            }
            if (this.identifiers.length == 1) {
                environment.remove(this.identifiers[0]);
            } else {
                for (let i = 0; i < this.identifiers.length; i++) {
                    environment.remove(this.identifiers[i]);
                }
            }
            return result;
        }
        if (list instanceof ValueObject) {
            const values = list.value;
            let result = ValueBoolean.TRUE;
            for (const [key, value] of values) {
                let val = value;
                if (this.what === "keys") val = new ValueString(key);
                else if (this.what === "values") val = value;
                else if (this.what === "entries") {
                    val = new ValueList();
                    val.addItem(new ValueString(key));
                    val.addItem(value);
                }
                if (this.identifiers.length == 1) {
                    environment.put(this.identifiers[0], val);
                } else {
                    let vals;
                    if (val.isList()) vals = val.value;
                    else if (val.isSet()) vals = val.value.sortedValues();
                    for (let i = 0; i < this.identifiers.length; i++) {
                        environment.put(this.identifiers[i], vals[i]);
                    }
                }
                result = this.block.evaluate(environment);
                if (result instanceof ValueControlBreak) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result instanceof ValueControlContinue) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result instanceof ValueControlReturn) {
                    break;
                }
            }
            if (this.identifiers.length == 1) {
                environment.remove(this.identifiers[0]);
            } else {
                for (let i = 0; i < this.identifiers.length; i++) {
                    environment.remove(this.identifiers[i]);
                }
            }
            return result;
        }
        if (list instanceof ValueString) {
            const str = list.value;
            let result = ValueBoolean.TRUE;
            for (let i = 0; i < str.length; i++) {
                environment.put(this.identifiers[0], new ValueString(str.substring(i, i + 1)));
                result = this.block.evaluate(environment);
                if (result instanceof ValueControlBreak) {
                    result = ValueBoolean.TRUE;
                    break;
                } else if (result instanceof ValueControlContinue) {
                    result = ValueBoolean.TRUE;
                    // continue
                } else if (result instanceof ValueControlReturn) {
                    break;
                }
                environment.remove(this.identifiers[0]);
            }
            return result;
        }
        throw new RuntimeError("Cannot iterate over " + list.type(), this.pos);
    }

    toString() {
        return "(for " + (this.identifiers.length == 1 ? this.identifiers[0] : "[" + this.identifiers + "]") + " in " + this.what + " " + this.expression + " do " + this.block + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
        const boundVarsLocal = [...boundVars, ...this.identifiers];
        this.block.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }
}

export class NodeFuncall {
    constructor(func, pos) {
        this.func = func;
        this.names = [];
        this.args = [];
        this.pos = pos;
    }

    addArg(name, arg) {
        this.names.push(name);
        this.args.push(arg);
    }

    evaluate(environment) {
        const fn = this.func.evaluate(environment);
        if (!fn.isFunc()) throw new RuntimeError("Expected function but got " + fn.type(), this.pos);
        return invoke(fn, this.names, this.args, environment, this.pos);
    }

    toString() {
        let result = "";
        for (const expression of this.args) {
            result = result.concat(expression, ", ");
        }
        if (result.length > 1) result = result.substr(0, result.length - 2);
        return "(" + this.func + " " + result + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.func.collectVars(freeVars, boundVars, additionalBoundVars);
        for (const arg of this.args) {
            arg.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }
}

export class NodeIdentifier {
    constructor(value, pos) {
        this.value = value;
        this.pos = pos;
    }

    evaluate(environment) {
        return environment.get(this.value, this.pos);
    }

    toString() {
        return this.value.toString();
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        if (!boundVars.includes(this.value) && !additionalBoundVars.includes(this.value)) {
            if (!freeVars.includes(this.value)) {
                freeVars.push(this.value);
            }
        }
    }
}

export class NodeIf {
    constructor(pos) {
        this.conditions = [];
        this.expressions = [];
        this.elseExpression = new NodeLiteral(ValueBoolean.TRUE, pos);
        this.pos = pos;
    }

    addIf(condition, expression) {
        this.conditions.push(condition);
        this.expressions.push(expression);
    }

    setElse(expression) {
        this.elseExpression = expression;
    }

    evaluate(environment) {
        for (let i = 0; i < this.conditions.length; i++) {
            const value = this.conditions[i].evaluate(environment);
            if (!(value instanceof ValueBoolean)) throw new RuntimeError("Expected boolean condition value but got " + value.type(), this.pos);
            if (value.isTrue()) {
                return this.expressions[i].evaluate(environment);
            }
        }
        return this.elseExpression.evaluate(environment);
    }

    toString() {
        let result = "(";
        for (let i = 0; i < this.conditions.length; i++) {
            result = result.concat("if ", this.conditions[i], ": ", this.expressions[i], " ");
        }
        return result.concat("else: ", this.elseExpression, ")");
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        for (const expression of this.conditions) {
            expression.collectVars(freeVars, boundVars, additionalBoundVars);
        }
        for (const expression of this.expressions) {
            expression.collectVars(freeVars, boundVars, additionalBoundVars);
        }
        this.elseExpression.collectVars(freeVars, boundVars, additionalBoundVars);
    }
}

export class NodeIn {
    constructor(expression, list, pos) {
        this.expression = expression;
        this.list = list;
        this.pos = pos;
    }

    evaluate(environment) {
        const value = this.expression.evaluate(environment);
        const container = this.list.evaluate(environment);
        if (container instanceof ValueList) {
            for (const item of container.value) {
                if (value.isEquals(item)) return ValueBoolean.TRUE;
            }
        } else if (container instanceof ValueSet) {
            return ValueBoolean.from(container.hasItem(value));
        } else if (container instanceof ValueMap) {
            return ValueBoolean.from(container.hasItem(value));
        } else if (container instanceof ValueObject) {
            return ValueBoolean.from(container.hasItem(value.value));
        } else if (container instanceof ValueString) {
            return ValueBoolean.from(container.value.indexOf(value.value) != -1);
        }
        return ValueBoolean.FALSE;
    }

    toString() {
        return "(" + this.expression + " in " + this.list + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
        this.list.collectVars(freeVars, boundVars, additionalBoundVars);
    }
}

export class NodeLambda {
    constructor(pos) {
        this.args = [];
        this.defs = [];
        this.pos = pos;
    }

    addArg(arg, defaultValue = null) {
        this.args.push(arg);
        this.defs.push(defaultValue);
    }

    setBody(body) {
        if (body instanceof NodeBlock) {
            const block = body;
            const expressions = block.expressions;
            if (expressions.length > 0) {
                const lastexpr = expressions[expressions.length - 1];
                if (lastexpr instanceof NodeReturn) {
                    expressions[expressions.length - 1] = lastexpr.expression;
                }
            }
        } else if (body instanceof NodeReturn) {
            body = body.expression;
        }
        this.body = body;
    }

    evaluate(environment) {
        const result = new FuncLambda(environment);
        for (let i = 0; i < this.args.length; i++) {
            result.addArg(this.args[i], this.defs[i]);
        }
        result.setBody(this.body);
        return result;
    }

    toString() {
        let result = "(lambda ";
        for (let i = 0; i < this.args.length; i++) {
            result = result.concat(this.args[i]);
            if (this.defs[i] !== null) {
                result = result.concat("=", this.defs[i]);
            }
            result = result.concat(", ");
        }
        return result.concat(this.body, ")");
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        for (const def of this.defs) {
            if (def !== null) def.collectVars(freeVars, boundVars, additionalBoundVars);
        }
        const boundVarsLocal = [...boundVars];
        for (const arg of this.args) {
            boundVarsLocal.push(arg);
        }
        this.body.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }
}

export class NodeList {
    constructor(pos) {
        this.items = [];
        this.pos = pos;
    }

    addItem(item) {
        this.items.push(item);
    }

    evaluate(environment) {
        const result = new ValueList();
        for (const item of this.items) {
            if (item instanceof NodeSpread) {
                const list = item.evaluate(environment);
                for (const value of list.value) {
                    result.addItem(value);
                }
            } else {
                result.addItem(item.evaluate(environment));
            }
        }
        return result;
    }

    toString() {
        let result = "[";
        for (const expression of this.items) {
            result = result.concat(expression.toString(), ", ");
        }
        if (result.length > 1) result = result.substr(0, result.length - 2);
        return result.concat("]");
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        for (const item of this.items) {
            item.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }
}

export class NodeListComprehension {
    constructor(valueExpr, identifier, listExpr, pos) {
        this.valueExpr = valueExpr;
        this.identifier = identifier;
        this.listExpr = listExpr;
        this.conditionExpr = null;
        this.pos = pos;
    }

    setCondition(conditionExpr) {
        this.conditionExpr = conditionExpr;
    }

    evaluate(environment) {
        const result = new ValueList();
        const localEnv = environment.newEnv();
        const list = this.listExpr.evaluate(environment);
        let values = null;
        if (list.isList()) values = list.value;
        else if (list.isSet()) values = list.value.sortedValues();
        else if (list.isMap()) values = list.value.sortedEntries();
        else if (list.isObject()) values = list.keys();
        else if (list.isString()) values = list.value.split("");
        for (const listValue of values) {
            localEnv.put(this.identifier, listValue);
            const value = this.valueExpr.evaluate(localEnv);
            if (this.conditionExpr != null) {
                const condition = this.conditionExpr.evaluate(localEnv);
                if (!(condition instanceof ValueBoolean)) {
                    throw new RuntimeError("Condition must be boolean but got " + condition.type(), this.pos);
                }
                if (condition.value) {
                    result.addItem(value);
                }
            } else {
                result.addItem(value);
            }
        }
        return result;
    }

    toString() {
        return "[" + this.valueExpr + " for " + this.identifier + " in " + this.listExpr + (this.conditionExpr == null ? "" : (" if " + this.conditionExpr)) + "]";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        const boundVarsLocal = [...boundVars];
        boundVarsLocal.push(this.identifier);
        this.valueExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
        this.listExpr.collectVars(freeVars, boundVars, additionalBoundVars);
        if (this.conditionExpr != null) this.conditionExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }
}

export class NodeLiteral {
    constructor(value, pos) {
        this.value = value;
        this.pos = pos;
    }

    evaluate(environment) {
        return this.value;
    }

    toString() {
        return this.value.toString();
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        // empty
    }
}

export class NodeMap {
    constructor(pos) {
        this.keys = [];
        this.values = [];
        this.pos = pos;
    }

    addKeyValue(key, value) {
        this.keys.push(key);
        this.values.push(value);
    }

    evaluate(environment) {
        const result = new ValueMap();
        for (let i = 0; i < this.keys.length; i++) {
            result.addItem(this.keys[i].evaluate(environment), this.values[i].evaluate(environment));
        }
        return result;
    }

    toString() {
        let result = "<<<";
        for (let i = 0; i < this.keys.length; i++) {
            result = result.concat(this.keys[i], " => ", this.values[i], ", ");
        }
        if (result.length > 3) result = result.substr(0, result.length - 2);
        return result.concat(">>>");
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        for (const item of this.keys) {
            item.collectVars(freeVars, boundVars, additionalBoundVars);
        }
        for (const item of this.values) {
            item.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }
}

export class NodeMapComprehension {
    constructor(keyExpr, valueExpr, identifier, listExpr, pos) {
        this.keyExpr = keyExpr;
        this.valueExpr = valueExpr;
        this.identifier = identifier;
        this.listExpr = listExpr;
        this.conditionExpr = null;
        this.pos = pos;
    }

    setCondition(conditionExpr) {
        this.conditionExpr = conditionExpr;
    }

    evaluate(environment) {
        const result = new ValueMap();
        const localEnv = environment.newEnv();
        const list = this.listExpr.evaluate(environment);
        let values = null;
        if (list.isList()) values = list.value;
        else if (list.isSet()) values = list.value.sortedValues();
        else if (list.isMap()) values = list.value.sortedEntries();
        else if (list.isString()) values = list.value.split("");
        for (const listValue of values) {
            localEnv.put(this.identifier, listValue);
            const key = this.keyExpr.evaluate(localEnv);
            const value = this.valueExpr.evaluate(localEnv);
            if (this.conditionExpr != null) {
                const condition = this.conditionExpr.evaluate(localEnv);
                if (!(condition instanceof ValueBoolean)) {
                    throw new RuntimeError("Condition must be boolean but got " + condition.type(), this.pos);
                }
                if (condition.value) {
                    result.addItem(key, value);
                }
            } else {
                result.addItem(key, value);
            }
        }
        return result;
    }

    toString() {
        return "<<<" + this.keyExpr + " => " + this.valueExpr + " for " + this.identifier + " in " + this.listExpr + (this.conditionExpr == null ? "" : (" if " + this.conditionExpr)) + ">>>";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        const boundVarsLocal = [...boundVars];
        boundVarsLocal.push(this.identifier);
        this.keyExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars)
        this.valueExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
        this.listExpr.collectVars(freeVars, boundVars, additionalBoundVars);
        if (this.conditionExpr != null) this.conditionExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }
}

export class NodeNot {
    constructor(expression, pos) {
        this.expression = expression;
        this.pos = pos;
    }

    evaluate(environment) {
        const value = this.expression.evaluate(environment);
        if (!value.isBoolean()) throw new RuntimeError("Expected boolean but got " + value.type(), this.pos);
        return value.value ? ValueBoolean.FALSE : ValueBoolean.TRUE;
    }

    toString() {
        return "(not " + this.expression + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
    }
}

export class NodeNull {
    constructor(pos) {
        this.pos = pos;
    }

    evaluate(environment) {
        return ValueNull.NULL;
    }

    toString() {
        return "NULL";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        // empty
    }
}

export class NodeObject {
    constructor(pos) {
        this.keys = [];
        this.values = [];
        this.pos = pos;
    }

    addKeyValue(key, value) {
        this.keys.push(key);
        this.values.push(value);
    }

    evaluate(environment) {
        const result = new ValueObject();
        for (let i = 0; i < this.keys.length; i++) {
            result.addItem(this.keys[i], this.values[i].evaluate(environment));
        }
        return result;
    }

    toString() {
        let result = "<*";
        for (let i = 0; i < this.keys.length; i++) {
            result = result.concat(this.keys[i], "=", this.values[i], ", ");
        }
        if (result.length > 2) result = result.substr(0, result.length - 2);
        return result.concat("*>");
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        for (const item of this.values) {
            item.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }
}

export class NodeOr {
    constructor(pos) {
        this.expressions = [];
        this.pos = pos;
    }

    addOrClause(expression) {
        this.expressions.push(expression);
        return this;
    }

    getSimplified() {
        if (this.expressions.length == 1) {
            return this.expressions[0];
        }
        return this;
    }

    evaluate(environment) {
        for (const expression of this.expressions) {
            const value = expression.evaluate(environment);
            if (!value.isBoolean()) throw RuntimeError("Expected boolean but got " + value.type(), this.pos);
            if (value.value) {
                return ValueBoolean.TRUE;
            }
        }
        return ValueBoolean.FALSE;
    }

    toString() {
        let result = "(";
        for (const expression of this.expressions) {
            result = result.concat(expression, " or ");
        }
        if (result.length > 1) result = result.substr(0, result.length - " or ".length);
        return result.concat(")");
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        for (const expression of this.expressions) {
            expression.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }
}

export class NodeRequire {
    constructor(modulespec, name, unqualified, symbols, pos) {
        this.modulespec = modulespec;
        this.name = name;
        this.unqualified = unqualified;
        this.symbols = symbols;
        this.pos = pos;
    }

    evaluate(environment) {
        const modules = environment.getModules();
        // resolve module file, identifier and name
        let modulespec = null;
        if (this.modulespec instanceof NodeIdentifier) {
            modulespec = this.modulespec.value;
            /*
            If we have an identifier node, then two cases can happen:
            either the identifier signifies a module, then we want to retain the identifier
            otherwise, we evaluate the identifier and use the (necessarily) string value
            thus resulting. This allows things like 
                for module in sys->checkerlang_modules do
                    require module unqualified;
                done;
            while retaining the possibility to use
                require math
            even if the math module is already loaded (and thus "math" is defined in the environment)
            */
            if (environment.isDefined(modulespec)) {
                const val = environment.get(modulespec, this.pos);
                if (!(val.isObject() && val.isModule)) {
                    if (!val.isString()) throw new RuntimeError("Expected string or identifier modulespec but got " + modulespec.type(), this.pos);
                    modulespec = val.value;
                }
            }
        } else {
            modulespec = this.modulespec.evaluate(environment);
            if (!modulespec.isString()) throw new RuntimeError("Expected string or identifier modulespec but got " + modulespec.type(), this.pos);
            modulespec = modulespec.value;
        }
        let modulefile = modulespec;
        if (!modulefile.endsWith(".ckl")) modulefile += ".ckl";
        let moduleidentifier = null;
        let modulename = this.name;
        const parts = modulespec.split("/");
        let name = parts[parts.length - 1];
        if (name.endsWith(".ckl")) name = name.substr(0, name.length - 4);
        moduleidentifier = name;
        if (modulename == null) modulename = name;
        environment.pushModuleStack(moduleidentifier, this.pos);
        
        // lookup or read module
        let moduleEnv = null;
        if (modules.has(moduleidentifier)) {
            moduleEnv = modules.get(moduleidentifier);
        } else {
            moduleEnv = environment.getBase().newEnv();
            const modulesrc = moduleloader(modulefile, this.pos);
            const node = Parser.parseScript(modulesrc, "mod:" + modulefile.substr(0, modulefile.length - 4));
            node.evaluate(moduleEnv);
            modules.set(moduleidentifier, moduleEnv);
        }
        environment.popModuleStack();

        // bind module or contents of module
        if (this.unqualified) {
            for (const name of moduleEnv.getLocalSymbols()) {
                if (name.startsWith("_")) continue; // skip private module symbols
                environment.put(name, moduleEnv.get(name));
            }
        } else if (this.symbols !== null) {
            for (const name of moduleEnv.getLocalSymbols()) {
                if (name.startsWith("_")) continue; // skip private module symbols
                if (!this.symbols.has(name)) continue;
                environment.put(this.symbols.get(name), moduleEnv.get(name));
            }
        } else {
            const obj = new ValueObject();
            obj.isModule = true;
            for (const name of moduleEnv.getLocalSymbols()) {
                if (name.startsWith("_")) continue; // skip private module symbols
                const val = moduleEnv.get(name);
                if (val.isObject() && val.isModule) continue; // do not re-export modules!
                obj.addItem(name, val);
            }
            environment.put(modulename, obj);
        }
        return ValueNull.NULL;
    }

    toString() {
        return "(require " + this.modulespec + (this.name != null ? " as " + this.name : "") + (this.unqualified ? " unqualified" : "") + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        // empty
    }
}

export class NodeReturn {
    constructor(expression, pos) {
        this.expression = expression;
        this.pos = pos;
    }

    evaluate(environment) {
        return new ValueControlReturn(this.expression === null ? ValueNull.NULL : this.expression.evaluate(environment), this.pos);
    }

    toString() {
        return "(return" + (this.expression === null ? "" : " " + this.expression) + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        if (this.expression !== null) {
            this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }
}

export class NodeSet {
    constructor(pos) {
        this.items = [];
        this.pos = pos;
    }

    addItem(item) {
        this.items.push(item);
    }

    evaluate(environment) {
        const result = new ValueSet();
        for (const item of this.items) {
            result.addItem(item.evaluate(environment));
        }
        return result;
    }

    toString() {
        let result = "<<";
        for (const expression of this.items) {
            result = result.concat(expression, ", ");
        }
        if (result.length > 2) result = result.substr(0, result.length - 2);
        return result.concat(">>");
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        for (const item of this.items) {
            item.collectVars(freeVars, boundVars, additionalBoundVars);
        }
    }
}

export class NodeSetComprehension {
    constructor(valueExpr, identifier, listExpr, pos) {
        this.valueExpr = valueExpr;
        this.identifier = identifier;
        this.listExpr = listExpr;
        this.conditionExpr = null;
        this.pos = pos;
    }

    setCondition(conditionExpr) {
        this.conditionExpr = conditionExpr;
    }

    evaluate(environment) {
        const result = new ValueSet();
        const localEnv = environment.newEnv();
        const list = this.listExpr.evaluate(environment);
        let values = null;
        if (list.isList()) values = list.value;
        else if (list.isSet()) values = list.value.sortedValues();
        else if (list.isMap()) values = list.value.sortedEntries();
        else if (list.isString()) values = list.value.split("");
        for (const listValue of values) {
            localEnv.put(this.identifier, listValue);
            const value = this.valueExpr.evaluate(localEnv);
            if (this.conditionExpr != null) {
                const condition = this.conditionExpr.evaluate(localEnv);
                if (!(condition instanceof ValueBoolean)) {
                    throw new RuntimeError("Condition must be boolean but got " + condition.type(), this.pos);
                }
                if (condition.value) {
                    result.addItem(value);
                }
            } else {
                result.addItem(value);
            }
        }
        return result;
    }

    toString() {
        return "<<" + this.valueExpr + " for " + this.identifier + " in " + this.listExpr + (this.conditionExpr == null ? "" : (" if " + this.conditionExpr)) + ">>";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        const boundVarsLocal = [...boundVars];
        boundVarsLocal.push(this.identifier);
        this.valueExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
        this.listExpr.collectVars(freeVars, boundVars, additionalBoundVars);
        if (this.conditionExpr != null) this.conditionExpr.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }
}

export class NodeSpread {
    constructor(expression, pos) {
        this.expression = expression;
        this.pos = pos;
    }

    evaluate(environment) {
        return this.expression.evaluate(environment);
    }

    toString() {
        return "..." + this.expression;
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
    }
}

export class NodeWhile {
    constructor(expression, block, pos) {
        this.expression = expression;
        this.block = block;
        this.pos = pos;
    }

    evaluate(environment) {
        let condition = this.expression.evaluate(environment);
        if (!condition.isBoolean()) throw new RuntimeError("Expected boolean condition but got " + condition.type(), this.pos);
        let result = ValueBoolean.TRUE;
        while (condition.value) {
            result = this.block.evaluate(environment);
            if (result instanceof ValueControlBreak) {
                result = ValueBoolean.TRUE;
                break;
            } else if (result instanceof ValueControlContinue) {
                result = ValueBoolean.TRUE;
                // continue
            } else if (result instanceof ValueControlReturn) {
                break;
            }
            condition = this.expression.evaluate(environment);
            if (!condition.isBoolean()) throw new RuntimeError("Expected boolean condition but got " + condition.type(), this.pos);
        }
        return result;
    }

    toString() {
        return "(while " + this.expression + " do " + this.block + ")";
    }

    collectVars(freeVars, boundVars, additionalBoundVars) {
        this.expression.collectVars(freeVars, boundVars, additionalBoundVars);
        const boundVarsLocal = [...boundVars];
        this.block.collectVars(freeVars, boundVarsLocal, additionalBoundVars);
    }
}
