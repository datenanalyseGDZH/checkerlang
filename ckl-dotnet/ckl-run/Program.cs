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
using System.IO;
using System.Text;
using CheckerLang;

namespace run
{
    class Program
    {
        static void Main(string[] args)
        {
            var interpreter = new Interpreter(false);
            interpreter.SetStandardInput(Console.In);
            interpreter.SetStandardOutput(Console.Out);
            if (File.Exists("base-library.txt"))
            {
                interpreter.LoadFile("base-library.txt");
            }
            
            // TODO handle options places before the scriptname, e.g. -i for include path!
            if (args.Length > 0) {
                interpreter.GetEnvironment().Put("scriptname", new ValueString(args[0]));
                var arglist = new ValueList();
                for (var i = 1; i < args.Length; i++) {
                    arglist.AddItem(new ValueString(args[i]));
                }
                interpreter.GetEnvironment().Put("args", arglist);
                var input = new StreamReader(args[0], Encoding.UTF8);
                try {
                    var value = interpreter.Interpret(input, args[0]);
                    if (value.IsReturn()) value = value.AsReturn().value;
                    if (value != ValueNull.NULL) Console.Out.WriteLine(value);
                } catch (ControlErrorException e) {
                    Console.Out.WriteLine("ERR: " + e.GetErrorValue().AsString().GetValue() + " (Line " + e.GetPos() + ")");
                    Console.Out.WriteLine(e.GetStacktrace().ToString());
                } catch (SyntaxError e) {
                    Console.Out.WriteLine(e.Message + (e.GetPos() != null ? " (Line " + e.GetPos() + ")" : ""));
                } catch (Exception e) {
                    Console.Out.WriteLine(e.Message);
                }
            }
        }
    }
}
