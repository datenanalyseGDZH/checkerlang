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
using System.Collections.Generic;
using System.Text;

namespace CheckerLang
{
    public class ValueList : Value
    {
        private List<Value> value = new List<Value>();

        public ValueList()
        {
            // empty
        }

        public ValueList(List<Value> value)
        {
            this.value.AddRange(value);
        }

        public ValueList(ValueList value)
        {
            this.value.AddRange(value.value);
        }

        public ValueList AddItems(IEnumerable<Value> items)
        {
            foreach (var item in items)
            {
                value.Add(item);
            }
            return this;
        }

        public ValueList AddItem(Value item)
        {
            value.Add(item);
            return this;
        }

        public List<Value> GetValue()
        {
            return value;
        }


        public override bool IsEquals(Value value)
        {
            if (!value.IsList()) return false;
            if (this.value.Count != value.AsList().GetValue().Count) return false;
            for (var i = 0; i < this.value.Count; i++)
            {
                if (!this.value[i].IsEquals(value.AsList().GetValue()[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int CompareTo(Value value)
        {
            if (!value.IsList()) return string.CompareOrdinal(ToString(), value.ToString());
            var lst = value.AsList().GetValue();
            for (var i = 0; i < Math.Min(GetValue().Count, lst.Count); i++)
            {
                var cmp = GetValue()[i].CompareTo(lst[i]);
                if (cmp != 0) return cmp;
            }

            if (GetValue().Count < lst.Count) return -1;
            if (GetValue().Count > lst.Count) return 1;
            return 0;
        }

        public override int HashCode()
        {
            return value.GetHashCode();
        }

        public override string Type()
        {
            return "list";
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
            return this;
        }

        public override ValueSet AsSet()
        {
            var result = new ValueSet();
            foreach (var item in value)
            {
                result.AddItem(item);
            }
            return result;
        }

        public override ValueMap AsMap()
        {
            var result = new ValueMap();
            foreach (var item in value)
            {
                result.AddItem(item.AsList().GetValue()[0], item.AsList().GetValue()[1]);
            }
            return result;
        }

        public override ValueObject AsObject()
        {
            var result = new ValueObject();
            foreach (var item in value)
            {
                result.AddItem(item.AsList().GetValue()[0].AsString().GetValue(), item.AsList().GetValue()[1]);
            }
            return result;
        }

        public override bool IsList()
        {
            return true;
        }
        
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[");
            foreach (var item in value) {
                builder.Append(item).Append(", ");
            }
            if (builder.Length > 1) builder.Remove(builder.Length - ", ".Length, ", ".Length);
            builder.Append("]");
            return builder.ToString();
        }

    }
}