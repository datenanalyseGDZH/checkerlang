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
    public class NodeDerefAssign : Node
    {
        private Node expression;
        private Node index;
        private Node value;

        private SourcePos pos;

        public NodeDerefAssign(Node expression, Node index, Node value, SourcePos pos) {
            this.expression = expression;
            this.index = index;
            this.value = value;
            this.pos = pos;
        }

        public Value Evaluate(Environment environment) {
            var idx = index.Evaluate(environment);
            var container = expression.Evaluate(environment);
            var value = this.value.Evaluate(environment);
            if (container.IsString())
            {
                var s = container.AsString().GetValue();
                var i = (int) idx.AsInt().GetValue();
                if (i < 0) i = i + s.Length;
                if (i < 0 || i >= s.Length) throw new ControlErrorException(new ValueString("ERROR"),"Index out of bounds " + s + "[" + i + "]", pos);
                return new ValueString(s.Substring(0, i) + value.AsString().GetValue() + s.Substring(i + 1));
            } 
            if (container.IsList())
            {
                var list = container.AsList().GetValue();
                var i = (int) idx.AsInt().GetValue();
                if (i < 0) i = i + list.Count;
                if (i < 0 || i >= list.Count) throw new ControlErrorException(new ValueString("ERROR"),"Index out of bounds" + container + "[" + i + "]", pos);
                list[i] = value;
                return container;
            }
            if (container.IsMap())
            {
                var map = container.AsMap().GetValue();
                map[idx] = value;
                return container;
            }
            if (container.IsObject())
            {
                var map = container.AsObject().value;
                var member = idx.AsString().GetValue();
                map[member] = value;
                return container;
            }
            throw new ControlErrorException(new ValueString("ERROR"),"Cannot dereference value " + container, pos);
        }

        public override string ToString() {
            return "(" + expression + "[" + index + "] = " + value + ")";
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            expression.CollectVars(freeVars, boundVars, additionalBoundVars);
            index.CollectVars(freeVars, boundVars, additionalBoundVars);
            value.CollectVars(freeVars, boundVars, additionalBoundVars);
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