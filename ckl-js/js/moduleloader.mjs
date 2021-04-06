import { RuntimeError } from "./errors.mjs";
import { system } from "./system.mjs";
import { modulecore } from "./module_core.mjs";
import { moduledate } from "./module_date.mjs";
import { moduleio } from "./module_io.mjs";
import { modulelist } from "./module_list.mjs";
import { modulemath } from "./module_math.mjs";
import { modulepredicate } from "./module_predicate.mjs";
import { modulerandom } from "./module_random.mjs";
import { moduleset } from "./module_set.mjs";
import { modulestat } from "./module_stat.mjs";
import { modulestring } from "./module_string.mjs";
import { modulesys } from "./module_sys.mjs";
import { moduletype } from "./module_type.mjs";

export const moduleloader = function(modulefile, pos) {
    switch (modulefile) {
        case "Core.ckl": return modulecore;
        case "Date.ckl": return moduledate;
        case "IO.ckl": return moduleio;
        case "List.ckl": return modulelist;
        case "Math.ckl": return modulemath;
        case "Predicate.ckl": return modulepredicate;
        case "Random.ckl": return modulerandom;
        case "Set.ckl": return moduleset;
        case "Stat.ckl": return modulestat;
        case "String.ckl": return modulestring;
        case "Sys.ckl": return modulesys;
        case "Type.ckl": return moduletype;
        default:
            if (system.fs !== undefined && system.fs !== null) {
                // TODO prevent directory traversal, but allow some relative paths?!
                // TODO configure base module path
                const path = modulefile.replace(/\\/g, "/").split("/");
                const filename = path[path.length - 1];
                if (!system.fs.existsSync(filename)) throw new RuntimeError("ERROR", "Module " + modulefile.substr(0, modulefile.length - 4) + " not found", pos);
                return system.fs.readFileSync(filename, {encoding: 'utf8', flag: 'r'});
            } else {
                throw new RuntimeError("ERROR", "Module " + modulefile.substr(0, modulefile.length - 4) + " not found", pos);
            }
    }
};
