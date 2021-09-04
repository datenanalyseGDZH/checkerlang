namespace CheckerLang
{
    public class AsList
    {
        public static ValueList From(Value value, string what)
        {
            if (value.IsString())
            {
                var s = value.AsString().GetValue();
                var list = new ValueList();
                for (int i = 0; i < s.Length; i++)
                {
                    list.AddItem(new ValueString(s.Substring(i, 1)));
                }

                return list;
            }

            if (value.IsMap() && what == "keys") {
                var list = new ValueList();
                foreach (var member in value.AsMap().GetValue().Keys) {
                    list.AddItem(member);
                }
                return list;
            }

            if (value.IsMap() && what == "entries") {
                var list = new ValueList();
                foreach (var entry in value.AsMap().GetValue()) {
                    var element = new ValueList();
                    element.AddItem(entry.Key);
                    element.AddItem(entry.Value);
                    list.AddItem(element);
                }
                return list;
            }

            if (value.IsObject() && what == "keys") {
                var list = new ValueList();
                foreach (var member in value.AsObject().value.Keys) {
                    list.AddItem(new ValueString(member));
                }
                return list;
            }

            if (value.IsObject() && what == "values") {
                var list = new ValueList();
                foreach (var member in value.AsObject().value.Values) {
                    list.AddItem(member);
                }
                return list;
            }

            if (value.IsObject() && what == "entries") {
                var list = new ValueList();
                foreach (var member in value.AsObject().value)
                {
                    var element = new ValueList();
                    element.AddItem(new ValueString(member.Key));
                    element.AddItem(member.Value);
                    list.AddItem(element);
                }
                return list;
            }

            if (value.IsObject())
            {
                var list = new ValueList();
                foreach (var member in value.AsObject().value.Keys) 
                {
                    list.AddItem(new ValueString(member));
                }
                return list;
            }

            return value.AsList();
        }
    }
}
