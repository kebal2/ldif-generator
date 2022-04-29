using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Terminal.Gui;

namespace App;

internal static class FormFactory
{
    public static T Get<T>(View parent, int startPositionX = 1, int startPositionY = 2) where T : new()
    {
        var form = new T();

        View referenceField = null;
        List<View> ret = new();

        _ = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute(typeof(DisplayNameAttribute)) is { })
            .Select(property =>
            {
                var field = GetFormControl<T>(startPositionX, startPositionY, property, referenceField!);

                referenceField = field.label ?? field.field;

                AddToParrent<T>(parent, field);

                SetValue(form, property, field);

                return field;
            }).ToArray();

        return form;
    }

    private static void SetValue<T>([DisallowNull] T form, PropertyInfo p, (View? label, View field) gen) where T : new()
    {
        form.GetType()
            .GetProperty(p.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(form, gen.field);
    }

    private static void AddToParrent<T>(View parent, (View? label, View field) gen) where T : new()
    {
        if (gen.label != null) parent.Add(gen.label);
        parent.Add(gen.field);
    }

    private static (View? label, View field) GetFormControl<T>(int startPositionX, int startPositionY, PropertyInfo p, View referenceField) where T : new()
    {
        var values = Array.Empty<string>();

        var name = (DisplayNameAttribute)p.GetCustomAttribute(typeof(DisplayNameAttribute))!;
        var defaultValue = p.GetCustomAttribute(typeof(DefaultValueAttribute)) as DefaultValueAttribute;

        var valueAttr = p.GetCustomAttribute(typeof(EnumDataTypeAttribute)) as EnumDataTypeAttribute;
        if (valueAttr is { }) values = Enum.GetNames(valueAttr.EnumType);

        var styleAttr = p.GetCustomAttribute(typeof(ScreenPositionAttribute)) as ScreenPositionAttribute;

        return GetView<T>(
            startPositionX,
            startPositionY,
            (title: name.DisplayName, type: p.PropertyType.Name, defaultValue: defaultValue?.Value, values, style: styleAttr),
            referenceField
        );
    }

    private static (View? label, View field) GetView<T>(int startPositionX, int startPositionY, (string title, string type, object? defaultValue, string[] values, ScreenPositionAttribute style) f, View? referenceField)
    {
        var absPos = f.style is {} && (f.style?.X != 0 || f.style?.Y != 0);
        
        var x = absPos ?  f.style?.X ?? 0: startPositionX;
        var y = absPos ?  f.style?.Y ?? 0: startPositionY;

        var offsetX = f.style?.RelativeStart ?? 0;
        var offsetY = f.style?.MarginTop ?? 0;
        
        switch (f.type)
        {
            case nameof(FieldType.TextField):
                return referenceField is null || absPos
                    ? ElemFactory.CreateTextField(f.title, x, y, f.defaultValue?.ToString() ?? "")
                    : ElemFactory.CreateTextField(f.title, referenceField, f.defaultValue?.ToString() ?? "", offsetX: offsetX, offsetY: offsetY);

            case nameof(FieldType.CheckBox):
            {
                var df = false;
                if (f.defaultValue is bool v) df = v;

                var elem = referenceField is null
                    ? ElemFactory.CreateCheckBox(f.title, (Pos)x, (Pos)y, df)
                    : ElemFactory.CreateCheckBox(f.title, referenceField, defaultValue: df, offsetX: offsetX, offsetY: offsetY);

                return (null, elem);
            }

            case nameof(FieldType.RadioGroup):
            {
                var df = 0;
                if (f.defaultValue is int v) df = v;

                return (null, referenceField is null
                    ? ElemFactory.CreateRadioGroup(f.title, f.values, (Pos)x, (Pos)y, df)
                    : ElemFactory.CreateRadioGroup(f.title, f.values, referenceField, defaultValue: df, offsetX: offsetX, offsetY: offsetY));
            }

            case nameof(FieldType.Button):
                return (null, referenceField is null
                    ? ElemFactory.CreateButton(f.title, (Pos)x, (Pos)y)
                    : ElemFactory.CreateButton(f.title, referenceField, offsetX: offsetX, offsetY: offsetY));
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private enum FieldType
    {
        TextField,
        CheckBox,
        RadioGroup,
        Button
    }
}