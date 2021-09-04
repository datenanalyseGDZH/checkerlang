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
import ch.checkerlang.values.ValueNull;

import java.io.*;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;

public class REPL {
    public static void main(String[] args) throws Exception {
        BufferedReader stdin = new BufferedReader(new InputStreamReader(System.in));
        PrintWriter stdout = new PrintWriter(new OutputStreamWriter(System.out));
        boolean secure = false;
        boolean legacy = false;
        for (String arg : args) {
            if (arg.equals("--secure")) secure = true;
            if (arg.equals("--legacy")) legacy = true;
        }
        Interpreter interpreter = new Interpreter(secure, legacy);
        interpreter.setStandardInput(stdin);
        interpreter.setStandardOutput(stdout);
        for (String arg : args) {
            if (arg.startsWith("--")) continue;
            interpreter.interpret(new String(Files.readAllBytes(new File(arg).toPath()), StandardCharsets.UTF_8), arg);
        }
        stdout.print("> ");
        stdout.flush();
        String line = stdin.readLine();
        while (!line.equals("exit")) {
            try {
                Parser.parse(line, "{stdin}");
            } catch (SyntaxError e) {
                if (e.getMessage().startsWith("Unexpected end of input")) {
                    stdout.write("+ ");
                    stdout.flush();
                    line += "\n" + stdin.readLine();
                    continue;
                }
            } catch (Exception e) {
                stdout.write("+ ");
                stdout.flush();
                line += "\n" + stdin.readLine();
                continue;
            }

            if (!line.equals(";")) {
                try {
                    Value value = interpreter.interpret(line, "{stdin}");
                    if (value.isReturn()) value = value.asReturn().value;
                    if (value != ValueNull.NULL) {
                        String str = value.toString();
                        if (str != null) {
                            stdout.println(str);
                        } else {
                            stdout.println("ERR: cannot convert value to string");
                        }
                    }
                } catch (ControlErrorException e) {
                    stdout.println("ERR: " + e.getErrorValue().asString().getValue() + " (Line " + e.getPos() + ")");
                    stdout.println(e.getStacktrace().toString());
                } catch (SyntaxError e) {
                    stdout.println(e.getMessage() + (e.getPos() != null ? " (Line " + e.getPos() + ")" : ""));
                } catch (Exception e) {
                    stdout.println(e.getMessage());
                    e.printStackTrace();
                }
            }

            stdout.print("> ");
            stdout.flush();
            line = stdin.readLine();
        }
    }
}
