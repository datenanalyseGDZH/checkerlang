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
import ch.checkerlang.values.*;

import java.util.Arrays;
import java.util.List;

public class FuncProcessLines extends FuncBase {
    public FuncProcessLines() {
        super("process_lines");
        this.info = "process_lines(input, callback)\r\n" +
                "\r\n" +
                "Reads lines from the input and calls the callback function\r\n" +
                "once for each line. The line string is the single argument\r\n" +
                "of the callback function.\r\n" +
                "\r\n" +
                "If input is a list, then each list element is converted to\r\n" +
                "a string and processed as a line\r\n" +
                "\r\n" +
                "The function returns the number of processed lines." +
                "\r\n" +
                ": def result = []; str_input('one\\ntwo\\nthree') !> process_lines(fn(line) result !> append(line)); result ==> ['one', 'two', 'three']\r\n" +
                ": str_input('one\\ntwo\\nthree') !> process_lines(fn(line) line) ==> 3\r\n" +
                ": def result = ''; process_lines(['a', 'b', 'c'], fn(line) result += line); result ==> 'abc'\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("input", "callback");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value inparg = args.get("input");
        ValueFunc callback = args.get("callback").asFunc();
        Environment env = environment.newEnv();
        if (inparg instanceof ValueInput) {
            ValueInput input = inparg.asInput();
            return new ValueInt(input.process(line -> {
                Args args_ = new Args(callback.getArgNames().get(0), new ValueString(line), pos);
                return callback.execute(args_, env, pos);
            }));
        } else if (inparg instanceof ValueList) {
            List<Value> list = inparg.asList().getValue();
            for (Value element : list) {
                Args args_ = new Args(callback.getArgNames().get(0), element.asString(), pos);
                callback.execute(args_, env, pos);
            };
            return new ValueInt(list.size());
        } else {
            throw new ControlErrorException("Cannot process lines from " + inparg.toString(), pos);
        }
    }
}
