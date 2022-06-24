/*  Copyright (c) 2022 Damian Brunold, Gesundheitsdirektion Kanton Zürich

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
    public class NodeClass : Node
    {
        private string identifier;
        private List<Node> members = new List<Node>();
        
        private string info;
        private SourcePos pos;
        
        public NodeClass(string identifier, string info, SourcePos pos)
        {
            this.identifier = identifier;
            this.info = info;
            this.pos = pos;
        }

        public void AddMember(Node member) 
        {
            members.Add(member);
        }

        public Value Evaluate(Environment environment)
        {
            var result = new ValueObject();
            foreach (var member in members)
            {
                NodeDef def = (NodeDef) member;
                result.AddItem(def.GetIdentifier(), def.Evaluate(environment));
            }
            environment.Put(identifier, result);
            return result;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("(class ");
            result.Append(identifier);
            result.Append(" ");
            foreach (var member in members)
            {
                result.Append(member);
                result.Append(" ");
            }
            if (result.Length > 1) result.Remove(result.Length - " ".Length, " ".Length);
            result.Append(")");
            return result.ToString();
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            foreach (var member in members) 
            {
                member.CollectVars(freeVars, boundVars, additionalBoundVars);
            }
            if (!boundVars.Contains(identifier)) 
            {
                boundVars.Add(identifier);
            }
        }

        public string GetIdentifier()
        {
            return identifier;
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