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

    public required bool IsLeftSidePlayer { get; set; }

    public PlayerPaddle(List<Keyboard.Key> upKeys, List<Keyboard.Key> downKeys, Vector2f size)
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
        collider = new Collider2D
        {
            IsStatic = true,
            PositionableGameObject = this,
            IsTrigger = false,
            Bounds = new FloatRect(Position, size)
        };
        rectangleShape = new RectangleShape
        {
            FillColor = Color.White,
            Size = size,
            Position = Position,
        };
    }

    public override List<Drawable> Drawables => [rectangleShape];
    public override Collider2D Collider => collider;

    public override void Attach()
    {
        Position = new Vector2f(Position.X, GameWindow.Size.Y / 2f);
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
        HandleResize();
        HandleMove();
    }

    private void HandleMove()
    {
        static int IfTrueThen(bool b, int value) => b ? value : 0;
        float yVelocity = 0 + IfTrueThen(upKeys.Any(key => pressedKeys[key]), -1) + IfTrueThen(downKeys.Any(key => pressedKeys[key]), 1);
        yVelocity *= GameWindow.Size.Y / 800; // scaled compared to the original window size of x=1200 y=800
        Move(new Vector2f(0, yVelocity));
        Position = new Vector2f(Position.X, Math.Clamp(Position.Y, 0, GameWindow.Size.Y - Collider.Bounds.Height));
    }

    private void HandleResize()
    {
        if (IsLeftSidePlayer)
        {
            Position = new Vector2f(200, Position.Y);
        }
        else
        {
            Position = new Vector2f(GameWindow.Size.X - 200, Position.Y);
        }
    }
}
