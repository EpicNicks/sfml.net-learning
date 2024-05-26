using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;

namespace SFML_tutorial.Games.PongGame.Entities;

/// <summary>
/// Base class for players
/// </summary>
public class PlayerPaddle : Moveable
{
    private readonly List<Keyboard.Key> upKeys;
    private readonly List<Keyboard.Key> downKeys;
    private readonly Collider2D collider;
    private readonly RectangleShape rectangleShape;

    private readonly Dictionary<Keyboard.Key, bool> pressedKeys;

    public PlayerPaddle(List<Keyboard.Key> upKeys, List<Keyboard.Key> downKeys, FloatRect bounds)
    {
        this.upKeys = upKeys;
        this.downKeys = downKeys;
        pressedKeys = [];
        foreach (var key in upKeys)
        {
            pressedKeys[key] = false;
        }
        foreach (var key in downKeys)
        {
            pressedKeys[key] = false;
        }
        Position = new Vector2f(bounds.Left, bounds.Top);
        collider = new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            IsTrigger = false,
            Bounds = bounds
        };
        rectangleShape = new RectangleShape
        {
            FillColor = Color.White,
            Size = new Vector2f(bounds.Width, bounds.Height),
            Position = Position,
        };
    }

    public override List<Drawable> Drawables => [rectangleShape];
    public override Collider2D Collider => collider;

    public override void Attach()
    {
        GameWindow.Instance.RenderWindow.KeyPressed += (_, e) =>
        {
            if (pressedKeys.ContainsKey(e.Code))
            {
                pressedKeys[e.Code] = true;
            }
        };
        GameWindow.Instance.RenderWindow.KeyReleased += (_, e) =>
        {
            if (pressedKeys.ContainsKey(e.Code))
            {
                pressedKeys[e.Code] = false;
            }
        };
    }

    public override void Update()
    {
        static int IfTrueThen(bool b, int value) => b ? value : 0;
        float yVelocity = 0 + IfTrueThen(upKeys.Any(key => pressedKeys[key]), -1) + IfTrueThen(downKeys.Any(key => pressedKeys[key]), 1);
        Move(new Vector2f(0, yVelocity));
        Position = new Vector2f(Position.X, Math.Clamp(Position.Y, 0, GameWindow.Instance.RenderWindow.Size.Y - Collider.Bounds.Height));
    }
}
