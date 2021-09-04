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
using System.IO;

namespace CheckerLang
{
    public class FuncRun : FuncBase
    {
        private Interpreter interpreter;
        
        public FuncRun(Interpreter interpreter) : base("run")
        {
            this.interpreter = interpreter;
            info = "run(file)\r\n" +
                   "\r\n" +
                   "Loads and interprets the file.\r\n";
        }
        
        public override bool IsSecure()
        {
            return false;
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"file"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var file = args.GetString("file").GetValue();
            try
            {
                return interpreter.Interpret(File.ReadAllText(file), file);
            }
            catch (FileNotFoundException)
            {
                throw new ControlErrorException(new ValueString("ERROR"),"File " + file + " not found", pos);
            }
        }
    }
}