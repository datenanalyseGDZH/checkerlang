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
    public class ValueMap : Value
    {
        private SortedDictionary<Value, Value> value = new SortedDictionary<Value, Value>(new ComparerValue());

        public ValueMap()
        {
            // empty
        }

        public ValueMap(Dictionary<Value, Value> value)
        {
            foreach (var key in value.Keys)
            {
                this.value[key] = value[key];
            }
        }

        public ValueMap AddItem(Value key, Value value)
        {
            this.value[key] = value;
            return this;
        }

        public SortedDictionary<Value, Value> GetValue()
        {
            return value;
        }


        public override bool IsEquals(Value value)
        {
            if (!value.IsMap()) return false;
            var other = value.AsMap().GetValue();
            if (this.value.Count != other.Count) return false;
            foreach (var key in this.value.Keys)
            {
                if (!other.ContainsKey(key))
                {
                    return false;
                }
                if (!this.value[key].IsEquals(other[key]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int CompareTo(Value value)
        {
            return string.CompareOrdinal(ToString(), value.ToString());
        }

        public override int HashCode()
        {
            return value.GetHashCode();
        }

        public override string Type()
        {
            return "map";
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
            foreach (var item in value.Values)
            {
                result.AddItem(item);
            }
            return result;
        }

        public override ValueSet AsSet()
        {
            var result = new ValueSet();
            foreach (var key in value.Keys)
            {
                result.AddItem(key);
            }
            return result;
        }

        public override ValueMap AsMap()
        {
            return this;
        }

        public override bool IsMap()
        {
            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("<<<");
            foreach (var item in value.Keys) {
                builder.Append(item).Append(" => ").Append(value[item]).Append(", ");
            }
            if (builder.Length > "<<<".Length) builder.Remove(builder.Length - ", ".Length, ", ".Length);
            builder.Append(">>>");
            return builder.ToString();
        }

    }
}