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

        public Environment()
        {
            parent = null;
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

        public IEnumerable<string> GetSymbols()
        {
            var result = new List<string>();
            result.AddRange(map.Keys);
            if (parent != null) result.AddRange(parent.GetSymbols());
            result.Sort();
            return result;
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
        
        public static Environment GetBaseEnvironment(bool secure = true)
        {
            var result = GetRootEnvironment(secure).NewEnv();
            var assembly = typeof(Environment).Assembly;
            var basenode = Parser.Parse(new StreamReader(assembly.GetManifestResourceStream("checkerlang.base-library.ckl")), "{res}/base-library.ckl");
            basenode.Evaluate(result);
            return result;
        }
        
        public static Environment GetRootEnvironment(bool secure = true)
        {
            var result = GetNullEnvironment();
            Add(result, new FuncAcos());
            Add(result, new FuncAdd());
            Add(result, new FuncAppend());
            Add(result, new FuncAsin());
            Add(result, new FuncAtan());
            Add(result, new FuncBody());
            Add(result, new FuncBoolean());
            Add(result, new FuncCeiling());
            Add(result, new FuncCompare());
            Add(result, new FuncContains(), "str_contains");
            Add(result, new FuncCos());
            Add(result, new FuncDate());
            Add(result, new FuncDecimal());
            Add(result, new FuncDiv());
            Add(result, new FuncDiv0());
            Add(result, new FuncEndsWith(), "str_ends_with");
            Add(result, new FuncEquals());
            Add(result, new FuncEscapePattern());
            Add(result, new FuncEval());
            Add(result, new FuncExp());
            Add(result, new FuncFind(), "str_find");
            Add(result, new FuncFloor());
            Add(result, new FuncFormatDate());
            Add(result, new FuncGreater());
            Add(result, new FuncGreaterEquals());
            Add(result, new FuncIfEmpty());
            Add(result, new FuncIfNull());
            Add(result, new FuncIfNullOrEmpty());
            Add(result, new FuncInfo());
            Add(result, new FuncInt());
            Add(result, new FuncIsEmpty());
            Add(result, new FuncIsNotEmpty());
            Add(result, new FuncIsNotNull());
            Add(result, new FuncIsNull());
            Add(result, new FuncLength());
            Add(result, new FuncLess());
            Add(result, new FuncLessEquals());
            Add(result, new FuncList());
            Add(result, new FuncLog());
            Add(result, new FuncLower());
            Add(result, new FuncLs());
            Add(result, new FuncMap());
            Add(result, new FuncMatches(), "str_matches");
            Add(result, new FuncMod());
            Add(result, new FuncMul());
            Add(result, new FuncNotEquals());
            Add(result, new FuncNow());
            Add(result, new FuncPairs());
            Add(result, new FuncParse());
            Add(result, new FuncParseJson());
            Add(result, new FuncParseDate());
            Add(result, new FuncPattern());
            Add(result, new FuncPow());
            Add(result, new FuncPrint());
            Add(result, new FuncPrintln());
            Add(result, new FuncProcessLines());
            Add(result, new FuncPut());
            Add(result, new FuncRandom());
            Add(result, new FuncRange());
            Add(result, new FuncRead());
            Add(result, new FuncReadall());
            Add(result, new FuncReadln());
            Add(result, new FuncReplace());
            Add(result, new FuncRound());
            Add(result, new FuncS());
            Add(result, new FuncSet());
            Add(result, new FuncSetSeed());
            Add(result, new FuncSin());
            Add(result, new FuncSorted());
            Add(result, new FuncSplit());
            Add(result, new FuncSplit2());
            Add(result, new FuncSqrt());
            Add(result, new FuncStartsWith(), "str_starts_with");
            Add(result, new FuncStrInput());
            Add(result, new FuncString());
            Add(result, new FuncSub());
            Add(result, new FuncSublist());
            Add(result, new FuncSubstr());
            Add(result, new FuncSum());
            Add(result, new FuncTan());
            Add(result, new FuncTimestamp());
            Add(result, new FuncTrim(), "str_trim");
            Add(result, new FuncType());
            Add(result, new FuncUpper());
            Add(result, new FuncZip());
            Add(result, new FuncZipMap());
            if (!secure)
            {
                Add(result, new FuncFileInput());
                Add(result, new FuncFileOutput());
                Add(result, new FuncClose());
            }
            result.Put("NULL", ValueNull.NULL);
            result.Put("PI", new ValueDecimal((decimal) Math.PI).WithInfo("The mathematical constant pi."));
            result.Put("E", new ValueDecimal((decimal) Math.E).WithInfo("The mathematical constant e."));
            result.Put("MAXINT", new ValueInt(long.MaxValue).WithInfo("The maximal int value"));
            result.Put("MININT", new ValueInt(long.MinValue).WithInfo("The minimal int value"));
            result.Put("MAXDECIMAL", new ValueDecimal(decimal.MaxValue).WithInfo("The maximal decimal value"));
            result.Put("MINDECIMAL", new ValueDecimal(decimal.MinValue).WithInfo("The minimal decimal value"));
            var v = Assembly.GetAssembly(typeof(Environment)).GetName().Version;
            var version = new ValueString(v.Major + "." + v.Minor + "." + v.Build);
            result.Put("checkerlang_version", version);
            result.Put("checkerlang_platform", new ValueString("dotnet"));
            return result;
        }

        private static void Add(Environment env, ValueFunc func, string alias = null)
        {
            env.Put(func.GetName(), func);
            if (alias != null) env.Put(alias, func);
        }
    }
}