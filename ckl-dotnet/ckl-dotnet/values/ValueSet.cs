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
using System.Text;

namespace CheckerLang
{
    public class ValueSet : Value
    {
        private HashSet<Value> value = new HashSet<Value>(new EqualityComparerValue());

        public ValueSet()
        {
            // empty
        }

        public ValueSet(HashSet<Value> value)
        {
            foreach (var item in value)
            {
                this.value.Add(item);
            }
        }

        public ValueSet(ValueSet value)
        {
            foreach (var item in value.value)
            {
                this.value.Add(item);
            }
        }

        public ValueSet AddItem(Value item)
        {
            value.Add(item);
            return this;
        }

        public ValueSet AddItems(IEnumerable<Value> items)
        {
            foreach (var item in items)
            {
                value.Add(item);
            }
            return this;
        }
        
        public HashSet<Value> GetValue()
        {
            return value;
        }


        public override bool IsEquals(Value value)
        {
            if (!value.IsSet()) return false;
            var other = value.AsSet().GetValue(); 
            if (this.value.Count != other.Count) return false;
            foreach (var item in this.value)
            {
                if (!other.Contains(item)) return false;
            }
            return true;
        }

        public override int CompareTo(Value value)
        {
            return string.CompareOrdinal(ToString(), value.ToString());
        }

        public override string Type()
        {
            return "set";
        }
 
        public override int HashCode()
        {
            return value.GetHashCode();
        }

        public override ValueString AsString()
        {
            return new ValueString(ToString());
        }

        public override ValueInt AsInt()
        {
            return new ValueInt(value.Count);
        }

        public override ValueBoolean AsBoolean()
        {
            return ValueBoolean.From(value.Count > 0);
        }

        public override ValueList AsList()
        {
            var result = new ValueList();
            foreach (var item in value)
            {
                result.AddItem(item);
            }
            return result;
        }

        public override ValueSet AsSet()
        {
            return this;
        }

        public override bool IsSet()
        {
            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("<<");
            SortedSet<Value> value_ = new SortedSet<Value>(value, new ComparerValue());
            foreach (var item in value_) {
                builder.Append(item).Append(", ");
            }
            if (builder.Length > "<<".Length) builder.Remove(builder.Length - ", ".Length, ", ".Length);
            builder.Append(">>");
            return builder.ToString();
        }

    }
}