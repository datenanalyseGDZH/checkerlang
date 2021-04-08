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
using System.Globalization;

namespace CheckerLang
{
    public class ValueString : Value
    {
        private string value;

        public ValueString(string value)
        {
            this.value = value;
        }

        public string GetValue()
        {
            return value;
        }

        public override bool IsEquals(Value value)
        {
            if (!value.IsString()) return false;
            return this.value == value.AsString().GetValue();
        }

        public override int CompareTo(Value value)
        {
            return string.CompareOrdinal(ToString(), value.ToString());
        }

        public override string Type()
        {
            return "string";
        }

        public override int HashCode()
        {
            return value.GetHashCode();
        }

        public bool Matches(Value pattern)
        {
            return pattern.AsPattern().GetPattern().Match(value).Success;
        }

        public override ValueString AsString()
        {
            return this;
        }

        public override ValueInt AsInt()
        {
            try
            {
                return new ValueInt(long.Parse(value));
            }
            catch (Exception)
            {
                throw new ControlErrorException(new ValueString("ERROR"), "Cannot convert '" + value + "' to int");
            }
        }

        public override ValueDecimal AsDecimal()
        {
            try
            {
                return new ValueDecimal(decimal.Parse(value));
            }
            catch (Exception)
            {
                throw new ControlErrorException(new ValueString("ERROR"), "Cannot convert '" + value + "' to decimal");
            }
        }

        public override ValueBoolean AsBoolean()
        {
            if (value == "1") return ValueBoolean.TRUE;
            if (value == "0") return ValueBoolean.FALSE;
            return ValueBoolean.From(bool.Parse(value));
        }

        public override ValueDate AsDate()
        {
            var fmt = "yyyyMMdd";
            if (value.Length == 10) {
                fmt = "yyyyMMddHH";
            } else if (value.Length == 14) {
                fmt = "yyyyMMddHHmmss";
            }
            try {
                return new ValueDate(DateTime.ParseExact(value, fmt, DateTimeFormatInfo.InvariantInfo));
            } catch (Exception) {
                throw new ControlErrorException(new ValueString("ERROR"), "Cannot convert '" + value + "' to date");
            }
        }

        public override ValuePattern AsPattern()
        {
            return new ValuePattern(value);
        }

        public override ValueList AsList()
        {
            return new ValueList().AddItem(this);
        }

        public override bool IsString()
        {
            return true;
        }
       
        public override string ToString()
        {
            return "'" + value.Replace("\\", "\\\\").Replace("'", "\\'") + "'";
        }

    }
}