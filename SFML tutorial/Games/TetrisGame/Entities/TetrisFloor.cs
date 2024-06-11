using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;

namespace SFML_tutorial.Games.TetrisGame.Entities;
public class TetrisFloor(Vector2f size) : Positionable
{
    private readonly Vector2f size = size;

    public override List<Drawable> Drawables => [new RectangleShape {
        Position = Position,
        OutlineColor = Color.Red,
        OutlineThickness = 1,
        Size = size,
    }];
}
