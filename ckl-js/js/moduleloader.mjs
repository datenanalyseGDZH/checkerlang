import { RuntimeError } from "./errors.mjs";
import { system } from "./system.mjs";
import { modulecore } from "./module_core.mjs";
import { moduleio } from "./module_io.mjs";
import { modulelists } from "./module_lists.mjs";
import { modulemath } from "./module_math.mjs";
import { modulerandom } from "./module_random.mjs";
import { modulesets } from "./module_sets.mjs";
import { modulestat } from "./module_stat.mjs";
import { modulesys } from "./module_sys.mjs";

export const moduleloader = function(modulefile, pos) {
    switch (modulefile) {
        case "core.ckl": return modulecore;
        case "io.ckl": return moduleio;
        case "lists.ckl": return modulelists;
        case "math.ckl": return modulemath;
        case "random.ckl": return modulerandom;
        case "sets.ckl": return modulesets;
        case "stat.ckl": return modulestat;
        case "sys.ckl": return modulesys;
        default:
            if (system.fs !== undefined && system.fs !== null) {
                // TODO prevent directory traversal, but allow some relative paths?!
                // TODO configure base module path
                const path = modulefile.replace(/\\/g, "/").split("/");
                const filename = path[path.length - 1];
                if (!system.fs.existsSync(filename)) throw new RuntimeError("Module " + modulefile.substr(0, modulefile.length - 4) + " not found", pos);
                return system.fs.readFileSync(filename, {encoding: 'utf8', flag: 'r'});
            } else {
                throw new RuntimeError("Module " + modulefile.substr(0, modulefile.length - 4) + " not found", pos);
            }
    }
};
