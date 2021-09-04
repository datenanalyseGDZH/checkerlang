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

import ch.checkerlang.*;
import ch.checkerlang.values.Value;

import java.io.*;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;
import java.util.List;

public class FuncRun extends FuncBase {
    private Interpreter interpreter;

    public FuncRun(Interpreter interpreter) {
        super("run");
        this.interpreter = interpreter;
        info = "run(file)\r\n" +
                "\r\n" +
                "Loads and interprets the file.\r\n";
    }

    public boolean isSecure() {
        return false;
    }

    public List<String> getArgNames() {
        return Arrays.asList("file");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        String file = args.getString("file").getValue();
        try {
            try (Reader input = new InputStreamReader(new FileInputStream(file), StandardCharsets.UTF_8)) {
                return interpreter.interpret(input, new File(file).getName());
            }
        } catch (IOException e) {
            throw new ControlErrorException("File " + file + " not found", pos);
        }
    }
}
