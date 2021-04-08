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
    public class NodeDefDestructuring : Node
    {
        private List<string> identifiers = new List<string>();
        private Node expression;
        
        private string info;
        private SourcePos pos;
        
        public NodeDefDestructuring(List<string> identifiers, Node expression, string info, SourcePos pos)
        {
            this.identifiers.AddRange(identifiers);
            this.expression = expression;
            this.info = info;
            this.pos = pos;
        }

        public Value Evaluate(Environment environment)
        {
            var value = expression.Evaluate(environment);
            value.info = info;
            if (value.IsList() || value.IsSet())
            {
                var list = value.AsList().GetValue();
                Value result = ValueNull.NULL;
                for (var i = 0; i < identifiers.Count; i++)
                {
                    if (i < list.Count)
                    {
                        environment.Put(identifiers[i], list[i]);
                        if (list[i].IsFunc() && typeof(FuncLambda) == value.GetType()) ((FuncLambda) value).SetName(identifiers[i]);
                        result = list[i];
                    }
                    else
                    {
                        environment.Put(identifiers[i], ValueNull.NULL);
                        result = ValueNull.NULL;
                    }
                }
                return result;
            }
            throw new ControlErrorException(new ValueString("ERROR"),"Destructuring def expected list or set but got " + value.Type(), pos);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("(def [");
            foreach (var identifier in identifiers)
            {
                result.Append(identifier).Append(", ");
            }
            if (identifiers.Count > 0) result.Remove(result.Length - 2, 2);
            result.Append("] = ");
            result.Append(expression);
            result.Append(")");
            return result.ToString();
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            expression.CollectVars(freeVars, boundVars, additionalBoundVars);
            foreach (var identifier in identifiers)
            {
                boundVars.Add(identifier);
            }
        }

        public List<string> GetIdentifiers()
        {
            return identifiers;
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