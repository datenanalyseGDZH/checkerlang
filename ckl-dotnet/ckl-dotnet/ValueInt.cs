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

namespace CheckerLang
{
    public class ValueInt : Value
    {
        private readonly long value;

        public ValueInt(long value)
        {
            this.value = value;
        }

        public long GetValue()
        {
            return value;
        }

        public override bool IsEquals(Value value)
        {
            if (!value.IsNumerical()) return false;
            if (value.IsDecimal()) return AsDecimal().IsEquals(value);
            return this.value == value.AsInt().GetValue();
        }

        public override int CompareTo(Value value)
        {
            if (!value.IsNumerical()) return ToString().CompareTo(value.ToString());
            if (value.IsDecimal()) return AsDecimal().CompareTo(value);
            return this.value.CompareTo(value.AsInt().value);
        }

        public override int HashCode()
        {
            return value.GetHashCode();
        }

        public override string Type()
        {
            return "int";
        }

        public override ValueString AsString()
        {
            return new ValueString(ToString());
        }


        public override ValueInt AsInt()
        {
            return this;
        }

        public override ValueDecimal AsDecimal()
        {
            return new ValueDecimal(value);
        }

        public override ValueBoolean AsBoolean()
        {
            return ValueBoolean.From(value != 0);
        }

        public override ValueDate AsDate()
        {
            return new ValueDate(DateTime.FromOADate(value));
        }

        public override ValueList AsList()
        {
            return new ValueList().AddItem(this);
        }

        public override bool IsInt()
        {
            return true;
        }
        
        public override string ToString()
        {
            return value.ToString();
        }
 
    }
}