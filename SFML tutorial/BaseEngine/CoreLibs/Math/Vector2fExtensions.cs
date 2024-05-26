using SFML.Graphics;
using SFML.System;

using static System.Math;

namespace SFML_tutorial.BaseEngine.CoreLibs.Math;

public static class Vector2fExtensions
{
    public static Vector2f ToVector(this (float x, float y) vectorTuple) => new Vector2f(vectorTuple.x, vectorTuple.y);
    public static double Length(this Vector2f vector2F) => vector2F == new Vector2f() ? 0 : Sqrt(Pow(vector2F.X, 2) + Pow(vector2F.Y, 2));
    public static Vector2f Normalize(this Vector2f vector2F) => vector2F / (float)vector2F.Length();
}
