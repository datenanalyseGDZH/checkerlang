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
    public class NodeSetComprehension : Node
    {
        private Node valueExpr;
        private string identifier;
        private Node listExpr;
        private string what;
        private Node conditionExpr;

        private SourcePos pos;
        
        public NodeSetComprehension(Node valueExpr, string identifier, Node listExpr, string what, SourcePos pos)
        {
            this.valueExpr = valueExpr;
            this.identifier = identifier;
            this.listExpr = listExpr;
            this.what = what;
            this.pos = pos;
        }

        public void SetCondition(Node conditionExpr)
        {
            this.conditionExpr = conditionExpr;
        }
        
        public Value Evaluate(Environment environment)
        {
            var result = new ValueSet();
            var localEnv = environment.NewEnv();
            var list = AsList.From(listExpr.Evaluate(environment), what);
            foreach (var listValue in list.GetValue())
            {
                localEnv.Put(identifier, listValue);
                var value = valueExpr.Evaluate(localEnv);
                if (conditionExpr != null)
                {
                    var condition = conditionExpr.Evaluate(localEnv);
                    if (!condition.IsBoolean())
                    {
                        throw new ControlErrorException(new ValueString("ERROR"), "Condition must be boolean but got " + condition.Type(), pos);
                    }
                    if (condition.AsBoolean().GetValue())
                    {
                        result.AddItem(value);
                    }
                }
                else
                {
                    result.AddItem(value);
                }
            }
            return result;
        }

        public override string ToString()
        {
            return "<<" + valueExpr + " for " + identifier + " in " + (what != null ? what + " " : "") + listExpr + (conditionExpr == null ? "" : (" if " + conditionExpr)) + ">>";
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            var boundVarsLocal = new HashSet<string>(boundVars);
            boundVarsLocal.Add(identifier);
            valueExpr.CollectVars(freeVars, boundVarsLocal, additionalBoundVars);
            listExpr.CollectVars(freeVars, boundVars, additionalBoundVars);
            conditionExpr?.CollectVars(freeVars, boundVarsLocal, additionalBoundVars);
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