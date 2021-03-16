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
package ch.checkerlang.nodes;

import ch.checkerlang.*;
import ch.checkerlang.values.*;

import java.io.IOException;
import java.util.*;

public class NodeRequire implements Node {
    private String modulespec;
    private String name;
    private boolean unqualified;

    private SourcePos pos;

    public NodeRequire(String modulespec, String name, boolean unqualified, SourcePos pos) {
        this.modulespec = modulespec;
        this.name = name;
        this.unqualified = unqualified;
        this.pos = pos;
    }

    public Value evaluate(Environment environment) {
        Map<String, Environment> modules = environment.getModules();
        // resolve module file, identifier and name
        String modulefile = this.modulespec;
        if (!modulefile.endsWith(".ckl")) modulefile += ".ckl";
        String moduleidentifier = null;
        String modulename = this.name;
        String[] parts = this.modulespec.split("/");
        String name = parts[parts.length - 1];
        if (name.endsWith(".ckl")) name = name.substring(0, name.length() - 4);
        moduleidentifier = name;
        if (modulename == null) modulename = name;
        environment.pushModuleStack(moduleidentifier, this.pos);

        // lookup or read module
        Environment moduleEnv = null;
        if (modules.containsKey(moduleidentifier)) {
            moduleEnv = modules.get(moduleidentifier);
        } else {
            moduleEnv = environment.getBase().newEnv();
            String modulesrc = ModuleLoader.loadModule(modulefile, this.pos);
            Node node = null;
            try {
                node = Parser.parse(modulesrc, modulefile);
            } catch (IOException e) {
                throw new ControlErrorException("Cannot parse module " + moduleidentifier, this.pos);
            }
            node.evaluate(moduleEnv);
            modules.put(moduleidentifier, moduleEnv);
        }
        environment.popModuleStack();

        // bind module or contents of module
        if (this.unqualified) {
            for (String symbol : moduleEnv.getLocalSymbols()) {
                if (symbol.startsWith("_")) continue; // skip private module symbols
                environment.put(symbol, moduleEnv.get(symbol, this.pos));
            }
        } else {
            ValueObject obj = new ValueObject();
            obj.isModule = true;
            for (String symbol : moduleEnv.getLocalSymbols()) {
                if (symbol.startsWith("_")) continue; // skip private module symbols
                Value val = moduleEnv.get(symbol, this.pos);
                if (val.isObject() && val.asObject().isModule) continue; // do not re-export modules!
                obj.addItem(symbol, val);
            }
            environment.put(modulename, obj);
        }
        return ValueNull.NULL;
    }

    public String toString() {
        return "(require " + modulespec + (name != null ? " as " + name : "") + (unqualified ? " unqualified" : "") + ")";
    }

    public void collectVars(Collection<String> freeVars, Collection<String> boundVars, Collection<String> additionalBoundVars) {
        // empty
    }

    public SourcePos getSourcePos() {
        return pos;
    }

    public boolean isLiteral() {
        return false;
    }
}
