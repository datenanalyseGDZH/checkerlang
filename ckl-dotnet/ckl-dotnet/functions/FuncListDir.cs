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
    public class FuncListDir : FuncBase
    {
        public FuncListDir() : base("list_dir")
        {
            info = "list_dir(dir, recursive = FALSE, include_path = FALSE, include_dirs = FALSE)\r\n" +
                   "\r\n" +
                   "Enumerates the files and directories in the specified directory and\r\n" +
                   "returns a list of filename or paths.\r\n";
        }
        
        public override bool IsSecure()
        {
            return false;
        }
        
        public override List<string> GetArgNames()
        {
            return new List<string> {"dir", "recursive", "include_path", "include_dirs"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var dir = args.GetString("dir").GetValue();
            var recursive = false;
            if (args.HasArg("recursive")) recursive = args.GetBoolean("recursive").GetValue();
            var include_path = recursive;
            if (args.HasArg("include_path")) include_path = args.GetBoolean("include_path").GetValue();
            var include_dirs = false;
            if (args.HasArg("include_dirs")) include_dirs = args.GetBoolean("include_dirs").GetValue();
            var result = new ValueList();
            CollectFiles(dir, recursive, include_path, include_dirs, result);
            return result;
        }
        
        private void CollectFiles(string dir, bool recursive, bool include_path, bool include_dirs, ValueList result) {
            var files = Directory.EnumerateFileSystemEntries(dir);
            foreach (var file in files)
            {
                var isDir = (File.GetAttributes(file) & FileAttributes.Directory) != 0;
                if (include_dirs || !isDir)
                {
                    result.AddItem(new ValueString(include_path ? file : Path.GetFileName(file)));
                }
                if (recursive && isDir) CollectFiles(file, recursive, include_path, include_dirs, result);
            }
        }
    }
    
}