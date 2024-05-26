using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML_tutorial.BaseEngine.CoreLibs.Composed;
using SFML_tutorial.BaseEngine.Window.Composed;
using static SFML.Window.Keyboard;

namespace SFML_tutorial.Game.ComposedObjects;

public class MoveableBox : Moveable
{
    private readonly RectangleShape rectangle;
    private readonly Collider2D boxCollider;
    private Vector2f input;

    public MoveableBox(Vector2f size)
    {
        rectangle = new RectangleShape(size)
        {
            Position = new(0, 0),
            FillColor = Color.Green
        };
        boxCollider = new Collider2D
        {
            PositionableGameObject = this,
            IsActive = true,
            IsStatic = false,
            Bounds = new FloatRect(Position, size)
        };
        GameWindow.Instance.RenderWindow.KeyPressed += HandleMovementKeyDown;
        GameWindow.Instance.RenderWindow.KeyReleased += HandleMovementKeyUp;
    }

    ~MoveableBox()
    {
        GameWindow.Instance.RenderWindow.KeyPressed -= HandleMovementKeyDown;
        GameWindow.Instance.RenderWindow.KeyReleased -= HandleMovementKeyUp;
    }

    //public override Vector2f Position
    //{
    //    get => rectangle.Position;
    //    set => rectangle.Position = value;
    //}
    public override Collider2D Collider => boxCollider;
    public override List<Drawable> Drawables => [rectangle];

    public override void Attach()
    {
        rectangle.Position = Position = (Vector2f)(GameWindow.Instance.RenderWindow.Size / 2);
    }

    public override void Update()
    {
        input = KeysPressedToInput();
        Move(input);
    }

    // U, D, L, R
    private readonly byte[] keysPressed = { 0, 0, 0, 0 };

    private Vector2f KeysPressedToInput()
    {
        int 
            up = -keysPressed[0],
            down = keysPressed[1],
            left = -keysPressed[2],
            right = keysPressed[3];
        return new Vector2f(left + right, up + down);
    }

    private void HandleMovementKeyDown(object? _, KeyEventArgs key)
    {
        if (key.Code is Key.Up or Key.W)
        {
            keysPressed[0] = 1;
        }
        if (key.Code is Key.Down or Key.S)
        {
            keysPressed[1] = 1;
        }
        if (key.Code is Key.Left or Key.A)
        {
            keysPressed[2] = 1;
        }
        if (key.Code is Key.Right or Key.D)
        {
            keysPressed[3] = 1;
        }
    }
    private void HandleMovementKeyUp(object? _, KeyEventArgs key)
    {
        if (key.Code is Key.Up or Key.W)
        {
            keysPressed[0] = 0;
        }
        if (key.Code is Key.Down or Key.S)
        {
            keysPressed[1] = 0;
        }
        if (key.Code is Key.Left or Key.A)
        {
            keysPressed[2] = 0;
        }
        if (key.Code is Key.Right or Key.D)
        {
            keysPressed[3] = 0;
        }
    }
}
