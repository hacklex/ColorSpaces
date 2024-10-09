using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ColorSpaces;

namespace WpfSample;

public class ColorPaletteViewer : Control
{
    public static readonly DependencyProperty HueProperty = DependencyProperty.Register(
        nameof(Hue), typeof(double), typeof(ColorPaletteViewer), new FrameworkPropertyMetadata(InvalidateVisualCallback));

    private static void InvalidateVisualCallback(DependencyObject s, DependencyPropertyChangedEventArgs ea)
    {
        if (s is ColorPaletteViewer cpv)
        {
            cpv.InvalidateVisual();
        }
    }

    public double Hue
    {
        get { return (double)GetValue(HueProperty); }
        set { SetValue(HueProperty, value); }
    }

    public static readonly DependencyProperty AlphaProperty = DependencyProperty.Register(
        nameof(Alpha), typeof(byte), typeof(ColorPaletteViewer), new FrameworkPropertyMetadata(InvalidateVisualCallback));

    public byte Alpha
    {
        get { return (byte)GetValue(AlphaProperty); }
        set { SetValue(AlphaProperty, value); }
    }

    public static readonly DependencyProperty UseHsbInsteadOfHslProperty = DependencyProperty.Register(
        nameof(UseHsbInsteadOfHsl), typeof(bool), typeof(ColorPaletteViewer), new FrameworkPropertyMetadata(InvalidateVisualCallback));

    public bool UseHsbInsteadOfHsl
    {
        get { return (bool)GetValue(UseHsbInsteadOfHslProperty); }
        set { SetValue(UseHsbInsteadOfHslProperty, value); }
    }
    
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        var rect = new Rect(0, 0, ActualWidth, ActualHeight);
        var stepX = rect.Width / 101;
        var stepY = rect.Height / 101;
        for (int sat = 0; sat < 101; sat++)
        {
            for (int val = 0; val < 101; val++)
            {
                System.Drawing.Color color = UseHsbInsteadOfHsl
                    ? (new HsbColor<double>(Hue, sat, val, Alpha)).ToColor()
                    : (new HslColor<double>(Hue, sat, val, Alpha)).ToColor();
                var brush = new SolidColorBrush(color.ToWpfColor());
                var pen = new Pen(brush, 1);
                drawingContext.DrawRectangle(brush, pen, new Rect(rect.TopLeft + new Vector(sat * stepX, val * stepY), new Size(stepX, stepY)).PrepareForAliasing());
            }
        }
    }
}