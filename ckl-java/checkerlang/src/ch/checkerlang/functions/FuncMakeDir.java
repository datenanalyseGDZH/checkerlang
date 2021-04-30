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
import ch.checkerlang.values.ValueNull;

import java.io.File;
import java.util.Arrays;
import java.util.List;

public class FuncMakeDir extends FuncBase {
    public FuncMakeDir() {
        super("make_dir");
        this.info = "make_dir(dir, with_parents = FALSE)\r\n" +
                "\r\n" +
                "Creates a new directory.\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("dir", "with_parents");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        File dir = new File(args.getString("dir").getValue());
        if (dir.exists() && dir.isDirectory()) return ValueNull.NULL;
        boolean with_parents = false;
        if (args.hasArg("with_parents")) with_parents = args.getBoolean("with_parents").getValue();
        if (with_parents) {
            if (!dir.mkdirs()) {
                throw new ControlErrorException("Cannot create directory " + dir, pos);
            }
        } else {
            if (!dir.mkdir()) {
                throw new ControlErrorException("Cannot create directory " + dir, pos);
            }
        }
        return ValueNull.NULL;
    }

}
