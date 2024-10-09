# ColorSpaces
Color Space Utilities for C#

This little utility library provides a set of classes to convert between different color spaces.
Initial version was created by [Uwe Keim](https://gist.github.com/UweKeim) and is available as a [GitHub Gist](https://gist.github.com/UweKeim/fb7f829b852c209557bc49c51ba14c8b).

I refactored his code a bit, fixed several bugs, made a NuGet package out of it.

The following color spaces are supported:

- RGB, Float RGB (channels are float values between 0 and 1)
- HSL
- HSB
- CMYK
- 4:4:4 byte YUV