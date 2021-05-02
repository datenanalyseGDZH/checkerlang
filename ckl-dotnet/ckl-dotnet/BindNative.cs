using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CheckerLang
{
    public class BindNative
    {
        public static void Bind(Environment env, string nativeName, string nativeAlias, SourcePos pos) 
        {
            switch (nativeName) 
            {
                case "acos": BindNativeFunc(env, new FuncAcos(), nativeAlias); break;
                case "add": BindNativeFunc(env, new FuncAdd(), nativeAlias); break;
                case "append": BindNativeFunc(env, new FuncAppend(), nativeAlias); break;
                case "asin": BindNativeFunc(env, new FuncAsin(), nativeAlias); break;
                case "atan": BindNativeFunc(env, new FuncAtan(), nativeAlias); break;
                case "atan2": BindNativeFunc(env, new FuncAtan2(), nativeAlias); break;
                case "bind_native": BindNativeFunc(env, new FuncBindNative(), nativeAlias); break;
                case "body": BindNativeFunc(env, new FuncBody(), nativeAlias); break;
                case "boolean": BindNativeFunc(env, new FuncBoolean(), nativeAlias); break;
                case "ceiling": BindNativeFunc(env, new FuncCeiling(), nativeAlias); break;
                case "close": BindNativeFunc(env, new FuncClose(), nativeAlias); break;
                case "compare": BindNativeFunc(env, new FuncCompare(), nativeAlias); break;
                case "contains": BindNativeFunc(env, new FuncContains(), nativeAlias); break;
                case "cos": BindNativeFunc(env, new FuncCos(), nativeAlias); break;
                case "date": BindNativeFunc(env, new FuncDate(), nativeAlias); break;
                case "decimal": BindNativeFunc(env, new FuncDecimal(), nativeAlias); break;
                case "delete_at": BindNativeFunc(env, new FuncDeleteAt(), nativeAlias); break;
                case "div": BindNativeFunc(env, new FuncDiv(), nativeAlias); break;
                case "ends_with": BindNativeFunc(env, new FuncEndsWith(), nativeAlias); break;
                case "equals": BindNativeFunc(env, new FuncEquals(), nativeAlias); break;
                case "escape_pattern": BindNativeFunc(env, new FuncEscapePattern(), nativeAlias); break;
                case "eval": BindNativeFunc(env, new FuncEval(), nativeAlias); break;
                case "execute": BindNativeFunc(env, new FuncExecute(), nativeAlias); break;
                case "exp": BindNativeFunc(env, new FuncExp(), nativeAlias); break;
                case "file_copy": BindNativeFunc(env, new FuncFileCopy(), nativeAlias); break;
                case "file_delete": BindNativeFunc(env, new FuncFileDelete(), nativeAlias); break;
                case "file_exists": BindNativeFunc(env, new FuncFileExists(), nativeAlias); break;
                case "file_info": BindNativeFunc(env, new FuncFileInfo(), nativeAlias); break;
                case "file_input": BindNativeFunc(env, new FuncFileInput(), nativeAlias); break;
                case "file_move": BindNativeFunc(env, new FuncFileMove(), nativeAlias); break;
                case "file_output": BindNativeFunc(env, new FuncFileOutput(), nativeAlias); break;
                case "find": BindNativeFunc(env, new FuncFind(), nativeAlias); break;
                case "floor": BindNativeFunc(env, new FuncFloor(), nativeAlias); break;
                case "format_date": BindNativeFunc(env, new FuncFormatDate(), nativeAlias); break;
                case "get_env": BindNativeFunc(env, new FuncGetEnv(), nativeAlias); break;
                case "get_output_string": BindNativeFunc(env, new FuncGetOutputString(), nativeAlias); break;
                case "greater": BindNativeFunc(env, new FuncGreater(), nativeAlias); break;
                case "greater_equals": BindNativeFunc(env, new FuncGreaterEquals(), nativeAlias); break;
                case "identity": BindNativeFunc(env, new FuncIdentity(), nativeAlias); break;
                case "if_empty": BindNativeFunc(env, new FuncIfEmpty(), nativeAlias); break;
                case "if_null": BindNativeFunc(env, new FuncIfNull(), nativeAlias); break;
                case "if_null_or_empty": BindNativeFunc(env, new FuncIfNullOrEmpty(), nativeAlias); break;
                case "info": BindNativeFunc(env, new FuncInfo(), nativeAlias); break;
                case "insert_at": BindNativeFunc(env, new FuncInsertAt(), nativeAlias); break;
                case "int": BindNativeFunc(env, new FuncInt(), nativeAlias); break;
                case "is_empty": BindNativeFunc(env, new FuncIsEmpty(), nativeAlias); break;
                case "is_not_empty": BindNativeFunc(env, new FuncIsNotEmpty(), nativeAlias); break;
                case "is_not_null": BindNativeFunc(env, new FuncIsNotNull(), nativeAlias); break;
                case "is_null": BindNativeFunc(env, new FuncIsNull(), nativeAlias); break;
                case "length": BindNativeFunc(env, new FuncLength(), nativeAlias); break;
                case "less": BindNativeFunc(env, new FuncLess(), nativeAlias); break;
                case "less_equals": BindNativeFunc(env, new FuncLessEquals(), nativeAlias); break;
                case "list": BindNativeFunc(env, new FuncList(), nativeAlias); break;
                case "list_dir": BindNativeFunc(env, new FuncListDir(), nativeAlias); break;
                case "log": BindNativeFunc(env, new FuncLog(), nativeAlias); break;
                case "lower": BindNativeFunc(env, new FuncLower(), nativeAlias); break;
                case "ls": BindNativeFunc(env, new FuncLs(), nativeAlias); break;
                case "make_dir": BindNativeFunc(env, new FuncMakeDir(), nativeAlias); break;
                case "map": BindNativeFunc(env, new FuncMap(), nativeAlias); break;
                case "matches": BindNativeFunc(env, new FuncMatches(), nativeAlias); break;
                case "mod": BindNativeFunc(env, new FuncMod(), nativeAlias); break;
                case "mul": BindNativeFunc(env, new FuncMul(), nativeAlias); break;
                case "not_equals": BindNativeFunc(env, new FuncNotEquals(), nativeAlias); break;
                case "object": BindNativeFunc(env, new FuncObject(), nativeAlias); break;
                case "parse": BindNativeFunc(env, new FuncParse(), nativeAlias); break;
                case "parse_date": BindNativeFunc(env, new FuncParseDate(), nativeAlias); break;
                case "parse_json": BindNativeFunc(env, new FuncParseJson(), nativeAlias); break;
                case "pattern": BindNativeFunc(env, new FuncPattern(), nativeAlias); break;
                case "pow": BindNativeFunc(env, new FuncPow(), nativeAlias); break;
                case "print": BindNativeFunc(env, new FuncPrint(), nativeAlias); break;
                case "println": BindNativeFunc(env, new FuncPrintln(), nativeAlias); break;
                case "process_lines": BindNativeFunc(env, new FuncProcessLines(), nativeAlias); break;
                case "put": BindNativeFunc(env, new FuncPut(), nativeAlias); break;
                case "random": BindNativeFunc(env, new FuncRandom(), nativeAlias); break;
                case "range": BindNativeFunc(env, new FuncRange(), nativeAlias); break;
                case "read": BindNativeFunc(env, new FuncRead(), nativeAlias); break;
                case "read_all": BindNativeFunc(env, new FuncReadall(), nativeAlias); break;
                case "readln": BindNativeFunc(env, new FuncReadln(), nativeAlias); break;
                case "remove": BindNativeFunc(env, new FuncRemove(), nativeAlias); break;
                case "replace": BindNativeFunc(env, new FuncReplace(), nativeAlias); break;
                case "round": BindNativeFunc(env, new FuncRound(), nativeAlias); break;
                case "s": BindNativeFunc(env, new FuncS(), nativeAlias); break;
                case "set": BindNativeFunc(env, new FuncSet(), nativeAlias); break;
                case "set_seed": BindNativeFunc(env, new FuncSetSeed(), nativeAlias); break;
                case "sin": BindNativeFunc(env, new FuncSin(), nativeAlias); break;
                case "sorted": BindNativeFunc(env, new FuncSorted(), nativeAlias); break;
                case "split": BindNativeFunc(env, new FuncSplit(), nativeAlias); break;
                case "split2": BindNativeFunc(env, new FuncSplit2(), nativeAlias); break;
                case "sqrt": BindNativeFunc(env, new FuncSqrt(), nativeAlias); break;
                case "str_input": BindNativeFunc(env, new FuncStrInput(), nativeAlias); break;
                case "starts_with": BindNativeFunc(env, new FuncStartsWith(), nativeAlias); break;
                case "str_output": BindNativeFunc(env, new FuncStrOutput(), nativeAlias); break;
                case "string": BindNativeFunc(env, new FuncString(), nativeAlias); break;
                case "sub": BindNativeFunc(env, new FuncSub(), nativeAlias); break;
                case "sublist": BindNativeFunc(env, new FuncSublist(), nativeAlias); break;
                case "substr": BindNativeFunc(env, new FuncSubstr(), nativeAlias); break;
                case "sum": BindNativeFunc(env, new FuncSum(), nativeAlias); break;
                case "tan": BindNativeFunc(env, new FuncTan(), nativeAlias); break;
                case "timestamp": BindNativeFunc(env, new FuncTimestamp(), nativeAlias); break;
                case "trim": BindNativeFunc(env, new FuncTrim(), nativeAlias); break;
                case "type": BindNativeFunc(env, new FuncType(), nativeAlias); break;
                case "upper": BindNativeFunc(env, new FuncUpper(), nativeAlias); break;
                case "zip": BindNativeFunc(env, new FuncZip(), nativeAlias); break;
                case "zip_map": BindNativeFunc(env, new FuncZipMap(), nativeAlias); break;
                case "E": env.Put("E", new ValueDecimal((decimal) Math.E).WithInfo("E\n\nThe mathematical constant E (Eulers number)")); break;
                case "PI": env.Put("PI", new ValueDecimal((decimal) Math.PI).WithInfo("PI\n\nThe mathematical constant PI")); break;
                case "PS": env.Put("PS", new ValueString(Path.DirectorySeparatorChar.ToString()).WithInfo("PS\n\nThe OS path separator (posix: /, windows: \\).")); break;
                case "LS": env.Put("LS", new ValueString(System.Environment.NewLine).WithInfo("PS\n\nThe OS line separator (posix: \\n, windows: \\r\\n).")); break;
                case "FS": env.Put("FS", new ValueString(Path.PathSeparator.ToString()).WithInfo("FS\n\nThe OS field separator (posix: :, windows: ;).")); break;
                case "OS_NAME": env.Put("OS_NAME", new ValueString(GetOsName()).WithInfo("OS_NAME\n\nThe name of the operating system, one of Windows, Linux, macOS")); break;
                case "OS_VERSION": env.Put("OS_VERSION", new ValueString(GetOsVersion()).WithInfo("OS_VERSION\n\nThe version of the operating system.")); break;
                case "OS_ARCH": env.Put("OS_ARCH", new ValueString(GetOsArch()).WithInfo("OS_ARCH\n\nThe architecture of the operating system, one of x86, amd64.")); break;
                default:
                    throw new ControlErrorException(new ValueString("ERROR"),"Unknown native " + nativeName, pos);
            }
        }
 
        private static string GetOsName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "Windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "Linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "macOS";
            return "Unknown";
        }

        private static string GetOsVersion()
        {
            return RuntimeInformation.OSDescription;
        }

        private static string GetOsArch()
        {
            var arch = RuntimeInformation.OSArchitecture;
            if (arch == Architecture.X86) return "x86";
            if (arch == Architecture.X64) return "amd64";
            return "Unknown";
        }
        
        private static void BindNativeFunc(Environment env, FuncBase func, string alias) 
        {
            env.Put(func.GetName(), func);
            if (alias != null) env.Put(alias, func);
        }
        
    }
}