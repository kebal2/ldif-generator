using NStack;

using Terminal.Gui;

namespace App;

internal static class ElemFactory
{
    private const int DefaultWidth = 40;

    internal static (Label label, TextField field) CreateTextField(string labelText, Pos x, Pos y, string defaultValue = "", int width = DefaultWidth)
    {
        var label = new Label(labelText) { X = x, Y = y };
        var field = new TextField(defaultValue)
        {
            X = Pos.Right(label),
            Y = Pos.Top(label),
            Width = width,
            Id = labelText
        };

        return (label, field);
    }

    internal static (Label label, TextField field) CreateTextField(string labelText, View relativeTo, string defaultValue = "", int width = DefaultWidth, int xOffset = 0, int yOffset = 0)
    {
        return CreateTextField(labelText, Pos.Left(relativeTo) + xOffset, Pos.Bottom(relativeTo) + yOffset, defaultValue, width);
    }

    internal static CheckBox CreateCheckBox(string title, Pos x, Pos y, bool defaultValue = false)
    {
        return new(title) { X = x, Y = y, Checked = defaultValue, Id = title };
    }

    internal static CheckBox CreateCheckBox(string title, View reference, int xOffset = 0, int yOffset = 0, bool defaultValue = false)
    {
        return CreateCheckBox(title, Pos.Left(reference) + xOffset, Pos.Bottom(reference) + yOffset, defaultValue);
    }

    internal static (FrameView frame, RadioGroup field) CreateFramedRadioGroup(string title, string[] values, Pos x, Pos y, int defaultValue = 0)
    {
        var frame = new FrameView((string?)title)
        {
            X = x,
            Y = y,
            Width = title.Length + 6, Height = 4
        };

        var radioGroup = new RadioGroup(values.Select(ustring.Make).ToArray(), defaultValue);

        frame.Add(radioGroup);

        return (frame, radioGroup);
    }

    internal static (FrameView frame, RadioGroup field) CreateFramedRadioGroup(string title, string[] values, View reference, int xOffset = 0, int yOffset = 0, int defaultValue = 0)
    {
        var xPos = Pos.Left(reference) + xOffset;
        var yPos = Pos.Bottom(reference) + 1 + yOffset;

        return CreateFramedRadioGroup(title, values, xPos, yPos, defaultValue);
    }

    internal static RadioGroup CreateRadioGroup(string title, string[] values, Pos x, Pos y, int defaultValue = 0)
    {
        return new(values.Select(ustring.Make).ToArray(), defaultValue) { X = x, Y = y };
    }

    internal static RadioGroup CreateRadioGroup(string title, string[] values, View reference, int xOffset = 0, int yOffset = 0, int defaultValue = 0)
    {
        var xPos = Pos.Left(reference) + xOffset;
        var yPos = Pos.Bottom(reference) + 1 + yOffset;

        return CreateRadioGroup(title, values, xPos, yPos, defaultValue);
    }

    public static Button CreateButton(string title, Pos x, Pos y)
    {
        return new(title) { X = x, Y = y };
    }

    public static Button CreateButton(string title, View referenceField, int offsetX = 0, int offsetY = 0)
    {
        return CreateButton(title, Pos.Left(referenceField) + offsetX, Pos.Bottom(referenceField) + offsetY);
    }
}