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
    public class ValueObject : Value
    {
        public Dictionary<string, Value> value = new Dictionary<string, Value>();
        public bool isModule = false;

        public ValueObject AddItem(string key, Value value)
        {
            this.value[key] = value;
            return this;
        }

        public override bool IsEquals(Value value)
        {
            if (!value.IsObject()) return false;
            Dictionary<string, Value> other = value.AsObject().value;
            if (this.value.Count != other.Count) return false;
            foreach (string key in this.value.Keys) {
                if (!other.ContainsKey(key)) {
                    return false;
                }
                if (!this.value[key].IsEquals(other[key])) {
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
            return "object";
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
                result.AddItem(new ValueString(key));
            }
            return result;
        }

        public override ValueMap AsMap()
        {
            var result = new ValueMap();
            foreach (var entry in value) {
                result.AddItem(new ValueString(entry.Key), entry.Value);
            }
            return result;
        }

        public override ValueObject AsObject()
        {
            return this;
        }

        public override bool IsObject()
        {
            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("<*");
            foreach (var item in value.Keys)
            {
                if (item.StartsWith("_")) continue;
                builder.Append(item).Append("=").Append(value[item]).Append(", ");
            }
            if (builder.Length > 2) builder.Remove(builder.Length - ", ".Length, ", ".Length);
            builder.Append("*>");
            return builder.ToString();
        }

    }
}