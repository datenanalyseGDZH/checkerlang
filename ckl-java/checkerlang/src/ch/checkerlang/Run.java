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
package ch.checkerlang;

import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueNull;
import ch.checkerlang.values.ValueString;

import java.io.*;
import java.nio.charset.StandardCharsets;

public class Run {
    public static void main(String[] args) throws Exception {
        BufferedReader stdin = new BufferedReader(new InputStreamReader(System.in));
        PrintWriter stdout = new PrintWriter(new OutputStreamWriter(System.out));
        try {
            Interpreter interpreter = new Interpreter(false);
            interpreter.setStandardInput(stdin);
            interpreter.setStandardOutput(stdout);
            // TODO handle options placed before the scriptname, e.g. -i for include path!
            if (args.length > 0) {
                interpreter.getEnvironment().put("scriptname", new ValueString(args[0]));
                ValueList arglist = new ValueList();
                for (int i = 1; i < args.length; i++) {
                    arglist.addItem(new ValueString(args[i]));
                }
                interpreter.getEnvironment().put("args", arglist);
                try (Reader input = new InputStreamReader(new FileInputStream(args[0]), StandardCharsets.UTF_8)) {
                    Value value = interpreter.interpret(input, args[0]);
                    if (value.isReturn()) value = value.asReturn().value;
                    if (value != ValueNull.NULL) stdout.println(value);
                }
            }
            stdout.flush();
        } catch (ControlErrorException e) {
            stdout.println("ERR: " + e.getErrorValue().asString().getValue() + " (Line " + e.getPos() + ")");
            stdout.println(e.getStacktrace().toString());
        } catch (SyntaxError e) {
            stdout.println(e.getMessage() + (e.getPos() != null ? " (Line " + e.getPos() + ")" : ""));
        } catch (Exception e) {
            stdout.println(e.getMessage());
        }
    }
}
