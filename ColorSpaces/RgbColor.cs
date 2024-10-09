using System.Drawing;
using ColorSpaces.Annotations;

namespace ColorSpaces;

/// <summary>
/// Represents an element of the RGB color space.
/// All channel values are from 0 to 255.
/// </summary>
[PublicAPI]
public readonly record struct RgbColor(byte Red, byte Green, byte Blue, byte Alpha) : IColor<RgbColor>
{
    [Pure] public static implicit operator RgbColor(Color color) => FromColor(color);
    [Pure] public static implicit operator Color(RgbColor color) => color.ToColor(); 
    [Pure] public override string ToString() => Alpha < 255 
        ? $@"rgba({Red}, {Green}, {Blue}, {Alpha / 255d})" 
        : $@"rgb({Red}, {Green}, {Blue})";

    /// <summary>
    /// Compares the current color with another color for equality.
    /// </summary>
    /// <param name="other">
    /// The color to compare with the current color.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified color is equal to the current color; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method compares the colors component-wise.
    /// Two colors with zero opacity are NOT considered equal unless all other components are also equal.
    /// </remarks>
    [Pure] public bool Equals(RgbColor other) => Red == other.Red &&
                                                 Green == other.Green &&
                                                 Blue == other.Blue &&
                                                 Alpha == other.Alpha;

    
    
    // When the entire structure fits into a single integer, we can use it as its own hash code.
    [Pure]
    public override int GetHashCode() => ToColor().ToArgb();
    [Pure] public static RgbColor FromColor(Color color) => new(color.R, color.G, color.B, color.A);
    [Pure] public Color ToColor() => Color.FromArgb(Alpha, Red, Green, Blue);
}