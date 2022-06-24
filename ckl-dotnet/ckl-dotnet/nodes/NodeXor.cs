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
    public class NodeXor : Node
    {
        private Node firstExpr;
        private Node secondExpr;

        private SourcePos pos;

        public NodeXor(SourcePos pos, Node firstExpr, Node secondExpr)
        {
            this.pos = pos;
            this.firstExpr = firstExpr;
            this.secondExpr = secondExpr;
        }

        public Value Evaluate(Environment environment)
        {
            var firstValue = firstExpr.Evaluate(environment);
            var secondValue = secondExpr.Evaluate(environment);
            if (!firstValue.IsBoolean()) throw new ControlErrorException(new ValueString("ERROR"), "Expected boolean but got " + firstValue.Type(), pos);
            if (!secondValue.IsBoolean()) throw new ControlErrorException(new ValueString("ERROR"), "Expected boolean but got " + secondValue.Type(), pos);
            return ValueBoolean.From(firstValue.AsBoolean().GetValue() ^ secondValue.AsBoolean().GetValue());
        }

        public override string ToString()
        {
            return "(" + firstExpr + " xor " + secondExpr + ")";
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            firstExpr.CollectVars(freeVars, boundVars, additionalBoundVars);
            secondExpr.CollectVars(freeVars, boundVars, additionalBoundVars);
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