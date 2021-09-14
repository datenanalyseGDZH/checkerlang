package ch.checkerlang;

import java.io.*;
import java.nio.charset.StandardCharsets;

import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueList;

public class ModuleLoader {
    public static String loadModule(String moduleidentifier, Environment environment, SourcePos pos) {
        try {
            InputStream strm = ModuleLoader.class.getResourceAsStream("/module-" + moduleidentifier.toLowerCase());
            if (strm == null) strm = getModuleStream(moduleidentifier, environment, pos);
            if (strm == null) throw new ControlErrorException("Module " + new File(moduleidentifier).getName() + " not found", pos);
            BufferedReader rdr = new BufferedReader(new InputStreamReader(strm, StandardCharsets.UTF_8));
            StringBuilder result = new StringBuilder();
            try {
                String line = rdr.readLine();
                while (line != null) {
                    result.append(line).append("\n");
                    line = rdr.readLine();
                }
            } finally {
                rdr.close();
            }
            return result.toString();
        } catch (IOException e) {
            throw new ControlErrorException("Module " + new File(moduleidentifier).getName() + " not found", pos);
        }
    }

    private static InputStream getModuleStream(String moduleidentifier, Environment environment, SourcePos pos) throws IOException {
        File moduledir = new File(new File(System.getProperty("user.home"), ".ckl"), "modules");
        File module = new File(moduledir, new File(moduleidentifier).getName());
        if (module.exists()) return new FileInputStream(module);
        if (environment.isDefined("checkerlang_module_path")) {
            ValueList modulepath = environment.get("checkerlang_module_path", pos).asList();
            for (Value dir : modulepath.getValue()) {
                module = new File(dir.asString().getValue(), new File(moduleidentifier).getName());
                if (module.exists()) return new FileInputStream(module);
            }
        }
        module = new File(".", new File(moduleidentifier).getName());
        if (module.exists()) return new FileInputStream(module);
        return null;
    }
}
