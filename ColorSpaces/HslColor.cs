using System.Drawing;
using System.Numerics;
using ColorSpaces.Annotations;

namespace ColorSpaces;

/// <summary>
/// Represents an element of the HSL color space.
/// https://en.wikipedia.org/wiki/HSL_and_HSV
/// </summary>
/// <param name="Hue"><typeparamref name="T"/>-valued Hue, from 0 to 360</param>
/// <param name="Saturation"><typeparamref name="T"/>-valued Saturation, from 0 to 100</param>
/// <param name="Light"><typeparamref name="T"/>-valued Light, from 0 to 100</param>
/// <param name="Alpha"><typeparamref name="T"/>-valued Alpha, from 0 to 1</param>
/// <typeparamref name="T">
/// The type to hold the color components. Must be a floating-point type.
/// </typeparamref>
[PublicAPI]
public readonly record struct HslColor<T>(T Hue, T Saturation, T Light, T Alpha) : IColor<HslColor<T>>
    where T : IFloatingPoint<T>
{
    private static readonly T One = T.CreateChecked(1);
    private static readonly T Zero = T.CreateChecked(0);
    private static readonly T MaxByte = T.CreateChecked(255);
    
    [Pure] public static implicit operator Color(HslColor<T> color) => color.ToColor();
    [Pure] public static implicit operator HslColor<T>(Color color) => FromColor(color);
    [Pure] public override string ToString() => Alpha < One
        ? $@"hsla({Hue:0}, {Saturation:0}%, {Light:0}%, {Alpha})"
        : $@"hsl({Hue:0}, {Saturation:0}%, {Light:0}%)";

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
    [Pure] public bool Equals(HslColor<T> other) => Hue.AlmostEquals(other.Hue) &&
                                                    Saturation.AlmostEquals(other.Saturation) &&
                                                    Light.AlmostEquals(other.Light) &&
                                                    Alpha.AlmostEquals(other.Alpha);
    
    [Pure] public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Alpha.GetHashCode();
            hashCode = (hashCode * 397) ^ Hue.GetHashCode();
            hashCode = (hashCode * 397) ^ Saturation.GetHashCode();
            hashCode = (hashCode * 397) ^ Light.GetHashCode();
            return hashCode;
        }
    }

    [Pure] private static T Hue2Rgb(T v1, T v2, T vH)
    {
        T two = T.CreateChecked(2);
        T six = T.CreateChecked(6);
        T three = T.CreateChecked(3);
        vH = vH.WrapAround(Zero, One);
        if (six * vH < One) return v1 + (v2 - v1) * six * vH;
        if (two * vH < One) return v2;
        if (three * vH < two) return v1 + (v2 - v1) * (two / three - vH) * six;
        return v1;
    }
    
    [Pure] public static HslColor<T> FromColor(Color color)
    { 
        
        var varR = color.R / 255.0; //input values are from 0 to 255 inclusive
        var varG = color.G / 255.0;
        var varB = color.B / 255.0;

        var (minChannel, maxChannel) = MathExtensions.GetMinMax(varR, varG, varB);
        var maxDelta = maxChannel - minChannel; //Delta RGB value

        double h, l = (maxChannel + minChannel) / 2;

        if (maxDelta.IsZero()) return new HslColor<T>(Zero, Zero, 
            (l * 100.0).ConvertTo<T>(), (color.A / 255.0).ConvertTo<T>());
        var denominator = (l < 0.5) ? maxChannel + minChannel : 2.0 - maxChannel - minChannel;
        double s = maxDelta / denominator;

        var delR = ((maxChannel - varR) / 6.0 + maxDelta / 2.0) / maxDelta;
        var delG = ((maxChannel - varG) / 6.0 + maxDelta / 2.0) / maxDelta;
        var delB = ((maxChannel - varB) / 6.0 + maxDelta / 2.0) / maxDelta;

        if ((varR - maxChannel).IsZero()) h = delB - delG;
        else if ((varG - maxChannel).IsZero()) h = 1.0 / 3.0 + delR - delB;
        else if ((varB - maxChannel).IsZero()) h = 2.0 / 3.0 + delG - delR;
        // ReSharper disable once CommentTypo
        else h = 0.0; // Uwe Keim
            
        h = h.WrapAround(0, 1);

        return new HslColor<T>(
            (h * 360.0).ConvertTo<T>(),
            (s * 100.0).ConvertTo<T>(),
            (l * 100.0).ConvertTo<T>(),
            (color.A / 255.0).ConvertTo<T>());
    }
    
    [Pure] public Color ToColor()
    {
        T red, green, blue;
        var fullCircle = T.CreateChecked(360);
        var fullSatLum = T.CreateChecked(100);
        var oneHalf = T.CreateChecked(0.5);
        var two = T.CreateChecked(2);
        var three = T.CreateChecked(3);
        var oneThird = One / three;
        
        var h = Hue / fullCircle;
        var s = Saturation / fullSatLum;
        var l = Light / fullSatLum;

        if (s.IsZero()) red = green = blue = l;
        else
        {
            var var2 = l < oneHalf ? l + l * s : l + s - l * s;
            var var1 = two * l - var2;

            red = Hue2Rgb(var1, var2, h + oneThird);
            green = Hue2Rgb(var1, var2, h);
            blue = Hue2Rgb(var1, var2, h - oneThird);
        }
        
        var redByte = byte.CreateSaturating(T.Round(red * MaxByte));
        var greenByte = byte.CreateSaturating(T.Round(green * MaxByte));
        var blueByte = byte.CreateSaturating(T.Round(blue * MaxByte));
        var alphaByte = byte.CreateSaturating(T.Round(Alpha * MaxByte));
        return Color.FromArgb(alphaByte, redByte, greenByte, blueByte);
    }
}