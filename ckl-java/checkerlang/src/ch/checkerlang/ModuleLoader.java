package ch.checkerlang;

import java.io.*;
import java.nio.charset.StandardCharsets;

public class ModuleLoader {
    public static String loadModule(String moduleidentifier, SourcePos pos) {
        try {
            InputStream strm = ModuleLoader.class.getResourceAsStream("/module-" + moduleidentifier);
            if (strm == null) strm = new FileInputStream("./" + new File(moduleidentifier).getName());
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
}
