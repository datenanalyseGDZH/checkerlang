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
using CheckerLang;

namespace repl
{
    class Program
    {
        static void Main(string[] args)
        {
            var interpreter = new Interpreter(false, false);
            interpreter.SetStandardInput(Console.In);
            interpreter.SetStandardOutput(Console.Out);
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line != null && line == "secure")
            {
                interpreter.MakeSecure();
                Console.WriteLine("Switched to secure mode");
                Console.Write("> ");
                line = Console.ReadLine();
            }
            while (line != "exit")
            {
                try
                {
                    Parser.Parse(line, "{stdin}");
                }
                catch (SyntaxError e)
                {
                    if (e.Message.StartsWith("Unexpected end of input"))
                    {
                        Console.Write("+ ");
                        line += "\n" + Console.ReadLine();
                        continue;
                    }
                }
                catch (Exception)
                {
                    Console.Write("+ ");
                    line += "\n" + Console.ReadLine();
                    continue;
                }

                if (!line.Equals(";"))
                {
                    try
                    {
                        var value = interpreter.Interpret(line, "{stdin}");
                        if (value.IsReturn()) value = value.AsReturn().value;
                        if (value != ValueNull.NULL) Console.WriteLine(value);
                    }
                    catch (ControlErrorException e)
                    {
                        Console.WriteLine("ERR: " + e.GetErrorValue().AsString().GetValue() + " (Line " + e.GetPos() + ")");
                        Console.WriteLine(e.GetStacktrace().ToString());
                    }
                    catch (SyntaxError e)
                    {
                        Console.WriteLine(e.Message + (e.GetPos() != null ? " (Line " + e.GetPos() + ")" : ""));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                Console.Write("> ");
                line = Console.ReadLine();
            }
        }
    }
}
