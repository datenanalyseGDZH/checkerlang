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
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CheckerLang
{
    public class FuncExecute : FuncBase
    {
        public FuncExecute() : base("execute")
        {
             info = "execute(program, args, work_dir = NULL, echo = FALSE, output_file = NULL)\r\n" +
                 "\r\n" +
                 "Executed the program and provides the specified arguments in the list args.\r\n";
        }

        public override bool IsSecure()
        {
            return false;
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"program", "args", "work_dir", "echo", "output_file"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var program = args.GetString("program").GetValue();
            var arguments = args.GetList("args").GetValue();
            string work_dir = null;
            if (args.HasArg("work_dir")) work_dir = args.GetString("work_dir").GetValue();
            var echo = false;
            if (args.HasArg("echo")) echo = args.GetBoolean("echo").GetValue();
            string output_file = null;
            if (args.HasArg("output_file")) output_file = args.GetString("output_file").GetValue();
            try
            {
                var list = new StringBuilder();
                foreach (var argument in arguments) {
                    list.Append(argument.AsString().GetValue()).Append(" ");
                }
                list.Remove(list.Length - 1, 1);
                var process = new Process();
                var info = process.StartInfo;
                info.FileName = program;
                info.Arguments = list.ToString();
                if (work_dir != null) info.WorkingDirectory = work_dir;
                info.CreateNoWindow = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = false;
                info.RedirectStandardInput = false;
                if (echo) System.Console.WriteLine(program + " " + list);
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                if (output_file != null)
                {
                    File.WriteAllText(output_file, output);
                }
                else
                {
                    Console.Write(output);
                }
                process.WaitForExit();
                return new ValueInt(process.ExitCode);
            } catch (Exception e) {
                throw new ControlErrorException(new ValueString("ERROR"), "Cannot execute " + program + ": " + e.Message, pos);
            }
         }
    }
    
}