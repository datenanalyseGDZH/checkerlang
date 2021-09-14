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
        if (args.length == 0) {
            System.err.println("Syntax: ckl-run-java [--secure] [--legacy] [-I<moduledir>] scriptname [scriptargs...]");
            System.exit(1);
        }

        boolean secure = false;
        boolean legacy = false;
        String scriptname = null;
        ValueList scriptargs = new ValueList();
        ValueList modulepath = new ValueList();

        boolean in_options = true;
        for (String arg : args) {
            if (in_options) {
                if (arg.equals("--secure")) secure = true;
                else if (arg.equals("--legacy")) legacy = true;
                else if (arg.startsWith("-I")) {
                    modulepath.addItem(new ValueString(arg.substring(2)));
                } else if (arg.startsWith("--")) {
                    System.err.println("Unknown option " + arg);
                    System.exit(1);
                } else {
                    in_options = false;
                    scriptname = arg;
                }
            } else {
                scriptargs.addItem(new ValueString(arg));
            }
        }
        modulepath.makeReadonly();

        if (scriptname == null) {
            System.err.println("Syntax: ckl-run-java [--secure] [--legacy] [-I<moduledir>] scriptname [scriptargs...]");
            System.exit(1);
        }

        if (!new File(scriptname).exists()) {
            System.err.println("File not found " + scriptname);
            System.exit(1);
        }

        BufferedReader stdin = new BufferedReader(new InputStreamReader(System.in));
        PrintWriter stdout = new PrintWriter(new OutputStreamWriter(System.out));
        try {
            Interpreter interpreter = new Interpreter(secure, legacy);
            interpreter.setStandardInput(stdin);
            interpreter.setStandardOutput(stdout);
            interpreter.getEnvironment().put("scriptname", new ValueString(scriptname));
            interpreter.getEnvironment().put("args", scriptargs);
            interpreter.getEnvironment().put("checkerlang_module_path", modulepath);
            try (Reader input = new InputStreamReader(new FileInputStream(scriptname), StandardCharsets.UTF_8)) {
                Value value = interpreter.interpret(input, scriptname);
                if (value.isReturn()) value = value.asReturn().value;
                if (value != ValueNull.NULL) stdout.println(value);
            }
            stdout.flush();
        } catch (ControlErrorException e) {
            stdout.println("ERR: " + e.getErrorValue().asString().getValue() + " (Line " + e.getPos() + ")");
            stdout.println(e.getStacktrace().toString());
            stdout.flush();
        } catch (SyntaxError e) {
            stdout.println(e.getMessage() + (e.getPos() != null ? " (Line " + e.getPos() + ")" : ""));
            stdout.flush();
        } catch (Exception e) {
            e.printStackTrace();
            stdout.println(e);
            stdout.flush();
        }
    }
}
