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

import java.io.IOException;
import java.util.Arrays;
import java.util.List;

public class FuncClose extends FuncBase {
    public FuncClose() {
        super("close");
        info = "close(conn)\r\n" +
                "\r\n" +
                "Closes the input or output connection and releases system resources.\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("conn");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value conn = args.get("conn");
        if (conn.isInput()) {
            try {
                conn.asInput().close();
            } catch (IOException e) {
                throw new ControlErrorException("Could not close connection", pos);
            }
            return ValueNull.NULL;
        }

        if (conn.isOutput()) {
            try {
                conn.asOutput().close();
            } catch (Exception e) {
                throw new ControlErrorException("Could not close connection", pos);
            }
            return ValueNull.NULL;
        }

        throw new ControlErrorException("Cannot close " + conn, pos);
    }
}
