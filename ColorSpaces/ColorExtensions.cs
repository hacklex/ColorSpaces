using System.Drawing;
using System.Numerics;
using ColorSpaces.Annotations;

namespace ColorSpaces;

/// <summary>
/// Provides extension methods for color space conversion.
/// </summary>
/// <remarks>
/// https://en.wikipedia.org/wiki/HSL_and_HSV
/// </remarks>
[PublicAPI]
public static class ColorExtensions
{
    /// <summary>
    /// Checks if the current color is visually equal to another color.
    /// The visual equality is determined in the RGB color space.
    /// All fully transparent colors are considered visually equal.
    /// </summary>
    /// <param name="color">
    /// The current color.
    /// </param>
    /// <param name="other">
    /// The color to compare with the current color.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified color is visually equal to the current color; otherwise, <c>false</c>.
    /// </returns>
    [Pure] public static bool IsVisuallyEqualTo(this IColor color, IColor other) => color.IsVisuallyEqualTo(other);
    /// <inheritdoc cref="IsVisuallyEqualTo"/>
    [Pure] public static bool IsVisuallyEqualTo<T>(this IColor<T> color, Color other) where T : IColor<T> => color.IsVisuallyEqualTo(other);
    /// <inheritdoc cref="IsVisuallyEqualTo"/>
    [Pure] public static bool IsVisuallyEqualTo<T>(this Color color, IColor<T> other) where T : IColor<T> => other.IsVisuallyEqualTo(color);
    /// <inheritdoc cref="IsVisuallyEqualTo"/>
    [Pure] public static bool IsVisuallyEqualTo<T>(this Color color, Color other) where T : IColor<T> => RgbColor.FromColor(color).IsVisuallyEqualTo(other);

    /// <summary>Converts the current color to a <see cref="CmykColor{T}"/>.</summary>
    /// <param name="color">The current color.</param>
    /// <typeparam name="T">The type to hold the color components. Must be a floating-point type.</typeparam>
    /// <returns>The <see cref="CmykColor{T}"/> representation of the current color.</returns>
    [Pure] public static CmykColor<T> ToCmykColor<T>(this Color color) where T : IFloatingPoint<T> => CmykColor<T>.FromColor(color);
    
    /// <summary>Converts the current color to a <see cref="HsbColor{T}"/>.</summary>
    /// <param name="color">The current color.</param>
    /// <typeparam name="T">The type to hold the color components. Must be a floating-point type.</typeparam>
    /// <returns>The <see cref="HsbColor{T}"/> representation of the current color.</returns>
    [Pure] public static HsbColor<T> ToHsbColor<T>(this Color color) where T : IFloatingPoint<T> => HsbColor<T>.FromColor(color);
    /// <summary>Converts the current color to a <see cref="HslColor{T}"/>.</summary>
    /// <param name="color">The current color.</param>
    /// <typeparam name="T">The type to hold the color components. Must be a floating-point type.</typeparam>
    /// <returns>The <see cref="HslColor{T}"/> representation of the current color.</returns>
    [Pure] public static HslColor<T> ToHslColor<T>(this Color color) where T : IFloatingPoint<T> => HslColor<T>.FromColor(color);
    /// <summary>Converts the current color to a <see cref="RgbColor"/>.</summary>
    /// <param name="color">The current color.</param>
    /// <returns>The <see cref="RgbColor"/> representation of the current color.</returns>
    [Pure] public static RgbColor ToRgbColor(this Color color) => RgbColor.FromColor(color);
    /// <summary>Converts the current color to a <see cref="FloatRgbColor{T}"/>.</summary>
    /// <param name="color">The current color.</param>
    /// <typeparam name="T">The type to hold the color components. Must be a floating-point type.</typeparam>
    /// <returns>The <see cref="FloatRgbColor{T}"/> representation of the current color.</returns>
    [Pure] public static FloatRgbColor<T> ToFloatRgbColor<T>(this Color color) where T : IFloatingPoint<T> => FloatRgbColor<T>.FromColor(color);

    /// <summary>Converts the current color to a <see cref="ByteYuvColor"/>.</summary>
    /// <param name="color">The current color.</param>
    /// <returns>The <see cref="ByteYuvColor"/> representation of the current color.</returns>
    [Pure] public static ByteYuvColor ToByteYuvColor(this Color color) => ByteYuvColor.FromColor(color);
    
    /// <inheritdoc cref="ToCmykColor{T}"/>
    [Pure] public static CmykColor<TNum> ToCmykColor<T, TNum>(this IColor<T> color) where T : IColor<T> where TNum: IFloatingPoint<TNum> => color.ToColor().ToCmykColor<TNum>();
    [Pure] public static HsbColor<TNum> ToHsbColor<T, TNum>(this IColor<T> color) where T : IColor<T> where TNum: IFloatingPoint<TNum> => color.ToColor().ToHsbColor<TNum>();
    [Pure] public static HslColor<TNum> ToHslColor<T, TNum>(this IColor<T> color) where T : IColor<T> where TNum: IFloatingPoint<TNum> => color.ToColor().ToHslColor<TNum>();
    [Pure] public static RgbColor ToRgbColor<T>(this IColor<T> color) where T : IColor<T> => color.ToColor().ToRgbColor();
    [Pure] public static FloatRgbColor<TNum> ToFloatRgbColor<T, TNum>(this IColor<T> color) where T : IColor<T> where TNum: IFloatingPoint<TNum> => color.ToColor().ToFloatRgbColor<TNum>();
    [Pure] public static ByteYuvColor ToByteYuvColor<T>(this IColor<T> color) where T : IColor<T> => color.ToColor().ToByteYuvColor();
}