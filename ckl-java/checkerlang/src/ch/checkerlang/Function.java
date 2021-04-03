package ch.checkerlang;

import ch.checkerlang.nodes.Node;
import ch.checkerlang.nodes.NodeSpread;
import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueFunc;
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueMap;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class Function {

    public static Value invoke(ValueFunc fn, List<String> names_, List<Node> args, Environment environment, SourcePos pos) {
        List<Value> values = new ArrayList<>();
        List<String> names = new ArrayList<>();
        for (int i = 0; i < args.size(); i++) {
            Node arg = args.get(i);
            if (arg instanceof NodeSpread) {
                Value argvalue = arg.evaluate(environment);
                if (argvalue.isMap()) {
                    ValueMap map = argvalue.asMap();
                    for (Map.Entry<Value, Value> entry : map.getValue().entrySet()) {
                        values.add(entry.getValue());
                        if (entry.getKey().isString()) {
                            names.add(entry.getKey().asString().getValue());
                        } else {
                            names.add(null);
                        }
                    }
                } else {
                    ValueList list = argvalue.asList();
                    for (Value value : list.getValue()) {
                        values.add(value);
                        names.add(null);
                    }
                }
            } else {
                values.add(arg.evaluate(environment));
                names.add(names_.get(i));
            }
        }

        Args args_ = new Args(fn.asFunc().getArgNames(), pos);
        args_.setArgs(names, values);

        try {
            return fn.asFunc().execute(args_, environment, pos);
        } catch (ControlErrorException e) {
            e.addStacktraceElement(getFuncallString(fn.asFunc(), args_), pos);
            throw e;
        }
    }

    public static String getFuncallString(ValueFunc fn, Args args) {
        return fn.getName() + "(" + args.toStringAbbrev() + ")";
    }

}
