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
using System.IO;

namespace CheckerLang
{
    public class ValueOutput : Value
    {
        private TextWriter output;

        public ValueOutput(TextWriter output)
        {
            this.output = output;
        }

        public void Write(string value)
        {
            output.Write(value);
        }
        
        public void WriteLine(string value)
        {
            output.WriteLine(value);
        }
        
        public void Close()
        {
            output.Close();
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
            return "output";
        }

        public override ValueOutput AsOutput()
        {
            return this;
        }
        
        public override bool IsOutput()
        {
            return true;
        }
        
        public override string ToString()
        {
            return "<!output-stream>";
        }

        public string GetStringOutput() 
        {
            // This does only really work for string output objects...
            return output.ToString();
        }
    }
}