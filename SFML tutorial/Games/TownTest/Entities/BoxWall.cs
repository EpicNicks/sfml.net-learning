using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;

namespace SFML_tutorial.Games.TownTest.Entities;
public class BoxWall : Positionable
{
    public required Color Color { get; set; }
    public required Vector2f Size { get; set; }
    public override required Vector2f Position { get => base.Position; set => base.Position = value; }

    public override List<Drawable> Drawables => [new RectangleShape {
        FillColor = Color,
        Position = Position,
        Size = Size,
    }];

    public override Collider2D Collider => new Collider2D
    {
        Bounds = new FloatRect(Position, Size),
        PositionableGameObject = this,
        IsStatic = true,
        IsTrigger = false,
    };
}
