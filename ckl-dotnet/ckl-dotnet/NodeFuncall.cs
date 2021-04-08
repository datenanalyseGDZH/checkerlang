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
    public class NodeFuncall : Node
    {
        private Node func;
        private List<string> names = new List<string>();
        private List<Node> args = new List<Node>();

        private SourcePos pos;

        public NodeFuncall(Node func, SourcePos pos)
        {
            this.func = func;
            this.pos = pos;
        }

        public int ArgCount()
        {
            return args.Count;
        }

        public Node GetFunc()
        {
            return func;
        }

        public string GetArgName(int i)
        {
            return names[i];
        }

        public Node GetArg(int i)
        {
            return args[i];
        }

        public void AddArg(string name, Node arg)
        {
            names.Add(name);
            args.Add(arg);
        }

        public Value Evaluate(Environment environment)
        {
            var fn = func.Evaluate(environment);
            if (!fn.IsFunc()) throw new ControlErrorException(new ValueString("ERROR"), "Expected function but got " + fn.Type(), pos);
            return Function.invoke(fn.AsFunc(), this.names, this.args, environment, this.pos);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var expression in args) {
                builder.Append(expression).Append(", ");
            }
            if (builder.Length > 1) builder.Remove(builder.Length - ", ".Length, ", ".Length);
            return "(" + func + " " + builder + ")";
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            func.CollectVars(freeVars, boundVars, additionalBoundVars);
            foreach (var arg in args)
            {
                arg.CollectVars(freeVars, boundVars, additionalBoundVars);
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