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
using System.Linq;
using System.Text;

namespace CheckerLang
{
    public class NodeMap : Node
    {
        private List<Node> keys = new List<Node>();
        private List<Node> values = new List<Node>();

        private SourcePos pos;

        public NodeMap(SourcePos pos)
        {
            this.pos = pos;
        }

        public void AddKeyValue(Node key, Node value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public Value Evaluate(Environment environment)
        {
            var result = new ValueMap();
            for (var i = 0; i < keys.Count; i++) 
            {
                result.AddItem(keys[i].Evaluate(environment), values[i].Evaluate(environment));
            }
            return result;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append("<<<");
            for (var i = 0; i < keys.Count; i++)
            {
                builder.Append(keys[i]).Append(" => ").Append(values[i]).Append(", ");
            }
            if (builder.Length > 3) builder.Remove(builder.Length - ", ".Length, ", ".Length);
            builder.Append(">>>");
            return builder.ToString();
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            foreach (var item in keys)
            {
                item.CollectVars(freeVars, boundVars, additionalBoundVars);
            }
            foreach (var item in values)
            {
                item.CollectVars(freeVars, boundVars, additionalBoundVars);
            }
        }
        
        public SourcePos GetSourcePos()
        {
            return pos;
        }

        public bool IsLiteral()
        {
            return false;
        }
    }
}