using System.Drawing;
using System.Numerics;
using ColorSpaces.Annotations;

namespace ColorSpaces;

/// <summary>
/// Represents an element of the CMYK color space.
/// All channel values are from 0 to 1.
/// </summary>
/// <typeparam name="T">
/// Type to use for channel values.
/// Must support floating point operations.
/// </typeparam>
[PublicAPI]
public readonly record struct CmykColor<T>(T Cyan, T Magenta, T Yellow, T Black, T Alpha)
    : IColor<CmykColor<T>> where T : IFloatingPoint<T>
{
    private static readonly T One = T.CreateChecked(1);
    private static readonly T MaxByte = T.CreateChecked(255);
    public Color ToColor() => Color.FromArgb(byte.CreateSaturating(T.Round(Alpha * MaxByte)),
        byte.CreateSaturating(T.Round(MaxByte * (One - Cyan) * (One - Black))),
        byte.CreateSaturating(T.Round(MaxByte * (One - Magenta) * (One - Black))),
        byte.CreateSaturating(T.Round(MaxByte * (One - Yellow) * (One - Black))));

    public static implicit operator CmykColor<T>(Color color) => FromColor(color);
    public static implicit operator Color(CmykColor<T> color) => color.ToColor();

    public static CmykColor<T> FromColor(Color color)
    {
        var r = color.R / 255d;
        var g = color.G / 255d;
        var b = color.B / 255d;
        var k = 1 - Math.Max(r, Math.Max(g, b));
        var c = T.CreateSaturating((1 - r - k) / (1 - k));
        var m = T.CreateSaturating((1 - g - k) / (1 - k));
        var y = T.CreateSaturating((1 - b - k) / (1 - k));
        return new CmykColor<T>(c, m, y,  T.CreateSaturating(k), T.CreateSaturating(color.A) / MaxByte);
    }
}