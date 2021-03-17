import { RuntimeError } from "./errors.mjs";
import { modulemath } from "./module_math.mjs";
import { modulecore } from "./module_core.mjs";
import { moduleio } from "./module_io.mjs";
import { modulerandom } from "./module_random.mjs";
import { modulesys } from "./module_sys.mjs";

export const moduleloader = function(modulefile, pos, fs) {
    switch (modulefile) {
        case "core.ckl": return modulecore;
        case "io.ckl": return moduleio;
        case "math.ckl": return modulemath;
        case "random.ckl": return modulerandom;
        case "sys.ckl": return modulesys;
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
