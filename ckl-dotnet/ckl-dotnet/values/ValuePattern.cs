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
using System.Text.RegularExpressions;

namespace CheckerLang
{
    public class ValuePattern : Value
    {
        private string value;
        private Regex pattern;
        
        private static Dictionary<string, Regex> cache = new Dictionary<string, Regex>(); 

        public ValuePattern(string value)
        {
            this.value = value;
            if (cache.ContainsKey(value)) pattern = cache[value];
            else
            {
                pattern = new Regex(value);
                cache[value] = pattern;
            }
        }

        public string GetValue()
        {
            return value;
        }

        public Regex GetPattern()
        {
            return pattern;
        }

        public override bool IsEquals(Value value)
        {
            if (!value.IsPattern()) return false;
            return this.value == value.AsPattern().GetValue();
        }

        public override int CompareTo(Value value)
        {
            if (!value.IsPattern()) return string.CompareOrdinal(ToString(), value.ToString());
            return string.CompareOrdinal(this.value, value.AsPattern().value);
        }

        public override int HashCode()
        {
            return value.GetHashCode();
        }

        public override string Type()
        {
            return "pattern";
        }

        public override ValueString AsString()
        {
            return new ValueString(value);
        }

        public override ValuePattern AsPattern()
        {
            return this;
        }

        public override ValueList AsList()
        {
            return new ValueList().AddItem(this);
        }

        public override bool IsPattern()
        {
            return true;
        }
        
        public override string ToString()
        {
            return "//" + value + "//";
        }

    }
}