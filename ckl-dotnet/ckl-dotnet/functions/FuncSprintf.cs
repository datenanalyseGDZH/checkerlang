/*  Copyright (c) 2022 Damian Brunold, Gesundheitsdirektion Kanton Zürich

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
    public class FuncSprintf : FuncBase
    {
        public FuncSprintf() : base("sprintf")
        {
            info = "sprintf(fmt, args...)\r\n" +
                "\r\n" +
                "Formats a string format using the provided args. Each\r\n" +
                "value can be referred to in the fmt string using the\r\n" +
                "{0} syntax, where 0 means the first argument passed.\r\n" +
                "\r\n" +
                "This uses the same formatting suffixes as the s function.\r\n" + 
                "See there for an explanation of available formatting suffixes.\r\n" +
                "\r\n" +
                ": sprintf('{0} {1}', 1, 2) ==> '1 2'\r\n" +
                ": sprintf('{0} {1}', 'a', 'b') ==> 'a b'\r\n" +
                ": sprintf('{0#5} {1#5}', 1, 2) ==> '    1     2'\r\n" +
                ": sprintf('{0#-5} {1#-5}', 1, 2) ==> '1     2    '\r\n" +
                ": sprintf('{0#05} {1#05}', 1, 2) ==> '00001 00002'\r\n" +
                ": require Math; sprintf('{0#.4}', Math->PI) ==> '3.1416'\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"fmt", "args..."};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("fmt")) return ValueNull.NULL;
            var fmt = args.GetString("fmt").GetValue();
            var arguments = args.Get("args...").AsList().GetValue();
            var result = "";
            var lastidx = 0;
            var pattern = "\\{([0-9]+)(#[^}]+)?\\}";
            foreach (Match match in Regex.Matches(fmt, pattern)) 
            {
                var start = match.Groups[0].Index;
                var end = start + match.Groups[0].Length;
                var var = int.Parse(match.Groups[1].Value);
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

                string value;
                if (numbase != 10) {
                    value = string.Format("{0:x}", arguments[var].AsInt().GetValue());
                } else if (digits != -1) {
                    value = string.Format("{0:f" + digits + "}", arguments[var].AsDecimal().GetValue());
                } else {
                    value = arguments[var].AsString().GetValue();
                }
                while (value.Length < width) {
                    if (leading) value = ' ' + value;
                    else if (zeroes) value = '0' + value;
                    else value = value + ' ';
                }

                if (lastidx < start) result += fmt.Substring(lastidx, start-lastidx);
                result += value;
                lastidx = end;
            }
            if (lastidx < fmt.Length) result += fmt.Substring(lastidx);
            return new ValueString(result);
        }
    }   
}