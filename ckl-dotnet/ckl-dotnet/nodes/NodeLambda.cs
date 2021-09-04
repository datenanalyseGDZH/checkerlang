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
    public class NodeLambda : Node
    {
        private List<string> args = new List<string>();
        private List<Node> defs = new List<Node>();
        private Node body;

        private SourcePos pos;

        public NodeLambda(SourcePos pos)
        {
            this.pos = pos;
        }
        
        public void AddArg(string arg)
        {
            args.Add(arg);
            defs.Add(null);
        }

        public void AddArg(string arg, Node defaultValue)
        {
            args.Add(arg);
            defs.Add(defaultValue);
        }

        public void SetBody(Node body)
        {
            if (body is NodeBlock)
            {
                var block = (NodeBlock) body;
                var expressions = block.GetExpressions();
                if (expressions.Count > 0)
                {
                    var lastexpr = expressions[expressions.Count - 1];
                    if (lastexpr is NodeReturn)
                    {
                        expressions[expressions.Count - 1] = ((NodeReturn) lastexpr).GetExpression();
                    }
                }
            }
            else if (body is NodeReturn)
            {
                body = ((NodeReturn) body).GetExpression();
            }
            this.body = body;
        }

        public Value Evaluate(Environment environment)
        {
            var result = new FuncLambda(environment, pos);
            for (var i = 0; i < args.Count; i++)
            {
                result.AddArg(args[i], defs[i]);
            }
            result.SetBody(body);
            return result;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("(lambda ");
            for (var i = 0; i < args.Count; i++)
            {
                result.Append(args[i]);
                if (defs[i] != null)
                {
                    result.Append("=").Append(defs[i]);
                }
                result.Append(", ");
            }
            result.Append(body);
            result.Append(")");
            return result.ToString();
        }

        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            foreach (var def in defs)
            {
                def?.CollectVars(freeVars, boundVars, additionalBoundVars);
            }
            var boundVarsLocal = new HashSet<string>(boundVars);
            foreach (var arg in args)
            {
                boundVarsLocal.Add(arg);
            }
            body.CollectVars(freeVars, boundVarsLocal, additionalBoundVars);
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