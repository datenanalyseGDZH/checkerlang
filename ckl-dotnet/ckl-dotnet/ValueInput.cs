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

namespace CheckerLang
{
    public class ValueInput : Value
    {
        private TextReader input;

        public ValueInput(TextReader input)
        {
            this.input = input;
        }

        public int Process(Func<string, Value> callback) {
            try {
                var line = input.ReadLine();
                var count = 0;
                while (line != null) {
                    count++;
                    callback.Invoke(line);
                    line = input.ReadLine();
                }
                return count;
            } catch (IOException) {
                throw new ControlErrorException("Cannot process file", SourcePos.Unknown);
            }
        }

        public string ReadLine()
        {
            return input.ReadLine();
        }

        public string Read()
        {
            var ch = input.Read();
            return ch == -1 ? null : ((char) ch).ToString();
        }

        public string ReadAll()
        {
            return input.ReadToEnd();
        }

        public void Close()
        {
            input.Close();
        }
        
        public override bool IsEquals(Value value)
        {
            return value == this;
        }
        
        public override int CompareTo(Value value)
        {
            return string.CompareOrdinal(ToString(), value.ToString());
        }

        public override int HashCode()
        {
            return 0;
        }

        public override string Type()
        {
            return "input";
        }

        public override ValueString AsString()
        {
            return new ValueString(ToString());
        }

        public override ValueInput AsInput()
        {
            return this;
        }
        
        public override bool IsInput()
        {
            return true;
        }
        
        public override string ToString()
        {
            return "<!input-stream>";
        }

    }
}