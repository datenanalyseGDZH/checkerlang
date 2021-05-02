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
import ch.checkerlang.ControlErrorException;
import ch.checkerlang.Environment;
import ch.checkerlang.SourcePos;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueInt;
import ch.checkerlang.values.ValueNull;

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.StandardCopyOption;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class FuncExecute extends FuncBase {
    public FuncExecute() {
        super("execute");
        info = "execute(program, args, work_dir = NULL, echo = FALSE)\r\n" +
                "\r\n" +
                "Executed the program and provides the specified arguments in the list args.\r\n";
    }

    public boolean isSecure() {
        return false;
    }

    public List<String> getArgNames() {
        return Arrays.asList("program", "args", "work_dir", "echo");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        String program = args.getString("program").getValue();
        List<Value> arguments = args.getList("args").getValue();
        String work_dir = null;
        if (args.hasArg("work_dir")) work_dir = args.getString("work_dir").getValue();
        boolean echo = false;
        if (args.hasArg("echo")) echo = args.getBoolean("echo").getValue();
        try {
            List<String> list = new ArrayList<>();
            list.add(program);
            for (Value argument : arguments) {
                list.add(argument.asString().getValue());
            }
            ProcessBuilder proc = new ProcessBuilder(list);
            proc.inheritIO();
            proc.directory(work_dir == null ? null : new File(work_dir));
            if (echo) System.out.println(proc.command());
            Process process = proc.start();
            process.waitFor();
            return new ValueInt(process.exitValue());
        } catch (Exception e) {
            throw new ControlErrorException("Cannot execute " + program + ": " + e.getMessage(), pos);
        }
    }

}
