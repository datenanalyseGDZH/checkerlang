namespace CheckerLang
{
    public class AsList
    {
        public static ValueList From(Value value)
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
