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
package ch.checkerlang.functions;

import ch.checkerlang.Args;
import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueString;

import java.util.Arrays;
import java.util.List;
import java.util.Map;

public class FuncLs extends FuncBase {
    public FuncLs() {
        super("ls");
        info = "ls()\r\n" +
               "ls(module)\r\n" +
               "\r\n" +
               "Returns a list of all defined symbols (functions and constants)\r\n" +
               "in the current environment or the specified module.\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("module");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        ValueList result = new ValueList();
        if (args.hasArg("module")) {
            Value moduleArg = args.get("module");
            Map<String, Value> module;
            if (moduleArg.isString()) module = environment.get(moduleArg.asString().getValue(), pos).asObject().value;
            else module = args.get("module").asObject().value;
            for (String symbol : module.keySet()) {
                result.addItem(new ValueString(symbol));
            }
        } else {
            for (String symbol : environment.getSymbols()) {
                result.addItem(new ValueString(symbol));
            }
        }
        return result;
    }
}
