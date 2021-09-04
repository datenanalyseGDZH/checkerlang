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
    public class ValueBoolean : Value
    {
        public static readonly ValueBoolean TRUE = new ValueBoolean(true);
        public static readonly ValueBoolean FALSE = new ValueBoolean(false);

        private readonly bool value;

        private ValueBoolean(bool value)
        {
            this.value = value;
        }

        public static ValueBoolean From(bool value)
        {
            return value ? TRUE : FALSE;
        }

        public bool GetValue()
        {
            return value;
        }

        public bool IsTrue()
        {
            return value;
        }

        public bool IsFalse()
        {
            return !value;
        }

        public override bool IsEquals(Value value)
        {
            if (!value.IsBoolean()) return false;
            return this.value == value.AsBoolean().GetValue();
        }

        public override int CompareTo(Value value)
        {
            if (!value.IsBoolean()) return string.CompareOrdinal(ToString(), value.ToString());
            return this.value.CompareTo(value.AsBoolean().value);
        }

        public override int HashCode()
        {
            return value.GetHashCode();
        }
        public override string Type()
        {
            return "boolean";
        }

        public override ValueString AsString()
        {
            return new ValueString(ToString());
        }

        public override ValueInt AsInt()
        {
            return new ValueInt(value ? 1 : 0);
        }

        public override ValueDecimal AsDecimal()
        {
            return new ValueDecimal(value ? 1 : 0);
        }


        public override ValueBoolean AsBoolean()
        {
            return this;
        }

        public override ValuePattern AsPattern()
        {
            return AsString().AsPattern();
        }

        public override ValueList AsList()
        {
            return new ValueList().AddItem(this);
        }

        public override bool IsBoolean()
        {
            return true;
        }
        
        public override string ToString()
        {
            return value ? "TRUE" : "FALSE";
        }

     }
}