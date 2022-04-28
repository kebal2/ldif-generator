using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Terminal.Gui;

namespace App;

internal static class FormFactory
{
    public static T Get<T>(View parent, int startPositionX = 1, int startPositionY = 3) where T : new()
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

        return GetView<T>(
            startPositionX,
            startPositionY,
            (title: name.DisplayName, type: p.PropertyType.Name, defaultValue: defaultValue?.Value, values),
            referenceField
        );
    }

    private static (View? label, View field) GetView<T>(int startPositionX, int startPositionY, (string title, string type, object? defaultValue, string[] values) f, View? referenceField)
    {
        switch (f.type)
        {
            case nameof(FieldType.TextField):
                return referenceField is null
                    ? ElemFactory.CreateTextField(f.title, startPositionX, startPositionY, f.defaultValue?.ToString() ?? "")
                    : ElemFactory.CreateTextField(f.title, referenceField, f.defaultValue?.ToString() ?? "");

            case nameof(FieldType.CheckBox):
            {
                var df = false;
                if (f.defaultValue is bool v) df = v;

                var elem = referenceField is null
                    ? ElemFactory.CreateCheckBox(f.title, (Pos)startPositionX, (Pos)startPositionY, df)
                    : ElemFactory.CreateCheckBox(f.title, referenceField, defaultValue: df);

                return (null, elem);
            }

            case nameof(FieldType.RadioGroup):
            {
                var df = 0;
                if (f.defaultValue is int v) df = v;

                return (null, referenceField is null
                    ? ElemFactory.CreateRadioGroup(f.title, f.values, (Pos)startPositionX, (Pos)startPositionY, df)
                    : ElemFactory.CreateRadioGroup(f.title, f.values, referenceField, defaultValue: df));
            }

            case nameof(FieldType.Button):
                return (null, referenceField is null
                    ? ElemFactory.CreateButton(f.title, (Pos)startPositionX, (Pos)startPositionY)
                    : ElemFactory.CreateButton(f.title, referenceField));
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