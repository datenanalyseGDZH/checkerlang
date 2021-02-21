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
import { FuncFileInput, FuncFileOutput, FuncRun } from "./js/functions.mjs";

import * as fs from "fs";
import { ValueList } from "./js/values.mjs";
import { ValueString } from "./js/values.mjs";
import { exit } from "process";
import { ValueNull } from "./js/values.mjs";

const interpreter = new Interpreter(false);

interpreter.fs = fs;
interpreter.baseEnvironment.set("stdout", interpreter.baseEnvironment.get("console"));
interpreter.baseEnvironment.parent.put("file_input", new FuncFileInput(fs));
interpreter.baseEnvironment.parent.put("file_output", new FuncFileOutput(fs));
interpreter.baseEnvironment.parent.put("run", new FuncRun(interpreter, fs));

if (process.argv.length <= 2) exit(0);

const scriptname = process.argv[2];
const scriptargs = process.argv.slice(3);

if (!fs.existsSync(scriptname)) {
    process.stderr.write(`File not found '${scriptname}'\n`);
    exit(1);
}

const args = new ValueList();
for (const scriptarg of scriptargs) args.addItem(new ValueString(scriptarg));
interpreter.environment.put("args", args);

const script = fs.readFileSync(scriptname, {encoding: 'utf8', flag: 'r'}); // TODO specify encoding and other stuff with flags?

try {
    const result = interpreter.interpret(script, scriptname);
    if (result !== ValueNull.NULL) {
        process.stdout.write(result.toString());
        process.stdout.write("\n");
    }
} catch (e) {
    let errortext = e.msg + (e.pos !== undefined ? " " + e.pos.toString() : "");
    if ("stacktrace" in e) {
        errortext += "\n\nStacktrace:\n" + e.stacktrace.join("\n")
    }
    process.stdout.write(errortext);
    process.stdout.write("\n");
}
