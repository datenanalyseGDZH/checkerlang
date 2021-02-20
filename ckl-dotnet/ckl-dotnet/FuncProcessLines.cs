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
using System.Collections.Generic;

namespace CheckerLang
{
    public class FuncProcessLines : FuncBase 
    {
        public FuncProcessLines() : base("process_lines") {
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

        public override List<string> GetArgNames()
        {
            return new List<string> {"input", "callback"};
        }

        public override Value Execute(Args args, Environment environment, SourcePos pos) 
        {
            var inparg = args.Get("input");
            var callback = args.Get("callback").AsFunc();
            Environment env = environment.NewEnv();
            if (inparg is ValueInput) {
                ValueInput input = inparg.AsInput();
                return new ValueInt(input.Process(line => {
                    Args args_ = new Args(callback.GetArgNames()[0], new ValueString(line), pos);
                    return callback.Execute(args_, env, pos);
                }));
            }
            if (inparg is ValueList) {
                var list = inparg.AsList().GetValue();
                foreach (var element in list) {
                    Args args_ = new Args(callback.GetArgNames()[0], element.AsString(), pos);
                    callback.Execute(args_, env, pos);
                };
                return new ValueInt(list.Count);
            }
            throw new ControlErrorException("Cannot process lines from " + inparg, pos);
        }
    }
}