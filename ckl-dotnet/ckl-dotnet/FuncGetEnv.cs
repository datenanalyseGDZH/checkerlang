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
using System.Text;

namespace CheckerLang
{
    public class FuncGetEnv : FuncBase
    {
        public FuncGetEnv() : base("get_env")
        {
            info = "get_env(var)\r\n" +
                   "\r\n" +
                   "Returns the value of the environment variable var.\r\n";
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"var"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var var = args.GetString("var").GetValue();
            return new ValueString(System.Environment.GetEnvironmentVariable(var));
        }
    }
    
}