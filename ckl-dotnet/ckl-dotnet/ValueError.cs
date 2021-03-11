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
namespace CheckerLang
{
    public class ValueError : Value
    {
        protected Value value;

        public ValueError(Value value)
        {
            this.value = value;
        }

        public Value GetValue()
        {
            return value;
        }
        
        public override bool IsEquals(Value value)
        {
            return value == this;
        }

        public override int CompareTo(Value value)
        {
            if (!value.IsError()) return string.CompareOrdinal(ToString(), value.ToString());
            return this.value.CompareTo(value.AsError().value);
        }

        public override string Type()
        {
            return "error";
        }

        public override int HashCode()
        {
            return value.GetHashCode();
        }

        public override ValueString AsString()
        {
            return new ValueString(ToString());
        }

        public override ValueError AsError()
        {
            return this;
        }

        public override bool IsError()
        {
            return true;
        }
        
        public override string ToString()
        {
            return "ERROR:" + value;
        }
    }
}