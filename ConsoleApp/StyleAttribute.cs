namespace App;

[AttributeUsage(AttributeTargets.Property)]
public class ScreenPositionAttribute : Attribute
{
    /// <summary>
    /// Relative to previously generated field
    /// </summary>
    public int RelativeStart { get; set; }
    
    /// <summary>
    /// Margin top
    /// </summary>
    public int MarginTop { get; set; }

    /// <summary>
    /// Absolute X position
    /// </summary>
    public int X { get; set; }
    /// <summary>
    /// Absolute Y position
    /// </summary>
    public int Y { get; set; }
}