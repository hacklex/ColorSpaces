using System.Numerics;
using ColorSpaces.Annotations;

namespace ColorSpaces;

/// <summary>
/// Contains extension methods for the <see cref="double"/> type.
/// </summary>
[PublicAPI] public static class MathExtensions
{
    private const double ToleranceForIsZero = 0.00001;
    private static class Helper<T> where T : IFloatingPoint<T> 
    {
        // in the future, we may want to implement a switch to change the tolerance value
        // for different types of floating-point numbers
        public static readonly T ToleranceForIsZeroCheck = T.CreateChecked(0.00001);
    }
    
    /// <summary>
    /// Creates a new instance of the <typeparamref name="T"/> type from the specified value.
    /// Uses the <see cref="IFloatingPoint{T}.CreateSaturating{T}"/> method to create the instance.
    /// This means that the value will be clamped to the range of the <typeparamref name="T"/> type.
    /// </summary>
    /// <param name="x">The value to convert.</param>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <returns>A new instance of the <typeparamref name="T"/> type created from the specified value.</returns>
    [Pure] public static T ConvertTo<T>(this double x) where T : IFloatingPoint<T> => T.CreateSaturating(x);
    
    /// <summary>Determines whether the specified value can be considered zero.</summary>
    /// <param name="x">The value to check.</param>
    /// <param name="tolerance">The strictly positive tolerance value to use.</param>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <returns><c>true</c> if the specified value can be considered zero with the specified tolerance; otherwise, <c>false</c>.</returns>
    [Pure] public static bool IsZero<T>(this T x, T? tolerance = default) where T :  IFloatingPoint<T>
        => (x < T.Zero ? -x : x) < (tolerance ?? Helper<T>.ToleranceForIsZeroCheck);
    
    /// <summary>Determines whether the current value equals the specified value within the specified tolerance.</summary>
    /// <param name="x">The current value.</param>
    /// <param name="other">The value to compare with the current value.</param>
    /// <param name="tolerance">The strictly positive tolerance value to use.</param>
    /// <typeparam name="T">The type of the value to compare.</typeparam>
    /// <returns><c>true</c> if the current value equals the specified value within the specified tolerance; otherwise, <c>false</c>.</returns>
    [Pure] public static bool AlmostEquals<T>(this T x, T other, T tolerance) where T : IFloatingPoint<T>
        => (x - other).IsZero(tolerance);
    
    /// <summary>Determines whether the current value equals the specified value within the default tolerance.</summary>
    /// <param name="x">The current value.</param>
    /// <param name="other">The value to compare with the current value.</param>
    /// <typeparam name="T">The type of the value to compare.</typeparam>
    /// <returns><c>true</c> if the current value equals the specified value within the default tolerance; otherwise, <c>false</c>.</returns>
    [Pure] public static bool AlmostEquals<T>(this T x, T other) where T : IFloatingPoint<T>
        => AlmostEquals(x, other, Helper<T>.ToleranceForIsZeroCheck);
    
    /// <summary>
    /// Determines whether the specified value can be considered zero.
    /// If the tolerance is not specified, the default value of 0.00001 is used. 
    /// </summary>
    /// <param name="x">The value to check</param>
    /// <param name="tolerance">The strictly positive tolerance value to use</param>
    /// <returns><c>true</c> if the specified value can be considered zero; otherwise, <c>false</c>.</returns>
    [Pure] public static bool IsZero(this double x, double? tolerance = null) => Math.Abs(x) < (tolerance ?? ToleranceForIsZero);
    /// <summary>
    /// Shifts the value to the specified range.
    /// </summary>
    /// <param name="value">The value to shift.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    [Pure] public static double WrapAround(this double value, double min, double max)
    {
        var range = max - min;
        var result = (value - min) % range;
        return min + (result < 0 ? result + range : result);
    }
    
    /// <summary>
    /// Shifts the value to the specified range.
    /// </summary>
    /// <param name="value">The value to shift.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <returns>The value shifted to the specified range.</returns>
    [Pure] public static T WrapAround<T>(this T value, T min, T max) where T : IFloatingPoint<T>
    {
        var range = max - min;
        var result = (value - min) % range;
        return min + (result < T.Zero ? result + range : result);
    }
    
    /// <summary>Simultaneously calculates the minimum and maximum values from the specified sequence of numbers.</summary>
    /// <param name="values">The sequence of numbers to process.</param>
    /// <returns>A tuple containing the minimum and maximum values from the specified sequence.</returns>
    [Pure] public static (double min, double max) GetMinMax(params double[] values)
    {
        var minValue = values[0];
        var maxValue = values[0];

        if (values.Length >= 2)
        {
            for (var i = 1; i < values.Length; i++)
            {
                var num = values[i];
                minValue = Math.Min(minValue, num);
                maxValue = Math.Max(maxValue, num);
            }
        }
        return (minValue, maxValue);
    }
}