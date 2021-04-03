using System.Collections.Generic;

namespace CheckerLang
{
    public class Function
    {
        public static Value invoke(ValueFunc fn, List<string> names_, List<Node> args, Environment environment, SourcePos pos) 
        {
            var values = new List<Value>();
            var names = new List<string>();
            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                if (arg is NodeSpread) 
                {
                    var argvalue = arg.Evaluate(environment);
                    if (argvalue.IsMap()) 
                    {
                        var map = argvalue.AsMap();
                        foreach (var entry in map.GetValue()) 
                        {
                            values.Add(entry.Value);
                            if (entry.Key.IsString()) 
                            {
                                names.Add(entry.Key.AsString().GetValue());
                            } 
                            else 
                            {
                                names.Add(null);
                            }
                        }
                    } 
                    else 
                    {
                        var list = argvalue.AsList();
                        foreach (var value in list.GetValue()) 
                        {
                            values.Add(value);
                            names.Add(null);
                        }
                    }
                } 
                else 
                {
                    values.Add(arg.Evaluate(environment));
                    names.Add(names_[i]);
                }
            }

            var args_ = new Args(fn.AsFunc().GetArgNames(), pos);
            args_.SetArgs(names, values);

            try 
            {
                return fn.AsFunc().Execute(args_, environment, pos);
            } 
            catch (ControlErrorException e) 
            {
                e.AddStacktraceElement(GetFuncallString(fn.AsFunc(), args_), pos);
                throw e;
            }
        }

        public static string GetFuncallString(ValueFunc fn, Args args) 
        {
            return fn.GetName() + "(" + args.ToStringAbbrev() + ")";
        }
       
    }
}