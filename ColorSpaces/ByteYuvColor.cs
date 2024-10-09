using System.Drawing;

namespace ColorSpaces;

/// <summary>
/// Represents an element of the 4:4:4 YUV color space.
/// All components are in the byte range [0, 255].
/// </summary>
public readonly record struct ByteYuvColor(byte Y, byte U, byte V, byte Alpha) : IColor<ByteYuvColor> 
{
    public Color ToColor()
    {
        int c = Y - 16;
        int d = U - 128;
        int e = V - 128;

        byte r = byte.CreateSaturating((298 * c + 409 * e + 128) >> 8);
        byte g = byte.CreateSaturating((298 * c - 100 * d - 208 * e + 128) >> 8);
        byte b = byte.CreateSaturating((298 * c + 516 * d + 128) >> 8);
        return Color.FromArgb(Alpha, r, g, b);
    }

    public static implicit operator ByteYuvColor(Color color) => FromColor(color);

    public static implicit operator Color(ByteYuvColor color) => color.ToColor();

    public static ByteYuvColor FromColor(Color color)
    {
        byte y = byte.CreateSaturating(((66 * color.R + 129 * color.G + 25 * color.B + 128) >> 8) + 16);
        byte u = byte.CreateSaturating(((-38 * color.R - 74 * color.G + 112 * color.B + 128) >> 8) + 128);
        byte v = byte.CreateSaturating(((112 * color.R - 94 * color.G + 25 * color.B + 128) >> 8) + 128);
        return new ByteYuvColor(y, u, v, color.A);
    }
}