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
    public class NodeFor : Node
    {
        private string identifier;
        private Node expression;
        private Node block;

        private SourcePos pos;

        public NodeFor(string identifier, Node expression, Node block, SourcePos pos)
        {
            this.identifier = identifier;
            this.expression = expression;
            this.block = block;
            this.pos = pos;
        }

        public Value Evaluate(Environment environment)
        {
            var list = expression.Evaluate(environment);
            if (list.IsList() || list.IsSet() || list.IsMap())
            {
                var values = list.AsList().GetValue();
                Value result = ValueBoolean.TRUE;
                foreach (var value in values)
                {
                    environment.Put(identifier, value);
                    result = block.Evaluate(environment);
                    environment.Remove(identifier);
                    if (result.IsBreak())
                    {
                        result = ValueBoolean.TRUE;
                        break;
                    }

                    if (result.IsContinue())
                    {
                        result = ValueBoolean.TRUE;
                        // continue
                    }

                    if (result.IsReturn())
                    {
                        break;
                    }
                }
                return result;
            }
            if (list.IsString())
            {
                var str = list.AsString().GetValue();
                Value result = ValueBoolean.TRUE;
                foreach (var value in str)
                {
                    if (environment.IsDefined(identifier)) throw new ControlErrorException("Symbol " + identifier + " already defined", pos);
                    environment.Put(identifier, new ValueString(value.ToString()));
                    result = block.Evaluate(environment);
                    environment.Remove(identifier);
                    if (result.IsBreak())
                    {
                        result = ValueBoolean.TRUE;
                        break;
                    }

                    if (result.IsContinue())
                    {
                        result = ValueBoolean.TRUE;
                        // continue
                    }

                    if (result.IsReturn())
                    {
                        break;
                    }
                }
                return result;
            }
            throw new ControlErrorException("Cannot iterate over " + list, pos);
        }

        public override string ToString() {
            var result = new StringBuilder();
            result.Append("(for ").Append(identifier).Append(" in ").Append(expression).Append(" do ").Append(block).Append(")");
            return result.ToString();
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            expression.CollectVars(freeVars, boundVars, additionalBoundVars);
            var boundVarsLocal = new HashSet<string>(boundVars);
            boundVarsLocal.Add(identifier);
            block.CollectVars(freeVars, boundVarsLocal, additionalBoundVars);
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