using Terminal.Gui;

namespace App;

internal static class ElemFactory
{
    const int DefaultWidth = 40;

    internal static (Label label, TextField field) CreateTextField(string labelText, View relativeTo, string defaultValue = "", int width = DefaultWidth, int xOffset = 0, int yOffset = 0)
    {
        var yPos = Pos.Bottom(relativeTo) + yOffset;
        var label = new Label(labelText) { X = Pos.Left(relativeTo) + xOffset, Y = yPos };
        var field = new TextField(defaultValue)
        {
            X = Pos.Right(label),
            Y = yPos,
            Width = width
        };

        return (label, field);
    }

    internal static (Label label, TextField field) CreateTextField(string labelText, Pos x, Pos y, string defaultValue = "", int width = DefaultWidth)
    {
        var label = new Label(labelText) { X = x, Y = y };
        var field = new TextField(defaultValue)
        {
            X = Pos.Right(label),
            Y = Pos.Top(label),
            Width = width
        };

        return (label, field);
    }

    internal static CheckBox CreateCheckBox(string title, View reference, int xOffset = 0, int yOffset = 0) =>
        new CheckBox(title) { X = Pos.Left(reference) + xOffset, Y = Pos.Bottom(reference) + yOffset };
}