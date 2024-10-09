using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using ColorSpaces.Annotations;

namespace ColorSpaces;

/// <summary>
/// Represents an element of the RGB color space.
/// Channel values are from 0 to 1.
/// </summary>
/// <param name="Red">Red channel value</param>
/// <param name="Green">Green channel value</param>
/// <param name="Blue">Blue channel value</param>
/// <param name="Alpha">Alpha channel value</param>
[PublicAPI]
public readonly record struct FloatRgbColor<T>(T Red, T Green, T Blue, T Alpha) 
    : IColor<FloatRgbColor<T>> 
    where T : IFloatingPoint<T> 
{
    private static readonly T MaxByte = T.CreateChecked(255);
    public FloatRgbColor(T red, T green, T blue) : this(red, green, blue, T.CreateChecked(1)) { }
    [Pure] public static implicit operator FloatRgbColor<T>(Color color) => FromColor(color);
    [Pure] public static implicit operator Color(FloatRgbColor<T> color) => color.ToColor(); 
    [Pure] public static FloatRgbColor<T> FromColor(Color color) => new(
        T.CreateChecked(color.R) / MaxByte,
        T.CreateChecked(color.G) / MaxByte,
        T.CreateChecked(color.B) / MaxByte,
        T.CreateChecked(color.A) / MaxByte);
        
    [Pure] public Color ToColor() => Color.FromArgb(
        byte.CreateSaturating(T.Round(Alpha * MaxByte)),
        byte.CreateSaturating(T.Round(Red * MaxByte)),
        byte.CreateSaturating(T.Round(Green * MaxByte)),
        byte.CreateSaturating(T.Round(Blue * MaxByte)));
}