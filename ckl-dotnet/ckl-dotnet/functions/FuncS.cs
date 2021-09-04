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

using System;
using System.Collections.Generic;
using System.IO;

namespace CheckerLang
{
    public class FuncS : FuncBase
    {
        public FuncS() : base("s")
        {
            info = "s(str, start = 0)\r\n" +
                   "\r\n" +
                   "Returns a string, where all placeholders are replaced with their\r\n" +
                   "appropriate values. Placeholder have the form '{var}' and result\r\n" +
                   "in the value of the variable var inserted at this location.\r\n" +
                   "\r\n" +
                   "The placeholder can also be expressions and their result will\r\n" +
                   "be inserted instead of the placeholder.\r\n" +
                   "\r\n" +
                   "There are formatting suffixes to the placeholder, which allow\r\n" +
                   "some control over the formatting. They formatting spec starts after\r\n" +
                   "a # character and consists of align/fill, width and precision fields.\r\n" +
                   "For example #06.2 will format the decimal to a width of six characters\r\n" +
                   "and uses two digits after the decimal point. If the number is less than\r\n" +
                   "six characters wide, then it is prefixed with zeroes until the width\r\n" +
                   "is reached, e.g. '001.23'. Please refer to the examples below.\r\n" +
                   "\r\n" +
                   ": def name = 'damian'; s('hello {name}') ==> 'hello damian'\r\n" +
                   ": def foo = '{bar}'; def bar = 'baz'; s('{foo}{bar}') ==> '{bar}baz'\r\n" +
                   ": def a = 'abc'; s('a = {a#5}') ==> 'a =   abc'\r\n" +
                   ": def a = 'abc'; s('a = {a#-5}') ==> 'a = abc  '\r\n" +
                   ": def n = 12; s('n = {n#5}') ==> 'n =    12'\r\n" +
                   ": def n = 12; s('n = {n#-5}') ==> 'n = 12   '\r\n" +
                   ": def n = 12; s('n = {n#05}') ==> 'n = 00012'\r\n" +
                   ": def n = 1.2345678; s('n = {n#.2}') ==> 'n = 1.23'\r\n" +
                   ": def n = 1.2345678; s('n = {n#06.2}') ==> 'n = 001.23'\r\n" +
                   ": s('2x3 = {2*3}') ==> '2x3 = 6'\r\n" +
                   ": s('{PI} is cool') ==> '3.14159265358979 is cool'\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"str", "start"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("str")) return ValueNull.NULL;
            var str = args.GetString("str").GetValue();
            var start = (int) args.GetInt("start", 0).GetValue();
            if (start < 0) start = str.Length + start;
            while (true) {
                var idx1 = str.IndexOf('{', start);
                if (idx1 == -1) return new ValueString(str);
                var idx2 = str.IndexOf('}', idx1 + 1);
                if (idx2 == -1) return new ValueString(str);
                var variable = str.Substring(idx1 + 1, idx2 - idx1 - 1);
                var width = 0;
                var zeroes = false;
                var leading = true;
                var digits = -1;
                int idx3 = variable.IndexOf('#');
                if (idx3 != -1) {
                    var spec = variable.Substring(idx3 + 1);
                    variable = variable.Substring(0, idx3);
                    if (spec.StartsWith("-")) {
                        leading = false;
                        spec = spec.Substring(1);
                    }
                    if (spec.StartsWith("0")) {
                        zeroes = true;
                        leading = false;
                        spec = spec.Substring(1);
                    }
                    int idx4 = spec.IndexOf('.');
                    if (idx4 == -1) {
                        digits = -1;
                        width = int.Parse(spec);
                    } else {
                        digits = int.Parse(spec.Substring(idx4 + 1));
                        width = idx4 == 0 ? 0 : int.Parse(spec.Substring(0, idx4));
                    }
                }
                var node = Parser.Parse(variable, pos.filename);
                var value = node.Evaluate(environment).AsString().GetValue();
                if (digits != -1) value = string.Format("{0:f" + digits + "}", decimal.Parse(value));
                while (value.Length < width) {
                    if (leading) value = ' ' + value;
                    else if (zeroes) value = '0' + value;
                    else value = value + ' ';
                }
                str = str.Substring(0, idx1) + value + str.Substring(idx2 + 1);
                start = idx1 + value.Length;
            }
        }
    }
    
}