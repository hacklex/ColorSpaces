using System.Drawing;
using ColorSpaces.Annotations;

namespace ColorSpaces;

public interface IColor
{ 
    /// <summary>
    /// Compares the current color with another color for visual equality.
    /// Two colors of zero opacity are considered equal, even if their other components differ.
    /// The comparison is done by converting both the current color and the other color to <see cref="Color"/> instances.
    /// </summary>
    /// <param name="other">
    /// The color to compare with the current color.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified color is visually equal to the current color; otherwise, <c>false</c>.
    /// </returns>
    [Pure] bool IsVisuallyEqualTo(IColor other);
    
    /// <summary>
    /// Converts the current color to a <see cref="Color"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="Color"/> instance that represents the current color.
    /// </returns>
    [Pure] Color ToColor();
}
 

/// <summary>
/// Defines a color in some color space.
/// </summary>
[PublicAPI]
public interface IColor<T> : IColor where T:IColor<T>
{
    /// <summary>
    /// Converts the current <see cref="Color"/> to <typeparamref name="T"/>.
    /// </summary>
    /// <param name="color"><see cref="Color"/> to convert</param>
    /// <returns>
    /// A <typeparamref name="T"/> that represents the current color.
    /// </returns>
    [Pure] static abstract implicit operator T(Color color);
    /// <summary>
    /// Converts the current <see cref="Color"/> to <typeparamref name="T"/>.
    /// </summary>
    /// <param name="color">
    /// A <typeparamref name="T"/> to convert.
    /// </param>
    /// <returns></returns>
    [Pure] static abstract implicit operator Color(T color);
    
    /// <summary>
    /// Constructs <typeparamref name="T"/> from the specified <see cref="Color"/>.
    /// </summary>
    /// <param name="color">
    /// The color to convert.
    /// </param>
    /// <returns>
    /// The <typeparamref name="T"/> instance that represents the specified RGB color.
    /// </returns>
    [Pure] static abstract T FromColor(Color color);
    
    [Pure] bool IColor.IsVisuallyEqualTo(IColor other) => IsVisuallyEqualTo(other.ToColor());
    
    /// <summary>
    /// Compares the current color with a <see cref="Color"/> for visual equality.
    /// Two colors of zero opacity are considered equal, even if their other components differ.
    /// The comparison is done by converting the current color to a <see cref="Color"/> instance.
    /// </summary>
    /// <param name="other">
    /// The color to compare with the current color.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified color is visually equal to the current color; otherwise, <c>false</c>.
    /// </returns>
    [Pure] bool IsVisuallyEqualTo(Color other)
    {
        var thisColor = ToColor();
        if (thisColor.A == 0 && other.A == 0) return true;
        return thisColor == other;
    }
    /// <summary>
    /// Compares the current color with another color for visual equality.
    /// Two colors of zero opacity are considered equal, even if their other components differ.
    /// The comparison is done by converting both the current color and the other color to <see cref="Color"/> instances.
    /// </summary>
    /// <param name="other">
    /// The color to compare with the current color.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified color is visually equal to the current color; otherwise, <c>false</c>.
    /// </returns>
    [Pure] bool IsVisuallyEqualTo<TOther>(IColor<TOther> other) where TOther : IColor<TOther> 
        => IsVisuallyEqualTo(other.ToColor());
}