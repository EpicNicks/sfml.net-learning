using SFML.Graphics;
using SFML.System;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;

namespace SFML_tutorial.Games.PongGame.Entities;
/// <summary>
/// A Positionable Invisible Collider with a Position and Bounds
/// </summary>
public class ColliderWall : Positionable
{
    private RectangleShape debugRectangle;
    private Collider2D collider;
    public required bool IsTopWall { get; set; }

    public override Vector2f Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            collider.Bounds = new FloatRect(value.X, value.Y, collider.Bounds.Width, collider.Bounds.Height);
        }
    }

    public ColliderWall()
    {
        collider = new Collider2D
        {
            PositionableGameObject = this,
            IsStatic = true,
            IsTrigger = false,
        };
        debugRectangle = new RectangleShape
        {
            Size = new Vector2f(collider.Bounds.Width, collider.Bounds.Height),
            Position = Position
        };
    }

    public override void Update()
    {
        HandleResize();
    }

    private void HandleResize()
    {
        Vector2u windowSize = GameWindow.Instance.RenderWindow.Size;
        if (IsTopWall)
        {
            // ensure position is locked to top
            // ensure width is correct
            Position = new Vector2f(10, -10);
            collider.Bounds = new FloatRect(Position.X, Position.Y, windowSize.X - 20, 10);
        }
        else
        {
            // ensure position is locked to bottom
            // ensure width is correct
            Position = new Vector2f(10, windowSize.Y);
            collider.Bounds = new FloatRect(Position.X, Position.Y, windowSize.X - 20, 10);
        }
        debugRectangle.Size = new Vector2f(collider.Bounds.Width, collider.Bounds.Height);
        debugRectangle.Position = Position;
    }

    // For debugging, remove later
    public override List<Drawable> Drawables => [debugRectangle];
    public override Collider2D Collider => collider;
}
