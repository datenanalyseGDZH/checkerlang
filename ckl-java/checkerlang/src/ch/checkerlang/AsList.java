package ch.checkerlang;

import ch.checkerlang.values.Value;
import ch.checkerlang.values.ValueList;
import ch.checkerlang.values.ValueString;

import java.util.Map;

public class AsList {

    public static ValueList from(Value value, String what)  {
        if (value.isString()) {
            String s = value.asString().getValue();
            ValueList list = new ValueList();
            for (int i = 0; i < s.length(); i++) {
                list.addItem(new ValueString(s.substring(i, i + 1)));
            }
            return list;
        }

        if (value.isMap() && what.equals("keys")) {
            ValueList list = new ValueList();
            for (Value member : value.asMap().getValue().keySet()) {
                list.addItem(member);
            }
            return list;
        }

        if (value.isMap() && what.equals("entries")) {
            ValueList list = new ValueList();
            for (Map.Entry<Value, Value> entry : value.asMap().getValue().entrySet()) {
                ValueList element = new ValueList();
                element.addItem(entry.getKey());
                element.addItem(entry.getValue());
                list.addItem(element);
            }
            return list;
        }

        if (value.isObject() && what.equals("keys")) {
            ValueList list = new ValueList();
            for (String member : value.asObject().value.keySet()) {
                list.addItem(new ValueString(member));
            }
            return list;
        }

        if (value.isObject() && what.equals("values")) {
            ValueList list = new ValueList();
            for (Value member : value.asObject().value.values()) {
                list.addItem(member);
            }
            return list;
        }

        if (value.isObject() && what.equals("entries")) {
            ValueList list = new ValueList();
            for (Map.Entry<String, Value> entry : value.asObject().value.entrySet()) {
                ValueList element = new ValueList();
                element.addItem(new ValueString(entry.getKey()));
                element.addItem(entry.getValue());
                list.addItem(element);
            }
            return list;
        }

        return value.asList();
    }
}
