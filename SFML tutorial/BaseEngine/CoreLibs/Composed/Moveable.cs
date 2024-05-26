using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Math;
using SFML_tutorial.BaseEngine.Window.Composed;

namespace SFML_tutorial.BaseEngine.CoreLibs.Composed;

/// <summary>
/// Mixin/Trait class for things which can move in all directions on the screen.
/// </summary>
public abstract class Moveable : Positionable
{
    // add callbacks to GameWindow.RenderWindow for controlling an instance of a derived class
    public required float MoveSpeed { get; set; }

    public void Move(Vector2f input)
    {
        if (input != new Vector2f())
        {
            Vector2f inputNormal = input.Normalize();
            Vector2f delta = inputNormal * MoveSpeed * GameWindow.DeltaTime.AsSeconds();
            Position += delta;
        }
    }
}
