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
import ch.checkerlang.values.ValueNode;

import java.util.Arrays;
import java.util.List;

public class FuncBody extends FuncBase {
    public FuncBody() {
        super("body");
        info = "body(f)\r\n" +
                "\r\n" +
                "Returns the body of the lambda f.\r\n" +
                "\r\n" +
                ": body(fn(x) 2 * x) ==> '(mul 2, x)'\r\n";
    }

    public List<String> getArgNames() {
        return Arrays.asList("f");
    }

    public Value execute(Args args, Environment environment, SourcePos pos) {
        Value f = args.get("f");
        if (f instanceof FuncLambda)
        {
            return new ValueNode(((FuncLambda) f).getBody());
        }
        throw new ControlErrorException("f is not a lambda function", pos);
    }

}
