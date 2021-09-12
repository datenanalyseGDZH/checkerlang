package ch.checkerlang;

import java.io.*;
import java.nio.charset.StandardCharsets;

public class ModuleLoader {
    public static String loadModule(String moduleidentifier, SourcePos pos) {
        try {
            InputStream strm = ModuleLoader.class.getResourceAsStream("/module-" + moduleidentifier.toLowerCase());
            if (strm == null) strm = getModuleStream(moduleidentifier);
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

    private static InputStream getModuleStream(String moduleidentifier) throws IOException {
        File moduledir = new File(new File(System.getProperty("user.home"), ".ckl"), "modules");
        File module = new File(moduledir, new File(moduleidentifier).getName());
        if (module.exists()) return new FileInputStream(module);
        module = new File(".", new File(moduleidentifier).getName());
        if (module.exists()) return new FileInputStream(module);
        return null;
    }
}
