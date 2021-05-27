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
    public class NodeSetComprehensionProduct : Node
    {
        private Node valueExpr;
        private string identifier1;
        private Node listExpr1;
        private string identifier2;
        private Node listExpr2;
        private Node conditionExpr;

        private SourcePos pos;
        
        public NodeSetComprehensionProduct(Node valueExpr, string identifier1, Node listExpr1, string identifier2, Node listExpr2, SourcePos pos)
        {
            this.valueExpr = valueExpr;
            this.identifier1 = identifier1;
            this.listExpr1 = listExpr1;
            this.identifier2 = identifier2;
            this.listExpr2 = listExpr2;
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
            var list1 = AsList.From(listExpr1.Evaluate(environment));
            var list2 = AsList.From(listExpr2.Evaluate(environment));
            foreach (var value1 in list1.GetValue()) 
            {
                localEnv.Put(identifier1, value1);
                foreach (var value2 in list2.GetValue())
                {
                    localEnv.Put(identifier2, value2);
                    var value = valueExpr.Evaluate(localEnv);
                    if (conditionExpr != null)
                    {
                        var condition = conditionExpr.Evaluate(localEnv);
                        if (!condition.IsBoolean())
                        {
                            throw new ControlErrorException(new ValueString("ERROR"),
                                "Condition must be boolean but got " + condition.Type(),
                                pos);
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
            }

            return result;
        }

        public override string ToString()
        {
            return "<<" + valueExpr + 
                   " for " + identifier1 + " in " + listExpr1 + 
                   " for " + identifier2 + " in " + listExpr2 + 
                   (conditionExpr == null ? "" : (" if " + conditionExpr)) + ">>";
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            var boundVarsLocal = new HashSet<string>(boundVars);
            boundVarsLocal.Add(identifier1);
            boundVarsLocal.Add(identifier2);
            valueExpr.CollectVars(freeVars, boundVarsLocal, additionalBoundVars);
            listExpr1.CollectVars(freeVars, boundVars, additionalBoundVars);
            listExpr2.CollectVars(freeVars, boundVars, additionalBoundVars);
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