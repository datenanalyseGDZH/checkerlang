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
import { Environment } from "./functions.mjs";
import { system } from "./system.mjs";

import { 
    StringInput,
    StringOutput,
    ConsoleOutput,
    ValueInput,
    ValueOutput,
} from "./values.mjs";

export class Interpreter {
    constructor(secure = true, legacy = false) {
        this.baseEnvironment = Environment.getBaseEnvironment(secure, legacy);
        this.environment = this.baseEnvironment.newEnv();
        this.baseEnvironment.put("console", new ValueOutput(new ConsoleOutput()));
        if (!secure) {
            this.baseEnvironment.put("stdout", new ValueOutput(new StringOutput())); // TODO in case of node.js use actual stdout
            this.baseEnvironment.put("stdin", new ValueInput(new StringInput("")));  // TODO in case of node.js use actual stdin
        }
    }

    makeSecure() {
        this.baseEnvironment.remove("stdout");
        this.baseEnvironment.remove("stdin");
        this.baseEnvironment.remove("run");
        this.baseEnvironment.remove("file_input");
        this.baseEnvironment.remove("file_output");
        this.baseEnvironment.remove("close");
    }

    setStandardOutput(stdout) {
        this.baseEnvironment.put("stdout", new ValueOutput(stdout));
    }

    setStandardInput(stdin) {
        this.baseEnvironment.put("stdin", new ValueInput(stdin));
    }

    loadFile(filename, encoding = "utf8") {
        let enc = encoding.toLowerCase()
        if (enc === 'utf-8') enc = 'utf8';
        const contents = system.fs.readFileSync(filename, {encoding: enc, flag: 'r'});
        interpret(contents, filename); // TODO extract filename from path
    }

    interpret(script, filename, environment = null) {
        let savedParent = null;
        let env;
        if (environment === null) {
            env = this.environment;
        } else {
            let environment_ = environment;
            while (environment_ !== null && environment_.getParent() !== null) {
                environment_ = environment_.getParent();
            }
            if (environment_ != null) {
                savedParent = environment_.getParent();
                environment_.withParent(this.environment);
            }
            env = environment;
        }
        try {
            const result = Parser.parseScript(script, filename).evaluate(env);
            if (result.isReturn()) {
                return result.value;
            } else if (result.isBreak()) {
                throw new RuntimeError("ERROR", "Cannot use break without surrounding loop", result.asBreak().pos);
            } else if (result.isContinue()) {
                throw new RuntimeError("ERROR", "Cannot use continue without surrounding loop", result.asContinue().pos);
            }
            return result;
        } finally {
            if (savedParent != null) {
                let environment_ = environment;
                while (environment_ != null && environment_.getParent() != null) {
                    environment_ = environment_.getParent();
                }
                if (environment_ != null) environment_.withParent(savedParent);
            }
        }
    }
}
