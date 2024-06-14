﻿using SFML.Graphics;
using Math = SFML_tutorial.BaseEngine.CoreLibs.Mathematics.Math;

namespace SFML_tutorial.BaseEngine.CoreLibs.SFMLExtensions;
public static class ColorExtensions
{
    public static Color Lerp(this Color color1, Color color2, float t)
    {
        t = System.Math.Clamp(t, 0.0f, 1.0f);

        byte r = (byte)(color1.R + (color2.R - color1.R) * t);
        byte g = (byte)(color1.G + (color2.G - color1.G) * t);
        byte b = (byte)(color1.B + (color2.B - color1.B) * t);
        byte a = (byte)(color1.A + (color2.A - color1.A) * t);
        
        return new Color(r, g, b, a);
    }

    public static Color PingPong(this Color color1, Color color2, float time, float cycleDuration)
    {
        return Lerp(color1, color2, Math.PingPong(time, cycleDuration) / cycleDuration);
    }

    public static Color Random(Random rnd)
        => new Color(unchecked((uint)rnd.Next(int.MaxValue)));

    public static Color RandomOpaque(Random rnd)
        => new Color((uint)rnd.Next(0x00FFFFFF) | 0xFF000000U);

    public static Color GenerateBrightColor(Random rnd) 
        => new Color((byte)rnd.Next(128, 256), (byte)rnd.Next(128, 256), (byte)rnd.Next(128, 256));

    public static Color GenerateDarkColor(Random rnd) 
        => new Color((byte)rnd.Next(128), (byte)rnd.Next(128), (byte)rnd.Next(128));

    public static Color GenerateWarmColor(Random rnd) 
        => new Color((byte)rnd.Next(128, 256), (byte)rnd.Next(256), (byte)rnd.Next(128));

    public static Color GenerateColdColor(Random rnd) 
        => new Color((byte)rnd.Next(128), (byte)rnd.Next(128), (byte)rnd.Next(128, 256));
}
