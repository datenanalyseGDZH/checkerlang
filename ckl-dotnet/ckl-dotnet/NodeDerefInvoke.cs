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
    public class NodeDerefInvoke : Node
    {
        private Node objectExpr;
        private string member;
        private List<string> names = new List<string>();
        private List<Node> args = new List<Node>();

        private SourcePos pos;

        public NodeDerefInvoke(Node objectExpr, string member, SourcePos pos) {
            this.objectExpr = objectExpr;
            this.member = member;
            this.pos = pos;
        }

        public void AddArg(string name, Node arg)
        {
            this.names.Add(name);
            this.args.Add(arg);
        }

        public Value Evaluate(Environment environment) 
        {
            var val = this.objectExpr.Evaluate(environment);
            if (val.IsObject()) 
            {
                var obj = val.AsObject();
                var exists = obj.value.ContainsKey(this.member);
                while (!exists && obj.value.ContainsKey("_proto_")) {
                    obj = obj.value["_proto_"].AsObject();
                    exists = obj.value.ContainsKey(this.member);
                }
                if (!exists) throw new ControlErrorException("Member " + this.member + " not found", this.pos);
                var fnval = obj.value[this.member];
                if (!fnval.IsFunc()) throw new ControlErrorException("Member " + this.member + " is not a function", this.pos);
                var fn = fnval.AsFunc();
                List<string> names;
                List<Node> args;
                if (val.AsObject().isModule) 
                {
                    names = this.names;
                    args = this.args;
                } 
                else 
                {
                    names = new List<string>();
                    args = new List<Node>();
                    names.Add(null);
                    names.AddRange(this.names);
                    args.Add(new NodeLiteral(val.AsObject(), this.pos));
                    args.AddRange(this.args);
                }
                return Function.invoke(fn, names, args, environment, this.pos);
            }
            if (val is ValueMap) 
            {
                var map = val.AsMap().GetValue();
                var fnval = map[new ValueString(this.member)];
                if (!fnval.IsFunc()) throw new ControlErrorException(this.member + " is not a function", this.pos);
                return Function.invoke(fnval.AsFunc(), this.names, this.args, environment, this.pos);
            }
            throw new ControlErrorException("Cannot deref-invoke " + val.Type(), this.pos);
        }

        public override string ToString() 
        {
            var result = new StringBuilder("(");
            for (var i = 0; i < this.args.Count; i++) 
            {
                result.Append(this.names[i]).Append("=").Append(this.args[i]).Append(", ");
            }
            if (result.Length > 1) result.Remove(result.Length - ", ".Length, ", ".Length);
            return "(" + this.objectExpr + "->" + this.member + "(" + result.ToString() + ")";
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            objectExpr.CollectVars(freeVars, boundVars, additionalBoundVars);
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