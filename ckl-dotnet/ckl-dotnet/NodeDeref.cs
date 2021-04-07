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

namespace CheckerLang
{
    public class NodeDeref : Node
    {
        private Node expression;
        private Node index;

        private SourcePos pos;

        public NodeDeref(Node expression, Node index, SourcePos pos) {
            this.expression = expression;
            this.index = index;
            this.pos = pos;
        }

        public Value Evaluate(Environment environment) {
            var idx = index.Evaluate(environment);
            var value = expression.Evaluate(environment);
            if (value.IsNull()) return ValueNull.NULL;
            if (value.IsString())
            {
                var s = value.AsString().GetValue();
                var i = (int) idx.AsInt().GetValue();
                if (i < 0) i = i + s.Length;
                if (i < 0 || i >= s.Length) throw new ControlErrorException("Index out of bounds " + i, pos);
                return new ValueString(s.Substring(i, 1));
            } 
            if (value.IsList())
            {
                var list = value.AsList().GetValue();
                var i = (int) idx.AsInt().GetValue();
                if (i < 0) i = i + list.Count;
                if (i < 0 || i >= list.Count) throw new ControlErrorException("Index out of bounds " + i, pos);
                return list[i];
            }
            if (value.IsMap())
            {
                var map = value.AsMap().GetValue();
                if (!map.ContainsKey(idx)) throw new ControlErrorException("Map does not contain key " + idx, pos);
                return map[idx];
            }
            if (value.IsObject())
            {
                var obj = value.AsObject();
                var member = idx.AsString().GetValue();
                var exists = obj.value.ContainsKey(member);
                while (!exists && obj.value.ContainsKey("_proto_")) {
                    obj = obj.value["_proto_"].AsObject();
                    exists = obj.value.ContainsKey(member);
                }
                if (!exists) return ValueNull.NULL;
                return obj.value[member];
            }
            throw new ControlErrorException("Cannot dereference value " + value, pos);
        }

        public override string ToString() {
            return "(" + expression + "[" + index + "])";
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            expression.CollectVars(freeVars, boundVars, additionalBoundVars);
            index.CollectVars(freeVars, boundVars, additionalBoundVars);
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