using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;

namespace SFML_tutorial.Game.ComposedObjects;

public class Obstacle : Moveable
{
    private RectangleShape shape;
    private Vector2f size;
    public Vector2f Size
    {
        get => size;
        set
        {
            size = value;
            shape = new RectangleShape(Size)
            {
                Position = Position,
            };
        }
    }

    public Obstacle(FloatRect rect)
    {
        Position = new Vector2f(rect.Left, rect.Top);
        Size = new Vector2f(rect.Width, rect.Height);
        shape = new RectangleShape(Size)
        {
            Position = Position
        };
    }

    public override List<Drawable> Drawables => [shape];

    public override Collider2D Collider
    {
        get => new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            Bounds = new FloatRect(Position, Size),
            IsTrigger = false,
        };
    }

    public override void Update()
    {
        // Move(new Vector2f(1, 0));
    }
}
