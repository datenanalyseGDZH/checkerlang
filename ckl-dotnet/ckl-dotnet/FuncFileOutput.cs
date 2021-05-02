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
    public class FuncFileOutput : FuncBase
    {
        public FuncFileOutput() : base("file_output")
        {
            info = "file_output(filename, encoding = 'UTF-8', append = FALSE)\r\n" +
                   "\r\n" +
                   "Returns an output object, that writes to the given file. If\r\n" +
                   "the file exists it is overwritten, unless append is TRUE.\r\n";
        }
        
        public override bool IsSecure()
        {
            return false;
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"filename", "encoding", "append"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var filename = args.GetString("filename").GetValue();
            var encoding = args.GetString("encoding", "UTF-8").GetValue();
            var append = ValueBoolean.FALSE;
            if (args.HasArg("append")) append = args.Get("append").AsBoolean();
            return new ValueOutput(new StreamWriter(filename, append.GetValue(), encoding == "UTF-8" ? Encoding.UTF8 : Encoding.GetEncoding(encoding)));
        }
    }
    
}