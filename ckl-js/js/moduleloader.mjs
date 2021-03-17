import { RuntimeError } from "./errors.mjs";
import { modulemath } from "./module_math.mjs";
import { modulecore } from "./module_core.mjs";
import { moduleio } from "./module_io.mjs";

export const moduleloader = function(modulefile, pos, fs) {
    switch (modulefile) {
        case "core.ckl": return modulecore;
        case "io.ckl": return moduleio;
        case "math.ckl": return modulemath;
        // TODO add other system modules
        default:
            if (fs !== undefined && fs !== null) {
                // TODO prevent directory traversal, but allow some relative paths?!
                // TODO configure base module path
                const path = modulefile.replace(/\\/g, "/").split("/");
                const filename = path[path.length - 1];
                if (!fs.existsSync(filename)) throw new RuntimeError("Module " + modulefile.substr(0, modulefile.length - 4) + " not found", pos);
                return fs.readFileSync(filename, {encoding: 'utf8', flag: 'r'});
            } else {
                throw new RuntimeError("Module " + modulefile.substr(0, modulefile.length - 4) + " not found", pos);
            }
    }
};
