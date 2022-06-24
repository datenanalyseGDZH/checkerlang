/*  Copyright (c) 2022 Damian Brunold, Gesundheitsdirektion Kanton Zürich

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
    public class NodeDerefSlice : Node
    {
        private Node expression;
        private Node start;
        private Node end;

        private SourcePos pos;

        public NodeDerefSlice(Node expression, Node start, Node end, SourcePos pos) 
        {
            this.expression = expression;
            this.start = start;
            this.end = end;
            this.pos = pos;
        }

        public Value Evaluate(Environment environment) 
        {
            Value value = this.expression.Evaluate(environment);
            Value start = this.start.Evaluate(environment);
            Value end = this.end != null ? this.end.Evaluate(environment) : null;
            if (value == ValueNull.NULL) return ValueNull.NULL;
            if (value.IsString()) 
            {
                var s = value.AsString().GetValue();
                int idxStart = (int) start.AsInt().GetValue();
                int idxEnd = end == null ? s.Length : (int) end.AsInt().GetValue();
                if (idxStart < 0) idxStart = idxStart + s.Length;
                if (idxEnd < 0) idxEnd = idxEnd + s.Length;
                if (idxStart < 0) idxStart = 0;
                if (idxEnd > s.Length) idxEnd = s.Length;
                return new ValueString(s.Substring(idxStart, idxEnd - idxStart));
            }
            if (value.IsList()) 
            {
                var list = value.AsList().GetValue();
                int idxStart = (int) start.AsInt().GetValue();
                int idxEnd = end == null ? list.Count : (int) end.AsInt().GetValue();
                if (idxStart < 0) idxStart = idxStart + list.Count;
                if (idxEnd < 0) idxEnd = idxEnd + list.Count;
                if (idxStart < 0) idxStart = 0;
                if (idxEnd > list.Count) idxEnd = list.Count;
                var result = new ValueList();
                for (int i = idxStart; i < idxEnd; i++) {
                    result.AddItem(list[i]);
                }
                return result;
            }
            throw new ControlErrorException(new ValueString("ERROR"), "Cannot slice value " + value, this.pos);
        }

        public override string ToString() 
        {
            return "(" + expression + "[" + start + " to " + end + "])";
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            expression.CollectVars(freeVars, boundVars, additionalBoundVars);
            start.CollectVars(freeVars, boundVars, additionalBoundVars);
            end.CollectVars(freeVars, boundVars, additionalBoundVars);
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