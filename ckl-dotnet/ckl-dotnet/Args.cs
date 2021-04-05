using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace CheckerLang
{
    public class Args
    {
        private List<string> argNames = new List<string>();
        private Dictionary<string, Value> args = new Dictionary<string, Value>();
        private string restArgName = null;
        private SourcePos pos;

        public Args(string name, Value value, SourcePos pos)
        {
            this.pos = pos;
            argNames.Add(name);
            args[name] = value;
        }

        public Args(string name1, string name2, Value value1, Value value2, SourcePos pos)
        {
            this.pos = pos;
            argNames.Add(name1);
            argNames.Add(name2);
            args[name1] = value1;
            args[name2] = value2;
        }

        
        public Args(IReadOnlyList<string> argnames, SourcePos pos)
        {
            this.pos = pos;
            for (var i = 0; i < argnames.Count; i++)
            {
                if (!argnames[i].EndsWith("..."))
                {
                    argNames.Add(argnames[i]);
                }
                else
                {
                    restArgName = argnames[i];
                }
            }
        }

        public void SetArgs(List<string> names, List<Value> values)
        {
            var rest = new ValueList();
            for (var i = 0; i < values.Count; i++)
            {
                if (names[i] != null)
                {
                    if (!argNames.Contains(names[i])) throw new ControlErrorException("Argument " + names[i] + " is unknown", pos);
                    args[names[i]] = values[i];
                }
            }

            var inKeywords = false;
            for (var i = 0; i < values.Count; i++)
            {
                if (names[i] == null)
                {
                    if (inKeywords) throw new ControlErrorException("Positional arguments need to be placed before named arguments", pos);
                    var argName = GetNextPositionalArgName();
                    if (argName == null)
                    {
                        if (restArgName == null) throw new ControlErrorException("Too many arguments", pos);
                        rest.AddItem(values[i]);
                    }
                    else if (!args.ContainsKey(argName))
                    {
                        args[argName] = values[i];
                    }
                    else
                    {
                        rest.AddItem(values[i]);
                    }
                }
                else
                {
                    inKeywords = true;
                    if (!argNames.Contains(names[i])) throw new ControlErrorException("Argument " + names[i] + " is unknown", pos);
                    args[names[i]] = values[i];
                }
            }

            if (restArgName != null)
            {
                args[restArgName] = rest;
            }
        }

        private string GetNextPositionalArgName()
        {
            foreach (var argname in argNames)
            {
                if (!args.ContainsKey(argname)) return argname;
            }
            return null;
        }
        
        public bool HasArg(string name)
        {
            return args.ContainsKey(name);
        }
        
        public Value Get(string name)
        {
            if (!HasArg(name)) throw new ControlErrorException("Missing argument " + name, pos);
            return args?[name];
        }

        public bool IsNull(string name) {
            if (!HasArg(name)) return false;
            return Get(name).IsNull();
        }

        public ValueString GetString(string name) {
            var value = Get(name);
            if (!value.IsString()) throw new ControlErrorException("String required but got " + value.Type(), pos);
            return value.AsString();
        }

        public ValueBoolean GetBoolean(string name) {
            var value = Get(name);
            if (!value.IsBoolean()) throw new ControlErrorException("Boolean required but got " + value.Type(), pos);
            return value.AsBoolean();
        }

        public ValueString GetString(string name, string defaultValue) {
            if (!HasArg(name)) return new ValueString(defaultValue);
            var value = Get(name);
            if (!value.IsString()) throw new ControlErrorException("String required but got " + value.Type(), pos);
            return value.AsString();
        }

        public ValueInt GetInt(string name) {
            var value = Get(name);
            if (!value.IsInt()) throw new ControlErrorException("Int required but got " + value.Type(), pos);
            return value.AsInt();
        }

        public ValueInt GetInt(string name, long defaultValue) {
            if (!HasArg(name)) return new ValueInt(defaultValue);
            var value = Get(name);
            if (!value.IsInt()) throw new ControlErrorException("Int required but got " + value.Type(), pos);
            return value.AsInt();
        }

        public ValueDecimal GetDecimal(string name) {
            var value = Get(name);
            if (!value.IsDecimal()) throw new ControlErrorException("Decimal required but got " + value.Type(), pos);
            return value.AsDecimal();
        }

        public ValueDecimal GetDecimal(string name, decimal defaultValue) {
            if (!HasArg(name)) return new ValueDecimal(defaultValue);
            var value = Get(name);
            if (!value.IsDecimal()) throw new ControlErrorException("Decimal required but got " + value.Type(), pos);
            return value.AsDecimal();
        }

        public ValueDecimal GetNumerical(string name) {
            var value = Get(name);
            if (!value.IsDecimal() && !value.IsInt()) throw new ControlErrorException("Numerical required but got " + value.Type(), pos);
            return value.AsDecimal();
        }

        public ValueDecimal GetNumerical(string name, decimal defaultValue) {
            if (!HasArg(name)) return new ValueDecimal(defaultValue);
            var value = Get(name);
            if (!value.IsDecimal() && !value.IsInt()) throw new ControlErrorException("Numerical required but got " + value.Type(), pos);
            return value.AsDecimal();
        }

        public ValueList GetList(string name) {
            var value = Get(name);
            if (!value.IsList()) throw new ControlErrorException("List required but got " + value.Type(), pos);
            return value.AsList();
        }

        public ValueMap GetMap(string name) {
            var value = Get(name);
            if (!value.IsMap()) throw new ControlErrorException("Map required but got " + value.Type(), pos);
            return value.AsMap();
        }

        public ValueInput GetInput(string name) {
            var value = Get(name);
            if (!value.IsInput()) throw new ControlErrorException("Input required but got " + value.Type(), pos);
            return value.AsInput();
        }

        public ValueInput GetInput(string name, ValueInput defaultValue) {
            if (!HasArg(name)) return defaultValue;
            return GetInput(name);
        }

        public ValueOutput GetOutput(string name) {
            var value = Get(name);
            if (!value.IsOutput()) throw new ControlErrorException("Output required but got " + value.Type(), pos);
            return value.AsOutput();
        }

        public ValueOutput GetOutput(string name, ValueOutput defaultValue) {
            if (!HasArg(name)) return defaultValue;
            return GetOutput(name);
        }

        public ValueFunc GetFunc(string name) {
            var value = Get(name);
            if (!value.IsFunc()) throw new ControlErrorException("Func required but got " + value.Type(), pos);
            return value.AsFunc();
        }

        public ValueDate GetDate(string name) {
            var value = Get(name);
            if (!value.IsDate()) throw new ControlErrorException("Date required but got " + value.Type(), pos);
            return value.AsDate();
        }

        public ValueBoolean GetAsBoolean(string name) {
            var value = Get(name);
            try {
                return value.AsBoolean();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValueDate GetAsDate(string name) {
            var value = Get(name);
            try {
                return value.AsDate();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValueString GetAsString(string name) {
            var value = Get(name);
            try {
                return value.AsString();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValuePattern GetAsPattern(string name) {
            var value = Get(name);
            try {
                return value.AsPattern();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValuePattern GetAsPattern(string name, ValuePattern defaultValue) {
            if (!HasArg(name)) return defaultValue;
            return GetAsPattern(name);
        }

        public ValueList GetAsList(string name) {
            var value = Get(name);
            try {
                return value.AsList();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValueSet GetAsSet(string name) {
            var value = Get(name);
            try {
                return value.AsSet();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValueObject GetAsObject(string name) {
            var value = Get(name);
            try {
                return value.AsObject();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValueMap GetAsMap(string name) {
            var value = Get(name);
            try {
                return value.AsMap();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValueInt GetAsInt(string name) {
            var value = Get(name);
            try {
                return value.AsInt();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValueDecimal GetAsDecimal(string name) {
            var value = Get(name);
            try {
                return value.AsDecimal();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public ValueNode GetAsNode(string name) {
            var value = Get(name);
            try {
                return value.AsNode();
            } catch (ControlErrorException e) {
                e.pos = pos;
                throw e;
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var argname in args.Keys)
            {
                result.Append(argname).Append("=").Append(args[argname]).Append(", ");
            }
            if (result.Length > 0) result.Remove(result.Length - 2, 2);
            return result.ToString();
        }

        public string ToStringAbbrev() {
            var result = new StringBuilder();
            foreach (var argname in args.Keys) {
                var value = args[argname].ToString();
                if (value.Length > 50) value = value.Substring(0, 50) + "... " + value.Substring(value.Length - 5, 5);
                result.Append(argname).Append("=").Append(value).Append(", ");
            }
            if (result.Length > 0) result.Remove(result.Length - 2, 2);
            return result.ToString();
        }
        
    }
}