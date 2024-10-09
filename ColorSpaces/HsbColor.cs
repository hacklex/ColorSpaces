using System.Drawing;
using System.Numerics;
using ColorSpaces.Annotations;

namespace ColorSpaces;

/// <summary>
/// Represents an element of the HSB color space.
/// https://en.wikipedia.org/wiki/HSL_and_HSV
/// </summary>
/// <param name="Hue"><typeparamref name="T"/>-valued Hue, from 0 to 360</param>
/// <param name="Saturation"><typeparamref name="T"/>-valued Saturation, from 0 to 100</param>
/// <param name="Brightness"><typeparamref name="T"/>-valued Brightness, from 0 to 100</param>
/// <param name="Alpha"><typeparamref name="T"/>-valued Alpha, from 0 to 1</param>
/// <typeparamref name="T">
/// The type to hold the color components. Must be a floating-point type.
/// </typeparamref>
[PublicAPI]
public readonly record struct HsbColor<T>(T Hue, T Saturation, T Brightness, T Alpha) : IColor<HsbColor<T>>
    where T : IFloatingPoint<T>
{
    [Pure] public static implicit operator HsbColor<T>(Color color) => FromColor(color);
    [Pure] public static implicit operator Color(HsbColor<T> color) => color.ToColor(); 
    [Pure] public override string ToString() => $@"Hue: {Hue:0}; saturation: {Saturation:0}; brightness: {Brightness:0}.";
    
    private static readonly T One = T.CreateChecked(1);
    private static readonly T Zero = T.CreateChecked(0);
    private static readonly T MaxByte = T.CreateChecked(255);

    /// <summary>
    /// Compares the current color with another color for equality.
    /// </summary>
    /// <param name="other"> The color to compare with the current color. </param>
    /// <returns>
    /// <c>true</c> if the specified color is equal to the current color; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Two colors are considered equal if their components are equal.
    /// Even if both colors have zero opacity, they will only be considered equal if all other components are also equal.
    /// </remarks>
    [Pure] public bool Equals(HsbColor<T> other) => Hue.AlmostEquals(other.Hue) &&
                                                    Saturation.AlmostEquals(other.Saturation) &&
                                                    Brightness.AlmostEquals(other.Brightness) &&
                                                    Alpha.AlmostEquals(other.Alpha);
    
    [Pure] public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Alpha.GetHashCode();
            hashCode = (hashCode * 397) ^ Hue.GetHashCode();
            hashCode = (hashCode * 397) ^ Saturation.GetHashCode();
            hashCode = (hashCode * 397) ^ Brightness.GetHashCode();
            return hashCode;
        }
    }

    [Pure] public static HsbColor<T> FromColor(Color color)
    {
        // We use double to increase the precision.
        var r = color.R / 255d;
        var g = color.G / 255d;
        var b = color.B / 255d;

        var (minValue, maxValue) = MathExtensions.GetMinMax(r, g, b);
        
        var delta = maxValue - minValue;

        double hue = 0;
        double saturation;
        var brightness = maxValue * 100;

        if (maxValue.IsZero() || delta.IsZero())
        {
            hue = 0;
            saturation = 0;
        }
        else
        {
            // The initial version had a mistake here. minValue was compared to zero,
            // instead of maxValue. Also, according to the wiki, the correct fallback is 0, not 100.
            saturation = maxValue.IsZero() ? 0 : delta / maxValue * 100;
            
            if ((r - maxValue).IsZero()) hue = (g - b) / delta;
            else if ((g - maxValue).IsZero()) hue = 2 + (b - r) / delta;
            else if ((b - maxValue).IsZero()) hue = 4 + (r - g) / delta;
        }

        hue = (hue < 0 ? hue + 6 : hue) * 60;

        return new(
            T.CreateSaturating(hue), 
            T.CreateSaturating(saturation), 
            T.CreateSaturating(brightness),
            T.CreateSaturating(color.A / 255.0));
    }
    
    
    [Pure] public Color ToColor()
    {
        T red, green, blue;
        T hundred = T.CreateChecked(100);
        T fullCircle = T.CreateChecked(360);
        T sixty = T.CreateChecked(60);
        
        T h = Hue, s = Saturation / hundred, b = Brightness / hundred;

        if (s.IsZero()) red = green = blue = b;
        else
        {
            // the color wheel has six sectors.
            // Without this WrapAround, the smooth transition between 360 and 0 will be broken
            var sectorPosition = h.WrapAround(Zero, fullCircle) / sixty;
            var floor = sectorPosition - (sectorPosition % One);
            var sectorNumber = int.CreateChecked(floor);
            var fractionalSector = sectorPosition - T.CreateChecked(sectorNumber);

            var p = b * (One - s);
            var q = b * (One - s * fractionalSector);
            var t = b * (One - s * (One - fractionalSector));

            // We save vertical screen space by using tuple deconstruction.
            (red, green, blue) = sectorNumber switch
            {
                0 => (b, t, p),
                1 => (q, b, p),
                2 => (p, b, t),
                3 => (p, q, b),
                4 => (t, p, b),
                _ => (b, p, q)
            };
        }
        
        var redByte = byte.CreateSaturating(red * MaxByte);
        var greenByte = byte.CreateSaturating(green * MaxByte);
        var blueByte = byte.CreateSaturating(blue * MaxByte);
        var alphaByte = byte.CreateSaturating(Alpha * MaxByte);
        return Color.FromArgb(alphaByte, redByte, greenByte, blueByte);    
    }
}