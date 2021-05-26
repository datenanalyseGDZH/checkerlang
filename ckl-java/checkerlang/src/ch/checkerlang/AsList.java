package ch.checkerlang;

import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueString;

public class AsList {

    public static ValueList from(Value value)  {
        if (value.isString()) {
            String s = value.asString().getValue();
            ValueList list = new ValueList();
            for (int i = 0; i < s.length(); i++) {
                list.addItem(new ValueString(s.substring(i, i + 1)));
            }
            return list;
        }

        if (value.isObject()) {
            ValueList list = new ValueList();
            for (String member : value.asObject().value.keySet()) {
                list.addItem(new ValueString(member));
            }
            return list;
        }

        return value.asList();
    }
}
