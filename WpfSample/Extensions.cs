using System.Windows;
using System.Windows.Media;

namespace WpfSample;

public static class Extensions
{
    public static Point PrepareForAliasing(this Point p) => new Point(0.5 + Math.Floor(p.X), 0.5 + Math.Floor(p.Y));
    public static Rect PrepareForAliasing(this Rect r) => new Rect(r.TopLeft.PrepareForAliasing(), r.BottomRight.PrepareForAliasing());
    public static System.Windows.Media.Color ToWpfColor(this System.Drawing.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);
}