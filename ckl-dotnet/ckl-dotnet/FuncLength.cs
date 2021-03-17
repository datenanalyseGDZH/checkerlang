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
    public class FuncLength : FuncBase
    {
        public FuncLength() : base("length")
        {
            info = "length(obj)\r\n" +
                   "\r\n" +
                   "Returns the length of obj. This only works for strings, lists, sets and maps.\r\n" +
                   "\r\n" +
                   ": length('123') ==> 3\r\n" +
                   ": length([1, 2, 3]) ==> 3\r\n" +
                   ": length(<<1, 2, 3>>) ==> 3\r\n" +
                   ": <<<'a' => 1, 'b' => 2, 'c' =>3>>> !> length() ==> 3\r\n" +
                   ": length(object()) ==> 0\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"obj"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var arg = args.Get("obj");
            if (arg.IsString()) return new ValueInt(arg.AsString().GetValue().Length);
            if (arg.IsList()) return new ValueInt(arg.AsList().GetValue().Count);
            if (arg.IsSet()) return new ValueInt(arg.AsSet().GetValue().Count);
            if (arg.IsMap()) return new ValueInt(arg.AsMap().GetValue().Count);
            if (arg.IsObject()) return new ValueInt(arg.AsObject().value.Count);
            throw new ControlErrorException("Cannot determine length of " + arg, pos);
        }
    }
}