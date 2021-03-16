import { RuntimeError } from "./errors.mjs";
import { modulemath } from "./module_math.mjs";

export const moduleloader = function(modulefile, pos, fs) {
    switch (modulefile) {
        case "math.ckl": return modulemath;
        // TODO add other system modules
        default:
            // TODO prevent directory traversal, but allow some relative paths?!
            // TODO configure base module path
            const path = modulefile.replace(/\\/g, "/").split("/");
            const filename = path[path.length - 1];
            if (!fs.existsSync(filename)) throw new RuntimeError("unknown system module " + modulefile.substr(0, modulefile.length - 4), pos);
            return fs.readFileSync(filename, {encoding: 'utf8', flag: 'r'});
    }
};
