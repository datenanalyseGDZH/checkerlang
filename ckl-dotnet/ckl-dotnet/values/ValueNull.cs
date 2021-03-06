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
    public class ValueNull : Value
    {
        public static ValueNull NULL = new ValueNull();
        
        // prevent instantiation
        private ValueNull() {}

        public override bool IsEquals(Value value)
        {
            return value == NULL;
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
            return "null";
        }
 
        public override ValueNull AsNull()
        {
            return this;
        }

        public override ValueInt AsInt()
        {
            return new ValueInt(0);
        }

        public override ValueString AsString()
        {
            return new ValueString("");
        }

        public override bool IsNull()
        {
            return true;
        }
        
        public override string ToString()
        {
            return "NULL";
        }

    }
}