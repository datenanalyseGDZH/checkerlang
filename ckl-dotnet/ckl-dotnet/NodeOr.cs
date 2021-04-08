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
    public class NodeOr : Node
    {
        private readonly List<Node> expressions = new List<Node>();

        private SourcePos pos;

        public NodeOr(SourcePos pos)
        {
            this.pos = pos;
        }

        public NodeOr(Node expression, SourcePos pos)
        {
            expressions.Add(expression);
            this.pos = pos;
        }

        public void AddOrClause(Node expression)
        {
            expressions.Add(expression);
        }

        public Node GetSimplified()
        {
            if (expressions.Count == 1)
            {
                return expressions[0];
            }
            return this;
        }

        public Value Evaluate(Environment environment)
        {
            foreach (var expression in expressions) {
                var value = expression.Evaluate(environment);
                if (!value.IsBoolean()) throw new ControlErrorException(new ValueString("ERROR"), "Expected boolean but got " + value.Type(), pos);
                if (value.AsBoolean().IsTrue())
                {
                    return ValueBoolean.TRUE;
                }
            }
            return ValueBoolean.FALSE;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("(");
            foreach (var expression in expressions) {
                result.Append(expression).Append(" or ");
            }
            if (result.Length > 1) result.Remove(result.Length - " or ".Length, " or ".Length);
            result.Append(")");
            return result.ToString();
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            foreach (var expression in expressions)
            {
                expression.CollectVars(freeVars, boundVars, additionalBoundVars);
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