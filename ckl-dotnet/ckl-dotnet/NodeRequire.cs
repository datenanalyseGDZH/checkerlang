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

using System;
using System.Collections.Generic;

namespace CheckerLang
{
    public class NodeRequire : Node
    {
        private string modulespec;
        private string name;
        private bool unqualified;

        private SourcePos pos;

        public NodeRequire(string modulespec, string name, bool unqualified, SourcePos pos) {
            this.modulespec = modulespec;
            this.name = name;
            this.unqualified = unqualified;
            this.pos = pos;
        }

        public Value Evaluate(Environment environment) {
            var modules = environment.GetModules();
            // resolve module file, identifier and name
            var modulefile = modulespec;
            if (!modulefile.EndsWith(".ckl")) modulefile += ".ckl";
            string moduleidentifier;
            var modulename = this.name;
            var parts = modulespec.Split('/');
            var name = parts[parts.Length - 1];
            if (name.EndsWith(".ckl")) name = name.Substring(0, name.Length - 4);
            moduleidentifier = name;
            if (modulename == null) modulename = name;
            environment.PushModuleStack(moduleidentifier, pos);

            // lookup or read module
            Environment moduleEnv = null;
            if (modules.ContainsKey(moduleidentifier)) {
                moduleEnv = modules[moduleidentifier];
            } else {
                moduleEnv = environment.GetBase().NewEnv();
                string modulesrc = ModuleLoader.LoadModule(modulefile, pos);
                Node node = null;
                try {
                    node = Parser.Parse(modulesrc, modulefile);
                } catch (Exception) {
                    throw new ControlErrorException("Cannot parse module " + moduleidentifier, pos);
                }
                node.Evaluate(moduleEnv);
                modules[moduleidentifier] = moduleEnv;
            }
            environment.PopModuleStack();

            // bind module or contents of module
            if (unqualified) 
            {
                foreach (var symbol in moduleEnv.GetLocalSymbols()) 
                {
                    if (symbol.StartsWith("_")) continue; // skip private module symbols
                    environment.Put(symbol, moduleEnv.Get(symbol, pos));
                }
            } 
            else 
            {
                var obj = new ValueObject {isModule = true};
                foreach (var symbol in moduleEnv.GetLocalSymbols()) {
                    if (symbol.StartsWith("_")) continue; // skip private module symbols
                    var val = moduleEnv.Get(symbol, pos);
                    if (val.IsObject() && val.AsObject().isModule) continue; // do not re-export modules!
                    obj.AddItem(symbol, val);
                }
                environment.Put(modulename, obj);
            }
            return ValueNull.NULL;
        }

        public override string ToString() 
        {
            return "(require " + modulespec + (name != null ? " as " + name : "") + (unqualified ? " unqualified" : "") + ")";
        }

        public void CollectVars(ICollection<string> freeVars, ICollection<string> boundVars, ICollection<string> additionalBoundVars) 
        {
            // empty
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