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
import { ValueList, ValueString, ValueNull } from "./js/values.mjs";
import { system } from "./js/system.mjs";

import { exit } from "process";
import * as fs from "fs";
import * as path from "path";
import * as child_process from "child_process";
import * as os from "os";

system.fs = fs;
system.path = path;
system.process = process;
system.child_process = child_process;
system.os = os;

let secure = false;
let legacy = false;
let scriptname = null;
let scriptargs = [];
let modulepath = new ValueList();

if (process.argv.length <= 2) {
    process.stderr.write("Syntax: ckl-run [--secure] [--legacy] [-I<moduledir>] scriptname [scriptargs...]\n");
    exit(1);
}

let in_options = true;
for (let arg of process.argv.slice(2)) {
    if (in_options) {
        if (arg === "--secure") secure = true;
        else if (arg === "--legacy") legacy = true;
        else if (arg.startsWith("-I")) {
            modulepath.addItem(new ValueString(arg.substring(2)));
        } else if (arg.startsWith("--")) {
            process.stderr.write(`Unknown option ${arg}\n`);
            exit(1);
        } else {
            in_options = false;
            scriptname = arg;
        }
    } else {
        scriptargs.push(arg);
    }
}

if (scriptname === null) {
    process.stderr.write("Syntax: ckl-run [--secure] [--legacy] [-I<moduledir>] scriptname [scriptargs...]\n");
    exit(1);
}

const interpreter = new Interpreter(secure, legacy);
interpreter.baseEnvironment.set("stdout", interpreter.baseEnvironment.get("console"));

if (!fs.existsSync(scriptname)) {
    process.stderr.write(`File not found '${scriptname}'\n`);
    exit(1);
}

const args = new ValueList();
for (const scriptarg of scriptargs) args.addItem(new ValueString(scriptarg));
interpreter.environment.put("args", args);
interpreter.environment.put("scriptname", new ValueString(scriptname));
interpreter.environment.put("checkerlang_module_path", modulepath);

const script = fs.readFileSync(scriptname, {encoding: 'utf8', flag: 'r'});

try {
    const result = interpreter.interpret(script, scriptname);
    if (result !== ValueNull.NULL) {
        process.stdout.write(result.toString());
        process.stdout.write("\n");
    }
} catch (e) {
    if (e.msg !== undefined) {
        let errortext = e.msg + (e.pos !== undefined ? " " + e.pos.toString() : "");
        if ("stacktrace" in e) {
            errortext += "\n\nStacktrace:\n" + e.stacktrace.join("\n")
        }
        process.stdout.write(errortext);
        process.stdout.write("\n");
    } else {
        console.log(e);
    }
}
