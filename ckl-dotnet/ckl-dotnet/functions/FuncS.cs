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
using System.Text.RegularExpressions;
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
                   ": def n = 123; s('n = {n#x}') ==> 'n = 7b'\r\n" +
                   ": def n = 255; s('n = {n#04x}') ==> 'n = 00ff'\r\n" +
                   ": s('{1} { {2}') ==> '1 { 2'\r\n" +
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
            var start_ = (int) args.GetInt("start", 0).GetValue();
            if (start_ < 0) start_ = str.Length + start_;

            var result = "";
            if (start_ > 0) {
                result = str.Substring(0, start_);
                str = str.Substring(start_);
            }

            var lastidx = 0;
            var pattern = "\\{([^#{}]+)(#-?[0-9.]*x?)?\\}";

            foreach (Match match in Regex.Matches(str, pattern)) 
            {
                var start = match.Groups[0].Index;
                var end = start + match.Groups[0].Length;
                var expr = match.Groups[1].Value;
                var spec = match.Groups[2].Value;

                var width = 0;
                var zeroes = false;
                var leading = true;
                int numbase = 10;
                var digits = -1;

                if (spec != null && spec != "") {
                    spec = spec.Substring(1); // skip #
                    if (spec.StartsWith("-")) {
                        leading = false;
                        spec = spec.Substring(1);
                    }
                    if (spec.StartsWith("0")) {
                        zeroes = true;
                        leading = false;
                        spec = spec.Substring(1);
                    }
                    if (spec.EndsWith("x")) {
                        numbase = 16;
                        spec = spec.Substring(0, spec.Length - 1);
                    }
                    int idx = spec.IndexOf('.');
                    if (idx == -1) {
                        digits = -1;
                        width = int.Parse(spec == "" ? "0" : spec);
                    } else {
                        digits = int.Parse(spec.Substring(idx + 1));
                        width = idx == 0 ? 0 : int.Parse(spec.Substring(0, idx));
                    }
                }

                string value = "";
                try {
                    var node = Parser.Parse(expr, pos.filename);
                    var val = node.Evaluate(environment);
                    if (numbase != 10) value = string.Format("{0:x}", val.AsInt().GetValue());
                    else if (digits != -1) value = string.Format("{0:f" + digits + "}", val.AsDecimal().GetValue());
                    else value = val.AsString().GetValue();
                    while (value.Length < width) {
                        if (leading) value = ' ' + value;
                        else if (zeroes) value = '0' + value;
                        else value = value + ' ';
                    }
                } catch (Exception) {
                    // ignore
                }

                if (lastidx < start) result += str.Substring(lastidx, start-lastidx);
                result += value;
                lastidx = end;
            }
            if (lastidx < str.Length) result += str.Substring(lastidx);
            return new ValueString(result);
        }
    }
}