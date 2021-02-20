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
    public class FuncLambda : FuncBase
    {
        private Environment lexicalEnv;
        private List<string> argNames = new List<string>();
        private List<Node> defValues = new List<Node>();
        private Node body;

        private SourcePos pos;
        
        public override List<string> GetArgNames()
        {
            return argNames;
        }

        public Node GetBody()
        {
            return body;
        }
        
        public FuncLambda(Environment lexicalEnv, SourcePos pos) : base("lambda")
        {
            this.pos = pos;
            this.lexicalEnv = lexicalEnv;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
        
        public void AddArg(string name)
        {
            argNames.Add(name);
            defValues.Add(null);
        }

        public void AddArg(string name, Node defaultValue)
        {
            argNames.Add(name);
            defValues.Add(defaultValue);
        }

        public void SetBody(Node body)
        {
            this.body = body;
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            var env = lexicalEnv.NewEnv();
            for (var i = 0; i < argNames.Count; i++)
            {
                if (args.HasArg(argNames[i]))
                {
                    env.Put(argNames[i], args.Get(argNames[i]));
                }
                else if (defValues[i] != null)
                {
                    env.Put(argNames[i], defValues[i].Evaluate(env));
                }
                else
                {
                    throw new ControlErrorException("Missing argument " + argNames[i], pos);
                }
            }

            var result = body.Evaluate(env);
            if (result.IsReturn())
            {
                return result.AsReturn().value;
            }
            if (result.IsBreak())
            {
                throw new ControlErrorException("Cannot use break without surrounding for loop", result.AsBreak().pos);
            }
            if (result.IsContinue())
            {
                throw new ControlErrorException("Cannot use continue without surrounding for loop", result.AsContinue().pos);
            }
            return result;
        }
    }
}