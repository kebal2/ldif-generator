using Terminal.Gui;

namespace App;

internal static class FormFactory
{
    internal enum FieldType
    {
        TextField,
        Checkbox
    }

    public static IList<View> Get((FieldType t, string title, string defaultValue)[] fields, int startPositionX = 1, int startPositionY = 2)
    {
        List<View> ret = new();
        View referenceField = null;
        foreach (var f in fields)
        {
            switch (f.t)
            {
                case FieldType.TextField:
                {
                    var elem = referenceField is null
                        ? ElemFactory.CreateTextField(f.title, startPositionX, startPositionY, f.defaultValue)
                        : ElemFactory.CreateTextField(f.title, referenceField, f.defaultValue);

                    referenceField = elem.label;
                    ret.Add(elem.label);
                    ret.Add(elem.field);
                }
                    break;
                case FieldType.Checkbox:
                {
                    var elem = referenceField is null
                        ? ElemFactory.CreateCheckBox(f.title, (Pos)startPositionX, (Pos)startPositionY, bool.Parse(f.defaultValue))
                        : ElemFactory.CreateCheckBox(f.title, referenceField, defaultValue: bool.Parse(f.defaultValue));
                    ret.Add(elem);
                    referenceField = elem;
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return ret;
    }
}