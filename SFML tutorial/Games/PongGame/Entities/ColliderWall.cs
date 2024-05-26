using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;

namespace SFML_tutorial.Games.PongGame.Entities;
/// <summary>
/// A Positionable Invisible Collider with a Position and Bounds
/// </summary>
public class ColliderWall : Positionable
{
    private Collider2D collider;
    public ColliderWall(Vector2f position, Vector2f size) : this(new FloatRect(position, size)) { }

    public override Vector2f Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            collider.Bounds = new FloatRect(value.X, value.Y, collider.Bounds.Width, collider.Bounds.Height);
        }
    }

    public ColliderWall(FloatRect bounds = default)
    {
        collider = new Collider2D
        {
            PositionableGameObject = this,
            IsStatic = true,
            IsTrigger = false,
            Bounds = bounds
        };
        Position = new Vector2f(bounds.Left, bounds.Top);
    }

    // For debugging, remove later
    public override List<Drawable> Drawables => [new RectangleShape { Size = new Vector2f(collider.Bounds.Width, collider.Bounds.Height), Position = Position }];
    public override Collider2D Collider => collider;
}
