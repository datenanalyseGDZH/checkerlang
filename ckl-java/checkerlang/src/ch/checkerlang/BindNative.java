package ch.checkerlang;

import ch.checkerlang.functions.*;
import ch.checkerlang.values.ValueDecimal;
import ch.checkerlang.values.ValueString;

import java.util.Locale;

public class BindNative {
    public static void bind(Environment env, String nativeName, String nativeAlias, SourcePos pos) {
        switch (nativeName) {
            case "acos": bindNative(env, new FuncAcos(), nativeAlias); break;
            case "add": bindNative(env, new FuncAdd(), nativeAlias); break;
            case "append": bindNative(env, new FuncAppend(), nativeAlias); break;
            case "asin": bindNative(env, new FuncAsin(), nativeAlias); break;
            case "atan": bindNative(env, new FuncAtan(), nativeAlias); break;
            case "atan2": bindNative(env, new FuncAtan2(), nativeAlias); break;
            case "bind_native": bindNative(env, new FuncBindNative(), nativeAlias); break;
            case "body": bindNative(env, new FuncBody(), nativeAlias); break;
            case "boolean": bindNative(env, new FuncBoolean(), nativeAlias); break;
            case "ceiling": bindNative(env, new FuncCeiling(), nativeAlias); break;
            case "close": bindNative(env, new FuncClose(), nativeAlias); break;
            case "compare": bindNative(env, new FuncCompare(), nativeAlias); break;
            case "contains": bindNative(env, new FuncContains(), nativeAlias); break;
            case "cos": bindNative(env, new FuncCos(), nativeAlias); break;
            case "date": bindNative(env, new FuncDate(), nativeAlias); break;
            case "decimal": bindNative(env, new FuncDecimal(), nativeAlias); break;
            case "delete_at": bindNative(env, new FuncDeleteAt(), nativeAlias); break;
            case "div": bindNative(env, new FuncDiv(), nativeAlias); break;
            case "ends_with": bindNative(env, new FuncEndsWith(), nativeAlias); break;
            case "equals": bindNative(env, new FuncEquals(), nativeAlias); break;
            case "escape_pattern": bindNative(env, new FuncEscapePattern(), nativeAlias); break;
            case "eval": bindNative(env, new FuncEval(), nativeAlias); break;
            case "execute": bindNative(env, new FuncExecute(), nativeAlias); break;
            case "exp": bindNative(env, new FuncExp(), nativeAlias); break;
            case "file_input": bindNative(env, new FuncFileInput(), nativeAlias); break;
            case "file_copy": bindNative(env, new FuncFileCopy(), nativeAlias); break;
            case "file_delete": bindNative(env, new FuncFileDelete(), nativeAlias); break;
            case "file_exists": bindNative(env, new FuncFileExists(), nativeAlias); break;
            case "file_info": bindNative(env, new FuncFileInfo(), nativeAlias); break;
            case "file_move": bindNative(env, new FuncFileMove(), nativeAlias); break;
            case "file_output": bindNative(env, new FuncFileOutput(), nativeAlias); break;
            case "find": bindNative(env, new FuncFind(), nativeAlias); break;
            case "floor": bindNative(env, new FuncFloor(), nativeAlias); break;
            case "format_date": bindNative(env, new FuncFormatDate(), nativeAlias); break;
            case "get_env": bindNative(env, new FuncGetEnv(), nativeAlias); break;
            case "get_output_string": bindNative(env, new FuncGetOutputString(), nativeAlias); break;
            case "greater": bindNative(env, new FuncGreater(), nativeAlias); break;
            case "greater_equals": bindNative(env, new FuncGreaterEquals(), nativeAlias); break;
            case "identity": bindNative(env, new FuncIdentity(), nativeAlias); break;
            case "if_empty": bindNative(env, new FuncIfEmpty(), nativeAlias); break;
            case "if_null": bindNative(env, new FuncIfNull(), nativeAlias); break;
            case "if_null_or_empty": bindNative(env, new FuncIfNullOrEmpty(), nativeAlias); break;
            case "info": bindNative(env, new FuncInfo(), nativeAlias); break;
            case "insert_at": bindNative(env, new FuncInsertAt(), nativeAlias); break;
            case "int": bindNative(env, new FuncInt(), nativeAlias); break;
            case "is_empty": bindNative(env, new FuncIsEmpty(), nativeAlias); break;
            case "is_not_empty": bindNative(env, new FuncIsNotEmpty(), nativeAlias); break;
            case "is_not_null": bindNative(env, new FuncIsNotNull(), nativeAlias); break;
            case "is_null": bindNative(env, new FuncIsNull(), nativeAlias); break;
            case "length": bindNative(env, new FuncLength(), nativeAlias); break;
            case "less": bindNative(env, new FuncLess(), nativeAlias); break;
            case "less_equals": bindNative(env, new FuncLessEquals(), nativeAlias); break;
            case "list": bindNative(env, new FuncList(), nativeAlias); break;
            case "list_dir": bindNative(env, new FuncListDir(), nativeAlias); break;
            case "log": bindNative(env, new FuncLog(), nativeAlias); break;
            case "lower": bindNative(env, new FuncLower(), nativeAlias); break;
            case "ls": bindNative(env, new FuncLs(), nativeAlias); break;
            case "make_dir": bindNative(env, new FuncMakeDir(), nativeAlias); break;
            case "map": bindNative(env, new FuncMap(), nativeAlias); break;
            case "matches": bindNative(env, new FuncMatches(), nativeAlias); break;
            case "mod": bindNative(env, new FuncMod(), nativeAlias); break;
            case "mul": bindNative(env, new FuncMul(), nativeAlias); break;
            case "not_equals": bindNative(env, new FuncNotEquals(), nativeAlias); break;
            case "object": bindNative(env, new FuncObject(), nativeAlias); break;
            case "parse": bindNative(env, new FuncParse(), nativeAlias); break;
            case "parse_date": bindNative(env, new FuncParseDate(), nativeAlias); break;
            case "parse_json": bindNative(env, new FuncParseJson(), nativeAlias); break;
            case "pattern": bindNative(env, new FuncPattern(), nativeAlias); break;
            case "pow": bindNative(env, new FuncPow(), nativeAlias); break;
            case "print": bindNative(env, new FuncPrint(), nativeAlias); break;
            case "println": bindNative(env, new FuncPrintln(), nativeAlias); break;
            case "process_lines": bindNative(env, new FuncProcessLines(), nativeAlias); break;
            case "put": bindNative(env, new FuncPut(), nativeAlias); break;
            case "random": bindNative(env, new FuncRandom(), nativeAlias); break;
            case "range": bindNative(env, new FuncRange(), nativeAlias); break;
            case "read": bindNative(env, new FuncRead(), nativeAlias); break;
            case "read_all": bindNative(env, new FuncReadall(), nativeAlias); break;
            case "readln": bindNative(env, new FuncReadln(), nativeAlias); break;
            case "remove": bindNative(env, new FuncRemove(), nativeAlias); break;
            case "replace": bindNative(env, new FuncReplace(), nativeAlias); break;
            case "round": bindNative(env, new FuncRound(), nativeAlias); break;
            case "s": bindNative(env, new FuncS(), nativeAlias); break;
            case "set": bindNative(env, new FuncSet(), nativeAlias); break;
            case "set_seed": bindNative(env, new FuncSetSeed(), nativeAlias); break;
            case "sin": bindNative(env, new FuncSin(), nativeAlias); break;
            case "sorted": bindNative(env, new FuncSorted(), nativeAlias); break;
            case "split": bindNative(env, new FuncSplit(), nativeAlias); break;
            case "split2": bindNative(env, new FuncSplit2(), nativeAlias); break;
            case "sqrt": bindNative(env, new FuncSqrt(), nativeAlias); break;
            case "str_input": bindNative(env, new FuncStrInput(), nativeAlias); break;
            case "starts_with": bindNative(env, new FuncStartsWith(), nativeAlias); break;
            case "str_output": bindNative(env, new FuncStrOutput(), nativeAlias); break;
            case "string": bindNative(env, new FuncString(), nativeAlias); break;
            case "sub": bindNative(env, new FuncSub(), nativeAlias); break;
            case "sublist": bindNative(env, new FuncSublist(), nativeAlias); break;
            case "substr": bindNative(env, new FuncSubstr(), nativeAlias); break;
            case "sum": bindNative(env, new FuncSum(), nativeAlias); break;
            case "tan": bindNative(env, new FuncTan(), nativeAlias); break;
            case "timestamp": bindNative(env, new FuncTimestamp(), nativeAlias); break;
            case "trim": bindNative(env, new FuncTrim(), nativeAlias); break;
            case "type": bindNative(env, new FuncType(), nativeAlias); break;
            case "upper": bindNative(env, new FuncUpper(), nativeAlias); break;
            case "zip": bindNative(env, new FuncZip(), nativeAlias); break;
            case "zip_map": bindNative(env, new FuncZipMap(), nativeAlias); break;
            case "E": env.put("E", new ValueDecimal(Math.E).withInfo("E\n\nThe mathematical constant E (Eulers number)")); break;
            case "PI": env.put("PI", new ValueDecimal(Math.PI).withInfo("PI\n\nThe mathematical constant PI")); break;
            case "PS": env.put("PS", new ValueString(System.getProperty("file.separator")).withInfo("PS\n\nThe OS path separator (posix: /, windows: \\).")); break;
            case "LS": env.put("LS", new ValueString(System.getProperty("line.separator")).withInfo("PS\n\nThe OS line separator (posix: \\n, windows: \\r\\n).")); break;
            case "FS": env.put("FS", new ValueString(System.getProperty("path.separator")).withInfo("FS\n\nThe OS field separator (posix: :, windows: ;).")); break;
            case "OS_NAME": env.put("OS_NAME", new ValueString(getOsName()).withInfo("OS_NAME\n\nThe name of the operating system, one of Windows, Linux, macOS")); break;
            case "OS_VERSION": env.put("OS_VERSION", new ValueString(getOsVersion()).withInfo("OS_VERSION\n\nThe version of the operating system.")); break;
            case "OS_ARCH": env.put("OS_ARCH", new ValueString(getOsArch()).withInfo("OS_ARCH\n\nThe architecture of the operating system, one of x86, amd64.")); break;
            default:
                System.out.println(nativeName);
                throw new ControlErrorException("Unknown native " + nativeName, pos);
        }
    }

    private static String getOsName() {
        String name = System.getProperty("os.name").toLowerCase();
        if (name.contains("windows")) return "Windows";
        if (name.contains("linux")) return "Linux";
        if (name.contains("mac") || name.contains("darwin")) return "macOS";
        return "Unknown";
    }

    private static String getOsVersion() {
        return System.getProperty("os.version");
    }

    private static String getOsArch() {
        String arch = System.getProperty("os.arch").toLowerCase();
        if (arch.equals("x86") || arch.equals("i386")) return "x86";
        if (arch.equals("amd64") || arch.equals("x64")) return "amd64";
        return "Unknown";
    }

    private static void bindNative(Environment env, FuncBase func, String alias) {
        env.put(func.getName(), func);
        if (alias != null) env.put(alias, func);
    }
}
