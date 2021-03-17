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
using System.Text.RegularExpressions;

namespace CheckerLang
{
    public class FuncSplit : FuncBase
    {
        public FuncSplit() : base("split")
        {
            info = "split(str, delim = '[ \\t]+')\r\n" +
                   "\r\n" +
                   "Splits the string str into parts and returns a list of strings.\r\n" +
                   "The delim is a regular expression. Default is spaces or tabs.\r\n" +
                   "\r\n" +
                   ": split('a,b,c', //,//) ==> ['a', 'b', 'c']\r\n" +
                   ": split('', '-') ==> []\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"str", "delim"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("str")) return ValueNull.NULL;
            var value = args.GetString("str").GetValue();
            var delim = "[ \\t]+";
            if (args.HasArg("delim")) delim = args.GetAsString("delim").GetValue();
            return SplitValue(value, delim);
        }

        public static ValueList SplitValue(string value, string delim)
        {
            if (value == "") return new ValueList();
            
            if (delim == "\\|" || delim == "\\.")
            {
                var values = new List<Value>();
                foreach (var part in value.Split(delim[1]))
                {
                    values.Add(new ValueString(part));
                }
                
                return new ValueList(values);
            } 
            else if (delim == "," || delim == ":" || delim == ";" || delim == "-")
            {
                var values = new List<Value>();
                foreach (var part in value.Split(delim[0]))
                {
                    values.Add(new ValueString(part));
                }
                
                return new ValueList(values);
            }
            else if (delim == "")
            {
                var values = new List<Value>();
                foreach (var ch in value)
                {
                    values.Add(new ValueString(ch.ToString()));
                }

                return new ValueList(values);
            }
            else
            {
                var values = new List<Value>();
                foreach (var part in Regex.Split(value, delim, RegexOptions.CultureInvariant))
                {
                    values.Add(new ValueString(part));
                }

                return new ValueList(values);
            }
        }
    }
}