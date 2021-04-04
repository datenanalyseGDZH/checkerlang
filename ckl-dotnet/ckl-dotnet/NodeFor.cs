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
using System.IO;
using System.Reflection;
using System.Text;

namespace CheckerLang
{
    public class NodeFor : Node
    {
        private List<string> identifiers = new List<string>();
        private Node expression;
        private Node block;
        private string what;

        private SourcePos pos;

        public NodeFor(List<string> identifiers, Node expression, Node block, string what, SourcePos pos)
        {
            this.identifiers.AddRange(identifiers);
            this.expression = expression;
            this.block = block;
            this.what = what;
            this.pos = pos;
        }

        public Value Evaluate(Environment environment)
        {
            var list = expression.Evaluate(environment);
            if (list.IsInput()) 
            {
                var input = list.AsInput();
                Value result = ValueBoolean.TRUE;
                string line = null;
                try 
                {
                    line = input.ReadLine();
                    while (line != null) 
                    {
                        var value = new ValueString(line);
                        if (identifiers.Count == 1) 
                        {
                            environment.Put(identifiers[0], value);
                        } 
                        else 
                        {
                            var vals = value.AsList().GetValue();
                            for (int i = 0; i < identifiers.Count; i++) 
                            {
                                environment.Put(identifiers[i], vals[i]);
                            }
                        }
                        result = block.Evaluate(environment);
                        if (result.IsBreak())
                        {
                            result = ValueBoolean.TRUE;
                            break;
                        }

                        if (result.IsContinue())
                        {
                            result = ValueBoolean.TRUE;
                            // continue
                        }

                        if (result.IsReturn())
                        {
                            break;
                        }
                        line = input.ReadLine();
                    }
                    if (identifiers.Count == 1) {
                        environment.Remove(identifiers[0]);
                    } else {
                        for (var i = 0; i < identifiers.Count; i++) {
                            environment.Remove(identifiers[i]);
                        }
                    }
                } 
                catch (IOException) 
                {
                    throw new ControlErrorException("Cannot read from input", pos);
                }
                return result;
            }
            if (list.IsList())
            {
                Value result = ValueBoolean.TRUE;
                foreach (var value in list.AsList().GetValue())
                {
                    if (identifiers.Count == 1) 
                    {
                        environment.Put(identifiers[0], value);
                    } 
                    else 
                    {
                        var vals = value.AsList().GetValue();
                        for (int i = 0; i < identifiers.Count; i++) 
                        {
                            environment.Put(identifiers[i], vals[i]);
                        }
                    }
                    result = block.Evaluate(environment);
                    if (result.IsBreak())
                    {
                        result = ValueBoolean.TRUE;
                        break;
                    }

                    if (result.IsContinue())
                    {
                        result = ValueBoolean.TRUE;
                        // continue
                    }

                    if (result.IsReturn())
                    {
                        break;
                    }
                }
                if (identifiers.Count == 1) {
                    environment.Remove(identifiers[0]);
                } else {
                    for (var i = 0; i < identifiers.Count; i++) {
                        environment.Remove(identifiers[i]);
                    }
                }
                return result;
            }
            if (list.IsSet())
            {
                Value result = ValueBoolean.TRUE;
                foreach (var value in list.AsSet().GetValue())
                {
                    if (identifiers.Count == 1) 
                    {
                        environment.Put(identifiers[0], value);
                    } 
                    else 
                    {
                        var vals = value.AsList().GetValue();
                        for (int i = 0; i < identifiers.Count; i++) 
                        {
                            environment.Put(identifiers[i], vals[i]);
                        }
                    }
                    result = block.Evaluate(environment);
                    if (result.IsBreak())
                    {
                        result = ValueBoolean.TRUE;
                        break;
                    }

                    if (result.IsContinue())
                    {
                        result = ValueBoolean.TRUE;
                        // continue
                    }

                    if (result.IsReturn())
                    {
                        break;
                    }
                }
                if (identifiers.Count == 1) {
                    environment.Remove(identifiers[0]);
                } else {
                    for (var i = 0; i < identifiers.Count; i++) {
                        environment.Remove(identifiers[i]);
                    }
                }
                return result;
            }
            if (list.IsMap())
            {
                Value result = ValueBoolean.TRUE;
                foreach (var entry in list.AsMap().GetValue())
                {
                    Value val = null;
                    if (what == "keys") val = entry.Key;
                    else if (what == "values") val = entry.Value;
                    else if (what == "entries") {
                        var vallist = new ValueList();
                        vallist.AddItem(entry.Key);
                        vallist.AddItem(entry.Value);
                        val = vallist;
                    }
                    if (identifiers.Count == 1) 
                    {
                        environment.Put(identifiers[0], val);
                    } 
                    else 
                    {
                        var vals = val.AsList().GetValue();
                        for (int i = 0; i < identifiers.Count; i++) 
                        {
                            environment.Put(identifiers[i], vals[i]);
                        }
                    }
                    result = block.Evaluate(environment);
                    if (result.IsBreak())
                    {
                        result = ValueBoolean.TRUE;
                        break;
                    }

                    if (result.IsContinue())
                    {
                        result = ValueBoolean.TRUE;
                        // continue
                    }

                    if (result.IsReturn())
                    {
                        break;
                    }
                }
                if (identifiers.Count == 1) {
                    environment.Remove(identifiers[0]);
                } else {
                    for (var i = 0; i < identifiers.Count; i++) {
                        environment.Remove(identifiers[i]);
                    }
                }
                return result;
            }
            if (list.IsObject())
            {
                Value result = ValueBoolean.TRUE;
                foreach (var entry in list.AsObject().value)
                {
                    Value val = null;
                    if (what == "keys") val = new ValueString(entry.Key);
                    else if (what == "values") val = entry.Value;
                    else if (what == "entries") {
                        var vallist = new ValueList();
                        vallist.AddItem(new ValueString(entry.Key));
                        vallist.AddItem(entry.Value);
                        val = vallist;
                    }
                    if (identifiers.Count == 1) 
                    {
                        environment.Put(identifiers[0], val);
                    } 
                    else 
                    {
                        var vals = val.AsList().GetValue();
                        for (int i = 0; i < identifiers.Count; i++) 
                        {
                            environment.Put(identifiers[i], vals[i]);
                        }
                    }
                    result = block.Evaluate(environment);
                    if (result.IsBreak())
                    {
                        result = ValueBoolean.TRUE;
                        break;
                    }

                    if (result.IsContinue())
                    {
                        result = ValueBoolean.TRUE;
                        // continue
                    }

                    if (result.IsReturn())
                    {
                        break;
                    }
                }
                if (identifiers.Count == 1) {
                    environment.Remove(identifiers[0]);
                } else {
                    for (var i = 0; i < identifiers.Count; i++) {
                        environment.Remove(identifiers[i]);
                    }
                }
                return result;
            }
            if (list.IsString())
            {
                var str = list.AsString().GetValue();
                Value result = ValueBoolean.TRUE;
                foreach (var value in str)
                {
                    environment.Put(identifiers[0], new ValueString(value.ToString()));
                    result = block.Evaluate(environment);
                    if (result.IsBreak())
                    {
                        result = ValueBoolean.TRUE;
                        break;
                    }

                    if (result.IsContinue())
                    {
                        result = ValueBoolean.TRUE;
                        // continue
                    }

                    if (result.IsReturn())
                    {
                        break;
                    }
                }
                environment.Remove(identifiers[0]);
                return result;
            }
            throw new ControlErrorException("Cannot iterate over " + list, pos);
        }

        public override string ToString() {
            var result = new StringBuilder();
            result.Append("(for ").Append(identifiers.Count == 1 ? identifiers[0] : identifiers.ToString()).Append(" in ").Append(expression).Append(" do ").Append(block).Append(")");
            return result.ToString();
        }
        
        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars)
        {
            expression.CollectVars(freeVars, boundVars, additionalBoundVars);
            var boundVarsLocal = new HashSet<string>(boundVars);
            foreach (var identifier in identifiers)
            {
                boundVarsLocal.Add(identifier);
            }
            block.CollectVars(freeVars, boundVarsLocal, additionalBoundVars);
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