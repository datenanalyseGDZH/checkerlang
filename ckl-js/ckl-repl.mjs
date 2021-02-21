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
import { ValueNull } from "./js/values.mjs";
import { FuncFileInput, FuncFileOutput, FuncRun } from "./js/functions.mjs";

import * as fs from "fs";
import { ValueList } from "./js/values.mjs";

const interpreter = new Interpreter(false);

interpreter.baseEnvironment.set("stdout", interpreter.baseEnvironment.get("console"));
interpreter.baseEnvironment.parent.put("file_input", new FuncFileInput(fs));
interpreter.baseEnvironment.parent.put("file_output", new FuncFileOutput(fs));
interpreter.baseEnvironment.parent.put("run", new FuncRun(interpreter, fs));

interpreter.environment.put("args", new ValueList());

for (let scriptname of process.argv.slice(2)) {
    const script = fs.readFileSync(scriptname, {encoding: 'utf8', flag: 'r'});
    interpreter.interpret(script, scriptname); // TODO better error handling/reporting
}

function interpretStatement(statement) {
    try {
        const result = interpreter.interpret(statement, "{repl}");
        if (!(result instanceof ValueNull)) {
            process.stdout.write(result.toString());
            process.stdout.write("\n");
        }
        process.stdout.write("> ");
    } catch (e) {
        let errortext = e.msg + (e.pos !== undefined ? " " + e.pos.toString() : "");
        if ("stacktrace" in e) {
            errortext += "\n\nStacktrace:\n" + e.stacktrace.join("\n")
        }
        process.stdout.write(errortext);
        process.stdout.write("\n");
        process.stdout.write("> ");
    }
}

process.stdout.write("> ");
let buffer = "";
process.stdin.on("data", function(data) {
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
