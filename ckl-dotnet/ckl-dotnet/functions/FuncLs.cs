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
    public class FuncLs : FuncBase
    {
        public FuncLs() : base("ls")
        {
            info = "ls()\r\n" +
                   "ls(module)\r\n" +
                   "\r\n" +
                   "Returns a list of all defined symbols (functions and constants)\r\n" +
                   "in the current environment or in the specified module.\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string>{"module"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var result = new ValueList();
            if (!args.HasArg("module"))
            {
                foreach (var symbol in environment.GetSymbols())
                {
                    result.AddItem(new ValueString(symbol));
                }
            }
            else
            {
                var moduleArg = args.Get("module");
                Dictionary<string, Value> module;
                if (moduleArg.IsString()) module = environment.Get(moduleArg.AsString().GetValue(), pos).AsObject().value;
                else module = args.Get("module").AsObject().value;
                foreach (var symbol in module.Keys)
                {
                    result.AddItem(new ValueString(symbol));
                }
            }
            return result;
        }
    }
}
