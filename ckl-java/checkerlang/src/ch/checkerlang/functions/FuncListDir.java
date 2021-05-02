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

import java.io.*;
import java.util.Arrays;
import java.util.List;

public class FuncListDir extends FuncBase {
    public FuncListDir() {
        super("list_dir");
        info = "list_dir(dir, recursive = FALSE, include_path = FALSE, include_dirs = FALSE)\r\n" +
                "\r\n" +
                "Enumerates the files and directories in the specified directory and\r\n" +
                "returns a list of filename or paths.\r\n";
    }

    public boolean isSecure() {
        return false;
    }

    public List<String> getArgNames() {
        return Arrays.asList("dir", "recursive", "include_path", "include_dirs");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        String dir = args.getString("dir").getValue();
        boolean recursive = false;
        if (args.hasArg("recursive")) recursive = args.getBoolean("recursive").getValue();
        boolean include_path = recursive;
        if (args.hasArg("include_path")) include_path = args.getBoolean("include_path").getValue();
        boolean include_dirs = false;
        if (args.hasArg("include_dirs")) include_dirs = args.getBoolean("include_dirs").getValue();
        ValueList result = new ValueList();
        collectFiles(new File(dir), recursive, include_path, include_dirs, result);
        return result;
    }

    private void collectFiles(File dir, boolean recursive, boolean include_path, boolean include_dirs, ValueList result) {
        File[] files = dir.listFiles();
        if (files == null) return;
        for (File file : files) {
            if (include_dirs || !file.isDirectory()) {
                result.addItem(new ValueString(include_path ? file.getPath() : file.getName()));
            }
            if (recursive && file.isDirectory()) collectFiles(file, recursive, include_path, include_dirs, result);
        }
    }
}
