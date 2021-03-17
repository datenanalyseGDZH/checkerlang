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
    public class NodeIf : Node
    {
        private List<Node> conditions = new List<Node>();
        private List<Node> expressions = new List<Node>();
        private Node elseExpression;

        private SourcePos pos;

        public NodeIf(SourcePos pos)
        {
            this.pos = pos;
            elseExpression = new NodeLiteralBoolean(true, pos);
        }
        
        public void AddIf(Node condition, Node expression) 
        {
            conditions.Add(condition);
            expressions.Add(expression);
        }

        public void SetElse(Node expression) 
        {
            elseExpression = expression;
        }

        public Value Evaluate(Environment environment)
        {
            for (var i = 0; i < conditions.Count; i++)
            {
                var value = conditions[i].Evaluate(environment);
                if (!value.IsBoolean()) throw new ControlErrorException("Expected boolean condition but got " + value.Type(), pos);
                if (value.AsBoolean().IsTrue()) {
                    return expressions[i].Evaluate(environment);
                }
            }
            return elseExpression.Evaluate(environment);
        }

        public override string ToString() 
        {
            var result = new StringBuilder();
            result.Append("(");
            for (var i = 0; i < conditions.Count; i++) 
            {
                result.Append("if ").Append(conditions[i]);
                result.Append(": ").Append(expressions[i]).Append(" ");
            }
            result.Append("else: ").Append(elseExpression);
            result.Append(")");
            return result.ToString();
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            foreach (var expression in conditions)
            {
                expression.CollectVars(freeVars, boundVars, additionalBoundVars);
            }
            foreach (var expression in expressions)
            {
                expression.CollectVars(freeVars, boundVars, additionalBoundVars);
            }
            elseExpression.CollectVars(freeVars, boundVars, additionalBoundVars);
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