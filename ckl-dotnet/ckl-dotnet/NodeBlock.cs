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
using System.Threading;

namespace CheckerLang
{
    public class NodeBlock : Node
    {
        private List<Node> expressions = new List<Node>();
        private List<Node> catchtypes = new List<Node>();
        private List<Node> catchexprs = new List<Node>();
        private List<Node> finallyexprs = new List<Node>();

        private SourcePos pos;
        
        public NodeBlock(SourcePos pos)
        {
            this.pos = pos;
        }

        public List<Node> GetExpressions()
        {
            return expressions;
        }
        
        public void Add(Node expression)
        {
            expressions.Add(expression);
        }

        public void AddFinally(Node expression)
        {
            finallyexprs.Add(expression);
        }

        public void AddCatch(Node type, Node expression)
        {
            catchtypes.Add(type);
            catchexprs.Add(expression);
        }

        public bool HasFinally()
        {
            return finallyexprs.Count > 0;
        }

        public bool HasCatch()
        {
            return catchexprs.Count > 0;
        }
        
        public Value Evaluate(Environment environment)
        {
            Value result = ValueBoolean.TRUE;
            try
            {
                foreach (var expression in expressions)
                {
                    result = expression.Evaluate(environment);
                    if (result.IsReturn())
                    {
                        break;
                    }

                    if (result.IsContinue())
                    {
                        break;
                    }

                    if (result.IsBreak())
                    {
                        break;
                    }
                }
            }
            catch (CheckerlangException e)
            {
                for (var i = 0; i < catchtypes.Count; i++)
                {
                    var err = catchtypes[i];
                    if (err == null || e.GetErrorType().IsEquals(err.Evaluate(environment))) {
                        return catchexprs[i].Evaluate(environment);
                    }
                }

                throw;
            }
            finally
            {
                foreach (var expression in finallyexprs)
                {
                    expression.Evaluate(environment);
                }
            }
            return result;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("(block ");
            foreach (var expression in expressions)
            {
                result.Append(expression).Append(", ");
            }
            if (expressions.Count > 0) result.Remove(result.Length - 2, 2);
            for (var i = 0; i < catchtypes.Count; i++) {
                result = result.Append(" catch ").Append(catchtypes[i]).Append(" ").Append(catchexprs[i]).Append(" ");
            }
            if (catchtypes.Count > 0) result.Remove(result.Length - 1, 1);
            if (finallyexprs.Count > 0)
            {
                result.Append(" finally ");
                foreach (var expression in finallyexprs)
                {
                    result.Append(expression).Append(", ");
                }
                result.Remove(result.Length - 2, 2);
            }
            result.Append(")");
            return result.ToString();
        }

        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            var additionalBoundVarsLocal = new SortedSet<string>(additionalBoundVars);
            foreach (var expression in expressions)
            {
                if (expression is NodeDef def)
                {
                    additionalBoundVarsLocal.Add(def.GetIdentifier());
                }
                if (expression is NodeDefDestructuring defdest)
                {
                    foreach (var identifier in defdest.GetIdentifiers())
                    {
                        additionalBoundVarsLocal.Add(identifier);
                    }
                }
            }
            foreach (var expression in finallyexprs)
            {
                if (expression is NodeDef def)
                {
                    additionalBoundVarsLocal.Add(def.GetIdentifier());
                }
                if (expression is NodeDefDestructuring defdest)
                {
                    foreach (var identifier in defdest.GetIdentifiers())
                    {
                        additionalBoundVarsLocal.Add(identifier);
                    }
                }
            }
            foreach (var expression in catchexprs)
            {
                if (expression is NodeDef def)
                {
                    additionalBoundVarsLocal.Add(def.GetIdentifier());
                }
                if (expression is NodeDefDestructuring defdest)
                {
                    foreach (var identifier in defdest.GetIdentifiers())
                    {
                        additionalBoundVarsLocal.Add(identifier);
                    }
                }
            }
            foreach (var expression in expressions)
            {
                if (expression is NodeDef || expression is NodeDefDestructuring)
                {
                    expression.CollectVars(freeVars, boundVars, additionalBoundVarsLocal);
                }
                else
                {
                    expression.CollectVars(freeVars, boundVars, additionalBoundVars);
                }
            }
            foreach (var expression in finallyexprs)
            {
                if (expression is NodeDef || expression is NodeDefDestructuring)
                {
                    expression.CollectVars(freeVars, boundVars, additionalBoundVarsLocal);
                }
                else
                {
                    expression.CollectVars(freeVars, boundVars, additionalBoundVars);
                }
            }

            for (var i = 0; i < catchtypes.Count; i++)
            {
                var err = catchtypes[i];
                var expression = catchexprs[i];
                
                if (expression is NodeDef || expression is NodeDefDestructuring)
                {
                    expression.CollectVars(freeVars, boundVars, additionalBoundVarsLocal);
                }
                else
                {
                    expression.CollectVars(freeVars, boundVars, additionalBoundVars);
                }

                err.CollectVars(freeVars, boundVars, additionalBoundVars);
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