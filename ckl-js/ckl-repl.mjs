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

import { Interpreter } from "./js/interpreter.mjs";
import { Parser } from "./js/parser.mjs";
import { ValueNull, ValueList } from "./js/values.mjs";
import { system } from "./js/system.mjs";

import * as readline from "readline";
import * as fs from "fs";
import * as path from "path";

system.fs = fs;
system.path = path;

let secure = false;
let legacy = false;

for (let arg of process.argv.slice(2)) {
    if (arg === "--secure") secure = true;
    else if (arg === "--legacy") legacy = true;
}

const interpreter = new Interpreter(secure, legacy);
interpreter.baseEnvironment.set("stdout", interpreter.baseEnvironment.get("console"));
interpreter.environment.put("args", new ValueList());

for (let arg of process.argv.slice(2)) {
    if (arg.startsWith("--")) continue;
    const script = fs.readFileSync(arg, {encoding: 'utf8', flag: 'r'});
    interpreter.interpret(script, arg); // TODO better error handling/reporting
}

function interpretStatement(statement) {
    try {
        const result = interpreter.interpret(statement, "{repl}");
        if (!(result instanceof ValueNull)) {
            const str = result.toString();
            if (str !== null) {
                process.stdout.write(str);
                process.stdout.write("\n");
            } else {
                process.stdout.write("ERR: cannot convert object to string\n");
            }
        }
        process.stdout.write("> ");
    } catch (e) {
        console.log(e);
        let errortext = e.msg + (e.pos !== undefined ? " " + e.pos.toString() : "");
        if ("stacktrace" in e) {
            errortext += "\n\nStacktrace:\n" + e.stacktrace.join("\n")
        }
        process.stdout.write(errortext);
        process.stdout.write("\n");
        process.stdout.write("> ");
    }
}

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

process.stdout.write("> ");
let buffer = "";
rl.on("line", function(data) {
    if (data.toString('utf8').trim() === '') {
        interpretStatement(buffer);
        buffer = "";
    } else {
        buffer = buffer.concat(data.toString('utf8'));
        try {
            Parser.parseScript(buffer, "{repl}");
            interpretStatement(buffer);
            buffer = "";
        } catch (e) {
            process.stdout.write(": ");
        }
    }
});
