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

import { HashSet } from "./collections.mjs";
import { HashMap } from "./collections.mjs";
import { RuntimeError } from "./errors.mjs";

const DAYS_PER_MONTH = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
const DAYS_EPOCH = 25569;

export function isLeapYear(year) {
    return (year % 4 === 0) && ((year % 100 !== 0) || (year % 400 === 0));
}

function yearDays(year) {
    return isLeapYear(year) ? 366 : 365;
}

function monthDays(year, month) {
    return isLeapYear(year) && month == 1 ? 29 : DAYS_PER_MONTH[month];
}

export function convertDateToOADate(date) {
    const year = date.getFullYear();
    let result = 1;
    for (let y = 1900; y < year; y++) {
        result += isLeapYear(y) ? 366 : 365;
    }
    const month = date.getMonth();
    for (let m = 0; m < month; m++) {
        result += monthDays(year, m);
    }
    result += date.getDate();
    result += date.getHours() / 24;
    result += date.getMinutes() / 24 / 60;
    result += date.getSeconds() / 24 / 60 / 60;
    result += date.getMilliseconds() / 24 / 60 / 60 / 1000;
    return result;
}

export function convertOADateToDate(oadate) {
    let value = oadate - DAYS_EPOCH;
    let year = 1970;
    while (value > yearDays(year)) {
        value -= yearDays(year);
        year++;
    }
    let month = 0;
    while (value > monthDays(year, month)) {
        value -= monthDays(year, month);
        month++;
    }
    let day = Math.trunc(value) + 1;
    value = value - Math.trunc(value);
    let hours = Math.trunc(value * 24);
    value = value * 24 - hours;
    let minutes = Math.trunc(value * 60);
    value = value * 60 - minutes;
    let seconds = Math.trunc(value * 60);
    value = value * 60 - seconds;
    let milliseconds = Math.trunc(value * 1000);
    return new Date(year, month, day, hours, minutes, seconds, milliseconds);
}

export class StringInput {
    constructor(str) {
        this.input = str;
        this.pos = 0;
    }

    process(callback) {
        let line = this.readLine();
        let count = 0;
        while (line !== null) {
            callback(line);
            count++;
            line = this.readLine();
        }
        return count;
    }

    read() {
        if (this.pos >= this.input.length) return null;
        const result = this.input.substr(this.pos, 1);
        this.pos++;
        return result;
    }

    readAll() {
        if (this.pos >= this.input.length) return null;
        const result = this.input.substring(this.pos);
        this.pos = this.input.length;
        return result;
    }

    readLine() {
        if (this.pos >= this.input.length) return null;
        if (this.input.indexOf("\n", this.pos) != -1) {
            const result = this.input.substring(this.pos, this.input.indexOf("\n", this.pos));
            this.pos = this.input.indexOf("\n", this.pos) + 1;
            return result;
        } else {
            const result = this.input.substring(this.pos);
            this.pos = this.input.length;
            return result;
        }
    }

    close() {}
}

export class StringOutput {
    constructor() {
        this.output = "";
    }

    write(s) {
        this.output = this.output.concat(s);
    }

    writeLine(s) {
        this.output = this.output.concat(s, "\n");
    }

    flush() {}
    close() {}
}


export class ConsoleOutput {
    constructor() {
        this.buffer = "";
    }

    write(s) {
        this.buffer = this.buffer.concat(s);
        while (this.buffer.indexOf("\n") !== -1) {
            console.log(this.buffer.substring(0, this.buffer.indexOf("\n")));
            this.buffer = this.buffer.substring(this.buffer.indexOf("\n") + 1);
        }
    }

    writeLine(s) {
        this.buffer = this.buffer.concat(s, "\n");
        while (this.buffer.indexOf("\n") !== -1) {
            console.log(this.buffer.substring(0, this.buffer.indexOf("\n")));
            this.buffer = this.buffer.substring(this.buffer.indexOf("\n") + 1);
        }
    }

    flush() {
        if (this.buffer !== "") console.log(this.buffer);
        this.buffer = "";
    }

    close() {}
}


export class FileInput {
    constructor(filename, encoding, fs) {
        this.fs = fs;
        this.encoding = encoding.toLowerCase();
        if (this.encoding === 'utf-8') this.encoding = 'utf8';
        // TODO rewrite to use open stream instead of reading all in at start.
        const data = fs.readFileSync(filename, {encoding: this.encoding, flag: 'r'});
        this.buffer = new StringInput(data);
    }
    process(callback) { this.buffer.process(callback); }
    read() { return this.buffer.read(); }
    readAll() { return this.buffer.readAll(); }
    readLine() { return this.buffer.readLine(); }
    close() { return this.buffer.close(); }
}


export class FileOutput {
    constructor(filename, encoding, append, fs) {
        this.fs = fs;
        this.encoding = encoding.toLowerCase();
        if (this.encoding === 'utf-8') this.encoding = 'utf8';
        let flags = "w";
        if (append) flags = "a";
        this.fd = this.fs.openSync(filename, flags);
    }
    write(s) { 
        this.fs.writeSync(this.fd, s, null, this.encoding);
    }

    writeLine(s) { 
        this.fs.writeSync(this.fd, s + "\n", null, this.encoding);
    }

    flush() { 
        // TODO how does this work in nodejs?
    }

    close() { 
        this.fs.closeSync(this.fd);
        this.fd = undefined;
    }
}


function cmp(a, b) {
    if (a < b) return -1;
    if (a > b) return 1;
    return 0;
}


export class Value {
    constructor() {
        this.info = "";
    }

    asString() {
        throw new RuntimeError("Cannot convert to String");
    }

    asInt() {
        throw new RuntimeError("Cannot convert to int");
    }

    asDecimal() {
        throw new RuntimeError("Cannot convert to decimal");
    }

    asBoolean() {
        throw new RuntimeError("Cannot convert to booleanean");
    }

    asPattern() {
        throw new RuntimeError("Cannot convert to pattern");
    }

    asDate() {
        throw new RuntimeError("Cannot convert to date");
    }

    asList() {
        throw new RuntimeError("Cannot convert to list");
    }

    asSet() {
        throw new RuntimeError("Cannot convert to set");
    }

    asMap() {
        throw new RuntimeError("Cannot convert to map");
    }

    asFunc() {
        throw new RuntimeError("Cannot convert to func");
    }

    asError() {
        throw new RuntimeError("Cannot convert to error");
    }

    asInput() {
        throw new RuntimeError("Cannot convert to input");
    }

    asOutput() {
        throw new RuntimeError("Cannot convert to output");
    }

    asNull() {
        throw new RuntimeError("Cannot convert to NULL");
    }

    asNode() {
        throw new RuntimeError("Cannot convert to Node");
    }

    asObject() {
        throw new RuntimeError("Cannot convert to Object");
    }

    asBreak() {
        throw new RuntimeError("Cannot convert to break");
    }

    asContinue() {
        throw new RuntimeError("Cannot convert to continue");
    }

    asReturn() {
        throw new RuntimeError("Cannot convert to return");
    }

    isString() {
        return false;
    }

    isInt() {
        return false;
    }

    isDecimal() {
        return false;
    }

    isBoolean() {
        return false;
    }

    isDate() {
        return false;
    }

    isPattern() {
        return false;
    }

    isList() {
        return false;
    }

    isSet() {
        return false;
    }

    isMap() {
        return false;
    }

    isObject() {
        return false;
    }

    isFunc() {
        return false;
    }

    isError() {
        return false;
    }

    isInput() {
        return false;
    }

    isOutput() {
        return false;
    }

    isNull() {
        return false;
    }

    isNode() {
        return false;
    }

    isBreak() {
        return false;
    }

    isContinue() {
        return false;
    }

    isReturn() {
        return false;
    }

    isCollection() { 
        return this.isList() || this.isSet(); 
    }

    isAtomic() {
        return this.isString() || this.isInt() || this.isDecimal() || this.isBoolean() || this.isDate() || this.isPattern() || this.isNull();
    }

    isNumerical() {
        return this.isInt() || this.isDecimal();
    }

    withInfo(info) {
        this.info = info;
        return this;
    }

}

export class ValueBoolean extends Value {
    constructor(value) {
        super();
        this.value = value;
    }

    static TRUE = new ValueBoolean(true);
    static FALSE = new ValueBoolean(false);

    static from(value) {
        return value ? ValueBoolean.TRUE : ValueBoolean.FALSE;
    }

    isTrue() {
        return this.value;
    }

    isFalse() {
        return !this.value;
    }

    isEquals(other) {
        if (!(other instanceof ValueBoolean)) return false;
        return this.value === other.value;
    }

    compareTo(other) {
        if (!(other instanceof ValueBoolean)) return this.toString().localeCompare(other.toString());
        return this.value - other.value;
    }
    
    type() {
        return "boolean";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asInt() {
        return new ValueInt(this.value ? 1 : 0);
    }

    asDecimal() {
        return new ValueDecimal(this.value ? 1 : 0);
    }

    asBoolean() {
        return this;
    }

    asPattern() {
        return this.asString().asPattern();
    }

    asList() {
        return new ValueList().addItem(this);
    }

    isBoolean() {
        return true;
    }

    toString() {
        return this.value ? "TRUE" : "FALSE";
    }

}

export class ValueControlBreak extends Value {
    constructor(pos) {
        super();
        this.pos = pos;
    }

    isEquals(other) {
        return this === other;
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    type() {
        return "break";
    }

    isBreak() {
        return true;
    }

    asBreak() {
        return this;
    }

    toString() {
        return "break";
    }
}

export class ValueControlContinue extends Value {
    constructor(pos) {
        super();
        this.pos = pos;
    }

    isEquals(other) {
        return this === other;
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    type() {
        return "continue";
    }

    isContinue() {
        return true;
    }

    asContinue() {
        return this;
    }

    toString() {
        return "continue";
    }
}

export class ValueControlReturn extends Value {
    constructor(value, pos) {
        super();
        this.value = value;
        this.pos = pos;
    }

    isEquals(vaotherlue) {
        return this === other;
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    type() {
        return "return";
    }

    isReturn() {
        return true;
    }

    asReturn() {
        return this;
    }

    toString() {
        return "return " + this.value;
    }
}

export class ValueDate extends Value {
    constructor(value = null) {
        super();
        this.value = value == null ? new Date() : value;
    }

    isEquals(other) {
        if (!(other instanceof ValueDate)) return false;
        return this.value.getTime() == other.value.getTime();
    }

    compareTo(other) {
        if (!(other instanceof ValueDate)) return this.toString().localeCompare(other.toString());
        return this.value.getTime() - other.value.getTime();
    }

    type() {
        return "date";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asInt() {
        return new ValueInt(Math.trunc(convertDateToOADate(this.value)));
    }

    asDecimal() {
        return new ValueDecimal(convertDateToOADate(this.value));
    }

    asDate() {
        return this;
    }

    asList() {
        return new ValueList().addItem(this);
    }

    isDate() {
        return true;
    }

    toString() {
        // format yyyyMMdd
        let year = String(this.value.getFullYear())
        let month = String(this.value.getMonth() + 1);
        let day = String(this.value.getDate());

        if (month.length == 1) month = "0" + month;
        if (day.length == 1) day = "0" + day;

        return year.concat(month, day);
    }

}

export class ValueDecimal extends Value {
    constructor(value) {
        super();
        this.value = value;
    }

    isEquals(other) {
        if (!other.isNumerical()) return false;
        return this.value === other.asDecimal().value;
    }

    compareTo(other) {
        if (!other.isNumerical()) return cmp(this.toString(), other.toString());
        return this.value - other.value;
    }

    type() {
        return "decimal";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asInt() {
        return new ValueInt(Math.trunc(this.value));
    }

    asDecimal() {
        return this;
    }

    asDate() {
        return new ValueDate(convertOADateToDate(this.value));
    }

    asList() {
        return new ValueList().addItem(this);
    }

    isDecimal() {
        return true;
    }

    toString() {
        const result = this.value.toString();
        if (result.indexOf(".") == -1) return result + ".0";
        return result;
    }

}

export class ValueError extends Value {
    constructor(value) {
        super();
        this.value = value;
    }

    isEquals(other) {
        return other === this;
    }

    compareTo(other) {
        if (!(other instanceof ValueError)) return this.toString().localeCompare(other.toString());
        return this.value.compareTo(other.value);
    }

    type() {
        return "error";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asError() {
        return this;
    }

    isError() {
        return true;
    }

    toString() {
        return "ERROR:" + this.value;
    }
}

export class ValueFunc extends Value {
    constructor(name) {
        super();
        this.name = name;
    }

    isEquals(other) {
        return other == this;
    }

    compareTo(other) {
        return this.toString().compareTo(other.toString());
    }

    type() {
        return "func";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asFunc() {
        return this;
    }

    isFunc() {
        return true;
    }

    toString() {
        return "<#" + this.name + ">";
    }
}

export class ValueInput extends Value {
    constructor(input) {
        super();
        this.input = input;
        this.closed = false;
    }

    process(callback) {
        return this.input.process(callback);
    }

    readLine() {
        return this.input.readLine();
    }

    read() {
        return this.input.read();
    }

    readAll() {
        return this.input.readAll();
    }

    close() {
        if (this.closed) return;
        this.input.close();
        this.closed = true;
    }

    isEquals(other) {
        return other == this;
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    type() {
        return "input";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asInput() {
        return this;
    }

    isInput() {
        return true;
    }

    toString() {
        return "<!input-stream>";
    }
}

export class ValueInt extends Value {
    constructor(value) {
        super();
        this.value = value;
    }

    isEquals(other) {
        if (!other.isNumerical()) return false;
        if (other instanceof ValueDecimal) return this.asDecimal().isEquals(other);
        return this.value == other.value;
    }

    compareTo(other) {
        if (!other.isNumerical()) return cmp(this.toString(), other.toString());
        if (other instanceof ValueDecimal) return this.asDecimal().compareTo(other);
        return this.value - other.value;
    }

    type() {
        return "int";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asInt() {
        return this;
    }

    asDecimal() {
        return new ValueDecimal(this.value);
    }

    asBoolean() {
        return ValueBoolean.from(this.value !== 0);
    }

    asDate() {
        return new ValueDate(convertOADateToDate(this.value));
    }

    asList() {
        return new ValueList().addItem(this);
    }

    isInt() {
        return true;
    }

    toString() {
        return this.value.toString();
    }
}

export class ValueList extends Value {
    constructor() {
        super();
        this.value = [];
    }

    addItems(list) {
        this.value = this.value.concat(list);
        return this;
    }

    addItem(item) {
        this.value.push(item);
        return this;
    }

    findItem(item) {
        for (let i = 0; i < this.value.length; i++) {
            if (this.value[i].isEquals(item)) {
                return i;
            }
        }
        return -1;
    }

    removeItem(item) {
        const pos = this.findItem(item);
        if (pos === -1) return;
        this.value.splice(pos, 1);
    }

    deleteAt(index) {
        let idx = index;
        if (idx < 0) idx = this.value.length + idx;
        if (idx >= this.value.length) return ValueNull.NULL;
        const result = this.value[idx];
        this.value.splice(idx, 1);
        return result;
    }

    insertAt(index, value) {
        let idx = index;
        if (idx < 0) idx = this.value.length + idx;
        if (idx > this.value.length) return this;
        if (idx === this.value.length) this.value.push(value);
        else this.value.splice(idx, 0, value);
        return this;
    }

    isEquals(other) {
        if (!(other instanceof ValueList)) return false;
        if (this.value.length !== other.value.length) return false;
        for (let i = 0; i < this.value.length; i++) {
            if (!this.value[i].isEquals(other.value[i])) {
                return false;
            }
        }
        return true;
    }

    compareTo(other) {
        if (!(other instanceof ValueList)) return this.toString().localeCompare(other.toString());
        for (let i = 0; i < Math.min(this.value.length, other.value.length); i++) {
            const cmp = this.value[i].compareTo(other.value[i]);
            if (cmp !== 0) return cmp;
        }

        return other.value.length - this.value.length;
    }

    type() {
        return "list";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asInt() {
        return new ValueInt(this.value.length);
    }

    asBoolean() {
        return ValueBoolean.from(this.value.length > 0);
    }

    asList() {
        return this;
    }

    asSet() {
        const result = new ValueSet();
        for (const item of this.value) {
            result.addItem(item);
        }
        return result;
    }

    asMap() {
        const result = new ValueMap();
        for (const entry of this.value) {
            result.addItem(entry.value[0], entry.value[1]);
        }
        return result;
    }

    isList() {
        return true;
    }

    toString() {
        let result = "[";
        for (const item of this.value) {
            result = result.concat(item.toString(), ", ");
        }
        if (result.length > 1) result = result.substr(0, result.length - 2);
        return result.concat("]");
    }
}

export class ValueMap extends Value {
    constructor() {
        super();
        this.value = new HashMap();
    }

    addMap(map) {
        for (const [key, value] of map) {
            this.value.set(key, value);
        }
        return this;
    }

    addItem(key, value) {
        this.value.set(key, value);
        return this;
    }

    hasItem(key) {
        return this.value.has(key);
    }

    getItem(key) {
        return this.value.get(key);
    }

    removeItem(key) {
        this.value.remove(key);
    }

    getSortedKeys() {
        return this.value.sortedKeys();
    }

    isEquals(other) {
        if (!(other instanceof ValueMap)) return false;
        if (this.value.size != other.value.size) return false;
        for (const [key, val] of this.value.entries()) {
            if (!other.value.has(key)) {
                return false;
            }
            if (!val.isEquals(other.value.get(key))) {
                return false;
            }
        }
        return true;
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    type() {
        return "map";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asInt() {
        return new ValueInt(this.value.size);
    }

    asBoolean() {
        return ValueBoolean.from(this.value.size > 0);
    }

    asList() {
        const result = new ValueList();
        for (const val of this.value.sortedValues()) {
            result.addItem(val);
        }
        return result;
    }

    asSet() {
        const result = new ValueSet();
        for (const key of this.value.keys()) {
            result.addItem(key);
        }
        return result;
    }

    asMap() {
        return this;
    }

    isMap() {
        return true;
    }

    toString() {
        let result = "<<<";
        for (const key of this.getSortedKeys()) {
            result = result.concat(key.toString(), " => ", this.getItem(key).toString(), ", ");
        }
        if (result.length > "<<<".length) result = result.substr(0, result.length - 2);
        return result.concat(">>>");
    }
}

export class ValueNode extends Value {
    constructor(value) {
        super();
        this.value = value;
    }

    isEquals(other) {
        if (!(other instanceof ValueNode)) return false;
        return this.value.toString() == this.toString();
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    type() {
        return "node";
    }

    asNode() { 
        return this; 
    }

    asString() {
        return new ValueString(this.toString());
    }

    isNode() {
        return true;
    }

    toString() {
        return this.value.toString();
    }
}

export class ValueNull extends Value {
    constructor() {
        super();
        this.value = null;
    }

    static NULL = new ValueNull();

    isEquals(other) {
        return other === ValueNull.NULL;
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    type() {
        return "null";
    }

    asNull() {
        return this;
    }

    asInt() {
        return new ValueInt(0);
    }

    asString() {
        return new ValueString("");
    }

    isNull() {
        return true;
    }

    toString() {
        return "NULL";
    }
}

export class ValueObject extends Value {
    constructor() {
        super();
        this.value = new Map();
    }

    addItem(key, value) {
        this.value.set(key, value);
        return this;
    }

    hasItem(key) {
        return this.value.has(key);
    }

    getItem(key) {
        return this.value.get(key);
    }

    removeItem(key) {
        this.value.remove(key);
    }

    isEquals(other) {
        if (!(other instanceof ValueObject)) return false;
        if (this.value.size != other.value.size) return false;
        for (const [key, val] of this.value.entries()) {
            if (!other.value.has(key)) {
                return false;
            }
            if (!val.isEquals(other.value.get(key))) {
                return false;
            }
        }
        return true;
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    type() {
        return "object";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asInt() {
        return new ValueInt(this.value.size);
    }

    asBoolean() {
        return ValueBoolean.from(this.value.size > 0);
    }

    asList() {
        const result = new ValueList();
        for (const val of this.value.values()) {
            result.addItem(val);
        }
        return result;
    }

    asSet() {
        const result = new ValueSet();
        for (const key of this.value.keys()) {
            result.addItem(key);
        }
        return result;
    }

    asMap() {
        const result = new ValueMap();
        for (const [key, val] of this.value) {
            result.addItem(key, val);
        }
        return result;
    }

    asObject() {
        return this;
    }

    isObject() {
        return true;
    }

    toString() {
        let result = "<!";
        for (const key of this.value.keys()) {
            result = result.concat(key.toString(), "=", this.value.get(key).toString(), ", ");
        }
        if (result.length > "<!".length) result = result.substr(0, result.length - 2);
        return result.concat("!>");
    }
}

export class ValueOutput extends Value {
    constructor(output) {
        super();
        this.output = output;
        this.closed = false;
    }

    write(str) {
        this.output.write(str);
    }

    writeLine(str) {
        this.output.write(str);
        this.output.write("\n");
        this.output.flush();
    }

    close() {
        if (this.closed) return;
        this.output.close();
        this.closed = true;
    }

    isEquals(other) {
        return other === this;
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    type() {
        return "output";
    }

    asOutput() {
        return this;
    }

    isOutput() {
        return true;
    }

    toString() {
        return "<!output-stream>";
    }

    getStringOutput() {
        // This does only really work for string output objects...
        return this.output.toString();
    }
}

export class ValuePattern extends Value {
    constructor(value) {
        super();
        this.value = value;
        this.pattern = new RegExp(value);
    }

    isEquals(other) {
        if (!(other instanceof ValuePattern)) return false;
        return this.value === other.value;
    }

    compareTo(other) {
        if (!(other instanceof ValuePattern)) return toString().localeCompare(other.toString());
        if (this.value < other.value) return -1;
        if (this.value > other.value) return 1;
        return 0;
    }

    type() {
        return "pattern";
    }

    asString() {
        return new ValueString(this.value);
    }

    asPattern() {
        return this;
    }

    asList() {
        return new ValueList().addItem(this);
    }

    isPattern() {
        return true;
    }

    toString() {
        return "//" + this.value + "//";
    }
}

export class ValueSet extends Value {
    constructor() {
        super();
        this.value = new HashSet();
    }

    addItem(item) {
        this.value.add(item);
        return this;
    }

    addItems(items) {
        for (const item of items) {
            this.addItem(item);
        }
        return this;
    }

    hasItem(item) {
        return this.value.has(item);
    }

    removeItem(item) {
        this.value.remove(item);
    }

    isEquals(other) {
        if (!(other instanceof ValueSet)) return false;
        if (this.value.size != other.value.size) return false;
        for (const item of this.value.values()) {
            if (!other.value.has(item)) return false;
        }
        return true;
    }

    compareTo(other) {
        return this.toString().localeCompare(other.toString());
    }

    getSortedItems() {
        return this.value.sortedValues();
    }

    type() {
        return "set";
    }

    asString() {
        return new ValueString(this.toString());
    }

    asInt() {
        return new ValueInt(this.value.length);
    }

    asBoolean() {
        return ValueBoolean.from(this.value.length > 0);
    }

    asList() {
        const result = new ValueList();
        for (const item of this.getSortedItems()) {
            result.addItem(item);
        }
        return result;
    }

    asSet() {
        return this;
    }

    isSet() {
        return true;
    }

    toString() {
        let result = "<<";
        for (const item of this.getSortedItems()) {
            result = result.concat(item.asString().value, ", ");
        }
        if (result.length > "<<".length) result = result.substr(0, result.length - 2);
        return result.concat(">>");
    }
}

export class ValueString extends Value {
    constructor(value) {
        super();
        this.value = value;
    }

    isEquals(other) {
        if (!(other instanceof ValueString)) return false;
        return this.value == other.value;
    }

    compareTo(other) {
        return cmp(this.toString(), other.toString());
    }

    type() {
        return "string";
    }

    matches(pattern) {
        return this.value.match(pattern.asPattern().pattern);
    }

    asString() {
        return this;
    }

    asInt() {
        const n = Number(this.value);
        if (isNaN(n)) throw new RuntimeError("Cannot convert " + this.value  + " to int");
        if (n !== Math.trunc(n)) throw new RuntimeError("Cannot convert " + this.value  + " to int");
        return new ValueInt(Math.trunc(n));
    }

    asDecimal() {
        const n = Number(this.value);
        if (isNaN(n)) throw new RuntimeError("Cannot convert " + this.value  + " to decimal");
        return new ValueDecimal(n);
    }

    asBoolean() {
        if (this.value === "1") return ValueBoolean.TRUE;
        if (this.value === "0") return ValueBoolean.FALSE;
        return ValueBoolean.from(this.value.toUpperCase() === "TRUE");
    }

    asDate() {
        // handle yyyyMMddHH and yyyyMMdd, throw exception if not matching
        if (this.value.length < 8) throw new RuntimeError("Cannot convert " + this.value + " to date");
        const year = Number(this.value.substr(0, 4));
        const month = Number(this.value.substr(4, 2));
        const day = Number(this.value.substr(6, 2));
        const hour = this.value.length == 10 ? Number(this.value.substr(8, 2)) : 0;
        if (isNaN(year) || isNaN(month) || isNaN(day) || isNaN(hour)) throw new RuntimeError("Cannot convert " + this.value + " to date");
        return new ValueDate(new Date(year, month - 1, day, hour));
    }

    asPattern() {
        return new ValuePattern(this.value);
    }

    asList() {
        return new ValueList().addItem(this);
    }

    isString() {
        return true;
    }

    toString() {
        let result = this.value.replace(/\\/g, "\\\\").replace(/'/g, "\\'");
        return "'" + result + "'";
    }
}
