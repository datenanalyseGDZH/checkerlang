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
    public class NodeIn : Node
    {
        private Node expression;
        private Node list;

        private SourcePos pos;

        public NodeIn(Node expression, Node list, SourcePos pos) {
            this.expression = expression;
            this.list = list;
            this.pos = pos;
        }

        public Value Evaluate(Environment environment) {
            var value = expression.Evaluate(environment);
            var container = list.Evaluate(environment);
            if (container.IsList())
            {
                var items = container.AsList().GetValue();
                foreach (var item in items) {
                    if (value.IsEquals(item)) return ValueBoolean.TRUE;
                }
            }
            else if (container.IsSet())
            {
                return ValueBoolean.From(container.AsSet().GetValue().Contains(value));
            }
            else if (container.IsMap())
            {
                return ValueBoolean.From(container.AsMap().GetValue().ContainsKey(value));
            }
            else if (container.IsString())
            {
                return ValueBoolean.From(container.AsString().GetValue().Contains(value.AsString().GetValue()));
            }
            return ValueBoolean.FALSE;
        }

        public override string ToString()
        {
            return "(" + expression + " in " + list + ")";
        }
    
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            expression.CollectVars(freeVars, boundVars, additionalBoundVars);
            list.CollectVars(freeVars, boundVars, additionalBoundVars);
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