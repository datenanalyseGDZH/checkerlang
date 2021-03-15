import { RuntimeError } from "./errors.mjs";
import { modulemath } from "./module_math.mjs";

export const moduleloader = function(modulefile, pos, fs) {
    if (!modulefile.startsWith(".")) {
        let module = modulefile;
        if (module.endsWith(".ckl")) module = module.substr(0, module.length - 4);
        switch (module) {
            case "math": return modulemath;
            default: throw new RuntimeError("unknown system module " + module, pos);
        }
    } else {
        return fs.readFileSync(modulefile, {encoding: 'utf8', flag: 'r'});
    }
};
