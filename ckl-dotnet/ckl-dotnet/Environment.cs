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
using System.ComponentModel;
using System.Data.SqlTypes;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CheckerLang
{
    public class Environment
    {
        private Dictionary<string, object> map = new Dictionary<string, object>();
        private Environment parent;

        public Dictionary<string, Environment> modules = null;
        public List<string> modulestack = null;

        public Environment()
        {
            parent = null;
            modules = new Dictionary<string, Environment>();
            modulestack = new List<string>();
        }

        public Environment(Environment parent)
        {
            this.parent = parent;
        }

        public Environment WithParent(Environment parent)
        {
            this.parent = parent;
            return this;
        }

        public Environment GetParent()
        {
            return parent;
        }

        public Environment GetBase() 
        {
            var current = this;
            while (current.parent != null) current = current.parent;
            return current;
        }

        public IEnumerable<string> GetSymbols()
        {
            var result = new List<string>();
            result.AddRange(map.Keys);
            if (parent != null) result.AddRange(parent.GetSymbols());
            result.Sort();
            return result;
        }
        
        public List<string> GetLocalSymbols() {
            return new List<string>(map.Keys);
        }

        public Dictionary<string, Environment> GetModules() {
            var current = this;
            while (current.parent != null) current = current.parent;
            return current.modules;
        }

        public void PushModuleStack(string moduleidentifier, SourcePos pos) {
            var current = this;
            while (current.parent != null) current = current.parent;
            if (current.modulestack.Contains(moduleidentifier)) throw new ControlErrorException("Found circular module dependency (" + moduleidentifier + ")", pos);
            current.modulestack.Add(moduleidentifier);
        }

        public void PopModuleStack() {
            var current = this;
            while (current.parent != null) current = current.parent;
            current.modulestack.RemoveAt(current.modulestack.Count - 1);
        }

        public void Put(string name, object value) 
        {
            map[name] = value;
        }

        public void Set(string name, object value)
        {
            if (map.ContainsKey(name)) map[name] = value;
            else if (parent != null) parent.Set(name, value);
            else throw new ControlErrorException(name + " is not defined");
        }

        public void Remove(string name)
        {
            map.Remove(name);
        }
        
        public Environment NewEnv()
        {
            return new Environment(this);
        }
        
        public bool IsDefined(string symbol)
        {
            if (map.ContainsKey(symbol)) return true;
            if (parent != null) return parent.IsDefined(symbol);
            return false;
        }
        
        public Value Get(string symbol, SourcePos pos) 
        {
            if (map.ContainsKey(symbol))
            {
                var value = map[symbol];
                if (value == null) return ValueNull.NULL;
                switch (value)
                {
                    case Value _:
                        return (Value) value;
                    case byte _:
                        return new ValueInt((byte) value);
                    case sbyte _:
                        return new ValueInt((sbyte) value);
                    case short _:
                        return new ValueInt((short) value);
                    case ushort _:
                        return new ValueInt((ushort) value);
                    case int _:
                        return new ValueInt((int) value);
                    case uint _:
                        return new ValueInt((uint) value);
                    case long _:
                        return new ValueInt((long) value);
                    case ulong _:
                        return new ValueInt((long) (ulong) value);
                    case float _:
                        return new ValueDecimal((decimal)(float) value);
                    case double _:
                        return new ValueDecimal((decimal)(double) value);
                    case decimal _:
                        return new ValueDecimal((decimal) value);
                    case bool _:
                        return ValueBoolean.From((bool) value);
                    case BigInteger _:
                        return new ValueInt((long) (BigInteger) value);
                    case SqlInt64 _:
                        return new ValueInt((long) (SqlInt64) value);
                    case SqlMoney _:
                        return new ValueDecimal((decimal) (SqlMoney) value);
                    case Regex _:
                        return new ValuePattern(((Regex) value).ToString());
                    case DateTime _:
                        return new ValueDate((DateTime) value);
                    case TimeSpan _:
                        return new ValueDecimal((decimal)((TimeSpan) value).TotalDays);
                    default:
                        return new ValueString(value.ToString());
                }
            }
            if (parent != null) return parent.Get(symbol, pos);
            throw new ControlErrorException(new ValueString("Symbol '" + symbol + "' not defined"), pos, new Stacktrace());
        }

        public static Environment GetNullEnvironment()
        {
            return new Environment();
        }
        
        public static Environment GetBaseEnvironment(bool secure = true, bool legacy = true)
        {
            var result = GetNullEnvironment();
            result.Put("checkerlang_secure_mode", ValueBoolean.From(secure));
            Add(result, new FuncBindNative());
            result.Put("NULL", ValueNull.NULL);
            result.Put("MAXINT", new ValueInt(long.MaxValue).WithInfo("MAXINT\n\nThe maximal int value"));
            result.Put("MININT", new ValueInt(long.MinValue).WithInfo("MININT\n\nThe minimal int value"));
            result.Put("MAXDECIMAL", new ValueDecimal(decimal.MaxValue).WithInfo("MAXDECIMAL\n\nThe maximal decimal value"));
            result.Put("MINDECIMAL", new ValueDecimal(decimal.MinValue).WithInfo("MINDECIMAL\n\nThe minimal decimal value"));
            var assembly = typeof(Environment).Assembly;
            if (legacy)
            {
                var basenode = Parser.Parse(new StreamReader(assembly.GetManifestResourceStream("checkerlang.module-legacy.ckl")), "mod:legacy.ckl");
                basenode.Evaluate(result);
            }
            else
            {
                var basenode = Parser.Parse(new StreamReader(assembly.GetManifestResourceStream("checkerlang.module-base.ckl")), "mod:base.ckl");
                basenode.Evaluate(result);
            }
            return result;
        }

        private static void Add(Environment env, ValueFunc func, string alias = null)
        {
            env.Put(func.GetName(), func);
            if (alias != null) env.Put(alias, func);
        }
    }
}